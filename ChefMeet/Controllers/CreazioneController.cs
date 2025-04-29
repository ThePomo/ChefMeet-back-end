using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChefMeet.Models.FormModels;
using ChefMeet.Models.DTOs;
using ChefMeet.Data;
using ChefMeet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChefMeet.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CreazioneController : ControllerBase
    {
        private readonly IWebHostEnvironment _env; private readonly ApplicationDbContext _context; private readonly UserManager<ApplicationUser> _userManager;

    public CreazioneController(IWebHostEnvironment env, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _env = env;
            _context = context;
            _userManager = userManager;
        }

        // 📌 GET singola creazione
        [HttpGet("{id}")]
        public IActionResult GetCreazione(int id)
        {
            var creazione = _context.Creazioni
                .Include(c => c.Creatore)
                .Include(c => c.Likes)
                .FirstOrDefault(c => c.Id == id);

            if (creazione == null)
                return NotFound();

            var dto = new CreazioneDTO
            {
                Id = creazione.Id,
                Nome = creazione.Nome,
                Descrizione = creazione.Descrizione,
                Immagine = creazione.Immagine,
                Autore = creazione.Creatore.Nome + " " + creazione.Creatore.Cognome,
                CreatoreId = creazione.CreatoreId,  // ✅ importante per collegare al profilo
                IsChef = creazione.IsChef,
                NumeroLike = creazione.Likes.Count
            };

            return Ok(dto);
        }

        // 📌 GET tutte le creazioni
        [HttpGet]
        public IActionResult GetTutteCreazioni()
        {
            var creazioni = _context.Creazioni
                .Include(c => c.Creatore)
                .Include(c => c.Likes)
                .Select(c => new CreazioneDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Descrizione = c.Descrizione,
                    Immagine = c.Immagine,
                    Autore = c.Creatore.Nome + " " + c.Creatore.Cognome,
                    CreatoreId = c.CreatoreId,
                    IsChef = c.IsChef,
                    NumeroLike = c.Likes.Count
                })
                .ToList();

            return Ok(creazioni);
        }

        // 📌 GET ricettario pubblico
        [HttpGet("ricettario")]
        [AllowAnonymous]
        public IActionResult Ricettario([FromQuery] bool? soloChef, [FromQuery] string? keyword)
        {
            var query = _context.Creazioni
                .Include(c => c.Creatore)
                .Include(c => c.Likes)
                .AsQueryable();

            if (soloChef == true)
                query = query.Where(c => c.IsChef);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(c => c.Nome.Contains(keyword) || c.Descrizione.Contains(keyword));

            var risultati = query
                .Select(c => new CreazioneDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Descrizione = c.Descrizione,
                    Immagine = c.Immagine,
                    Autore = c.Creatore.Nome + " " + c.Creatore.Cognome,
                    CreatoreId = c.CreatoreId,
                    IsChef = c.IsChef,
                    NumeroLike = c.Likes.Count
                })
                .ToList();

            return Ok(risultati);
        }

        // 📌 GET creazioni di un utente
        [HttpGet("byUser/{userId}")]
        [AllowAnonymous]
        public IActionResult GetCreazioniByUser(string userId)
        {
            var creazioni = _context.Creazioni
                .Include(c => c.Creatore)
                .Include(c => c.Likes)
                .Where(c => c.CreatoreId == userId)
                .Select(c => new CreazioneDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Descrizione = c.Descrizione,
                    Immagine = c.Immagine,
                    Autore = c.Creatore.Nome + " " + c.Creatore.Cognome,
                    CreatoreId = c.CreatoreId,
                    IsChef = c.IsChef,
                    NumeroLike = c.Likes.Count
                })
                .ToList();

            return Ok(creazioni);
        }

        // 📌 POST - Crea ricetta
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreaCreazione([FromForm] CreazioneFormModel form)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            if (form.Immagine == null || form.Immagine.Length == 0)
                return BadRequest("Immagine non valida.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(form.Immagine.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await form.Immagine.CopyToAsync(stream);
            }

            var imageUrl = $"/uploads/{fileName}";
            int? chefId = null;
            var isChef = user.Ruolo == "Chef";

            if (isChef)
            {
                var chef = await _context.Chefs.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (chef != null)
                    chefId = chef.Id;
            }

            var creazione = new Creazione
            {
                Nome = form.Nome,
                Descrizione = form.Descrizione,
                Immagine = imageUrl,
                CreatoreId = user.Id,
                IsChef = isChef,
                ChefId = chefId
            };

            _context.Creazioni.Add(creazione);
            await _context.SaveChangesAsync();

            var dto = new CreazioneDTO
            {
                Id = creazione.Id,
                Nome = creazione.Nome,
                Descrizione = creazione.Descrizione,
                Immagine = creazione.Immagine,
                Autore = user.Nome + " " + user.Cognome,
                CreatoreId = user.Id,
                IsChef = creazione.IsChef,
                NumeroLike = 0
            };

            return Ok(dto);
        }

        // 📌 PUT - Modifica ricetta
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ModificaCreazione(int id, [FromForm] CreazioneFormModel form)
        {
            var creazione = await _context.Creazioni.FindAsync(id);
            if (creazione == null) return NotFound();

            creazione.Nome = form.Nome;
            creazione.Descrizione = form.Descrizione;

            if (form.Immagine != null && form.Immagine.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(form.Immagine.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await form.Immagine.CopyToAsync(stream);
                }

                creazione.Immagine = $"/uploads/{fileName}";
            }

            await _context.SaveChangesAsync();
            return Ok($"Creazione {id} modificata con successo");
        }

        // 📌 DELETE - Elimina ricetta
        [HttpDelete]
        public async Task<IActionResult> EliminaCreazione([FromBody] DeleteCreazioneDTO dto)
        {
            var creazione = await _context.Creazioni.FindAsync(dto.Id);
            if (creazione == null) return NotFound();

            _context.Creazioni.Remove(creazione);
            await _context.SaveChangesAsync();

            return Ok($"Creazione {dto.Id} eliminata");
        }
    }
}