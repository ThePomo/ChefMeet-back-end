using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChefMeet.Models.DTOs;
using ChefMeet.Models.FormModels;
using ChefMeet.Models;
using ChefMeet.Data;
using System.Security.Claims;

namespace ChefMeet.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EventoController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public EventoController(IWebHostEnvironment env, ApplicationDbContext context)
        {
            _env = env;
            _context = context;
        }

        // 📌 GET - Evento singolo
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvento(int id)
        {
            var evento = await _context.Eventi
                .Include(e => e.Chef)
                .ThenInclude(c => c.Utente)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null)
                return NotFound();

            var dto = new EventoDTO
            {
                Id = evento.Id,
                Titolo = evento.Titolo,
                Descrizione = evento.Descrizione,
                Data = evento.Data,
                Prezzo = evento.Prezzo,
                Immagine = evento.Immagine,
                ChefNome = evento.Chef.Utente != null
                    ? $"{evento.Chef.Utente.Nome} {evento.Chef.Utente.Cognome}"
                    : "Chef",
                ChefUserId = evento.Chef.UserId
            };

            return Ok(dto);
        }

        // 📌 GET - Tutti gli eventi (ordinati dal più recente)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTuttiEventi()
        {
            try
            {
                var eventi = await _context.Eventi
                    .Include(e => e.Chef)
                    .ThenInclude(c => c.Utente)
                    .OrderByDescending(e => e.Data)
                    .ToListAsync();

                var dtoList = eventi.Select(e => new EventoDTO
                {
                    Id = e.Id,
                    Titolo = e.Titolo,
                    Descrizione = e.Descrizione,
                    Data = e.Data,
                    Prezzo = e.Prezzo,
                    Immagine = e.Immagine,
                    ChefNome = e.Chef != null && e.Chef.Utente != null
                        ? $"{e.Chef.Utente.Nome} {e.Chef.Utente.Cognome}"
                        : "Chef sconosciuto",
                    ChefUserId = e.Chef?.UserId
                }).ToList();

                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Errore in GetTuttiEventi: {ex.Message}");
                return StatusCode(500, "Errore interno del server");
            }
        }


        
        // 📌 POST - Crea nuovo evento (immagine obbligatoria)
        [HttpPost]
        [Authorize(Roles = "Chef")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreaEvento([FromForm] EventoFormModel form)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var chef = await _context.Chefs
                .Include(c => c.Utente)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (chef == null)
                return BadRequest("Chef non trovato.");

            if (form.Immagine == null || form.Immagine.Length == 0)
                return BadRequest("L'immagine è obbligatoria.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(form.Immagine.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await form.Immagine.CopyToAsync(stream);
            }

            var imageUrl = $"/uploads/{fileName}";

            var evento = new Evento
            {
                Titolo = form.Titolo,
                Descrizione = form.Descrizione,
                Data = form.Data,
                Prezzo = form.Prezzo,
                Immagine = imageUrl,
                ChefId = chef.Id
            };

            _context.Eventi.Add(evento);
            await _context.SaveChangesAsync();

            var dto = new EventoDTO
            {
                Id = evento.Id,
                Titolo = evento.Titolo,
                Descrizione = evento.Descrizione,
                Data = evento.Data,
                Prezzo = evento.Prezzo,
                Immagine = evento.Immagine,
                ChefNome = chef.Utente != null
                    ? $"{chef.Utente.Nome} {chef.Utente.Cognome}"
                    : "Chef",
                ChefUserId = chef.UserId
            };

            return Ok(dto);
        }


        // 📌 PUT - Modifica evento
        [HttpPut("{id}")]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> ModificaEvento(int id, [FromBody] EventoDTO dto)
        {
            var evento = await _context.Eventi.FindAsync(id);
            if (evento == null)
                return NotFound();

            evento.Titolo = dto.Titolo;
            evento.Descrizione = dto.Descrizione;
            evento.Data = dto.Data;
            evento.Prezzo = dto.Prezzo;

            await _context.SaveChangesAsync();

            return Ok($"Evento {id} aggiornato");
        }

        // 📌 DELETE - Elimina evento
        [HttpDelete("{id}")]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> EliminaEvento(int id)
        {
            var evento = await _context.Eventi.FindAsync(id);
            if (evento == null)
                return NotFound();

            _context.Eventi.Remove(evento);
            await _context.SaveChangesAsync();

            return Ok($"Evento {id} eliminato");
        }

        // 📌 GET - Eventi dello chef loggato
        [HttpGet("miei-eventi")]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> GetMieiEventi()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return BadRequest("Utente non valido.");

            var chef = await _context.Chefs
                .Include(c => c.Utente)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (chef == null)
                return BadRequest("Chef non trovato.");

            var eventi = await _context.Eventi
                .Where(e => e.ChefId == chef.Id)
                .Include(e => e.Chef)
                .ThenInclude(c => c.Utente)
                .OrderByDescending(e => e.Data)
                .ToListAsync();

            var dtoList = eventi.Select(e => new EventoDTO
            {
                Id = e.Id,
                Titolo = e.Titolo,
                Descrizione = e.Descrizione,
                Data = e.Data,
                Prezzo = e.Prezzo,
                Immagine = e.Immagine,
                ChefNome = e.Chef != null && e.Chef.Utente != null
                    ? $"{e.Chef.Utente.Nome} {e.Chef.Utente.Cognome}"
                    : "Chef sconosciuto",
                ChefUserId = e.Chef?.UserId
            }).ToList();

            return Ok(dtoList);
        }

    }
}

