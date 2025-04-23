using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ChefMeet.Models;
using ChefMeet.Models.DTOs;
using ChefMeet.Data;
using Microsoft.AspNetCore.Identity;

namespace ChefMeet.Controllers
{
    [Authorize(Roles = "Utente")]
    [ApiController]
    [Route("api/[controller]")]
    public class PrenotazioneController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PrenotazioneController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 📌 GET - Singola prenotazione
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrenotazione(int id)
        {
            var prenotazione = await _context.Prenotazioni
                .Include(p => p.Utente)
                .Include(p => p.Evento)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prenotazione == null)
                return NotFound();

            var dto = new PrenotazioneDTO
            {
                Id = prenotazione.Id,
                EventoId = prenotazione.EventoId,
                EventoTitolo = prenotazione.Evento.Titolo,
                UtenteNome = prenotazione.Utente.Nome + " " + prenotazione.Utente.Cognome,
                DataPrenotazione = prenotazione.DataPrenotazione
            };

            return Ok(dto);
        }

        // 📌 GET - Prenotazioni per utente
        [HttpGet("utente/{utenteId}")]
        public async Task<IActionResult> GetPrenotazioniUtente(string utenteId)
        {
            var prenotazioni = await _context.Prenotazioni
                .Include(p => p.Evento)
                .Include(p => p.Utente)
                .Where(p => p.UtenteId == utenteId)
                .Select(p => new PrenotazioneDTO
                {
                    Id = p.Id,
                    EventoId = p.EventoId,
                    EventoTitolo = p.Evento.Titolo,
                    UtenteNome = p.Utente.Nome + " " + p.Utente.Cognome,
                    DataPrenotazione = p.DataPrenotazione
                })
                .ToListAsync();

            return Ok(prenotazioni);
        }

        // 📌 POST - Effettua una prenotazione
        [HttpPost]
        public async Task<IActionResult> Prenota([FromBody] int eventoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var evento = await _context.Eventi.FindAsync(eventoId);
            if (evento == null) return NotFound("Evento non trovato.");

            var prenotazione = new Prenotazione
            {
                EventoId = eventoId,
                UtenteId = userId,
                DataPrenotazione = DateTime.UtcNow
            };

            _context.Prenotazioni.Add(prenotazione);
            await _context.SaveChangesAsync();

            return Ok("Prenotazione effettuata.");
        }

        // 📌 DELETE - Cancella una prenotazione
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancellaPrenotazione(int id)
        {
            var prenotazione = await _context.Prenotazioni.FindAsync(id);
            if (prenotazione == null)
                return NotFound();

            _context.Prenotazioni.Remove(prenotazione);
            await _context.SaveChangesAsync();

            return Ok($"Prenotazione {id} cancellata.");
        }
    }
}
