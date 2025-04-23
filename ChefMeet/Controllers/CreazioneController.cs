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
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreazioneController(IWebHostEnvironment env, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _env = env;
            _context = context;
            _userManager = userManager;
        }

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
                IsChef = creazione.IsChef,
                NumeroLike = creazione.Likes.Count
            };

            return Ok(dto);
        }

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
                    IsChef = c.IsChef,
                    NumeroLike = c.Likes.Count
                })
                .ToList();

            return Ok(creazioni);
        }

        [HttpGet("ricettario")]
        [AllowAnonymous]
        public IActionResult Ricettario([FromQuery] bool? soloChef, [FromQuery] string? keyword)
        {
            var query = _context.Creazioni
                .Include(c => c.Creatore)
                .Include(c => c.Likes)
                .AsQueryable();

            if (soloChef.HasValue && soloChef.Value)
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
                    IsChef = c.IsChef,
                    NumeroLike = c.Likes.Count
                })
                .ToList();

            return Ok(risultati);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreaCreazione([FromForm] CreazioneFormModel form)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            if (form.Immagine == null || form.Immagine.Length == 0)
                return BadRequest("Immagine non valida.");

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

            int? chefId = null;

            if (user.Ruolo == "Chef")
            {
                var chef = await _context.Chefs.FirstOrDefaultAsync(c => c.UserId == user.Id);
                if (chef != null)
                {
                    chefId = chef.Id;
                }
                else
                {
                    return BadRequest("Chef non trovato nel database.");
                }
            }

            var creazione = new Creazione
            {
                Nome = form.Nome,
                Descrizione = form.Descrizione,
                Immagine = imageUrl,
                CreatoreId = user.Id,
                IsChef = user.Ruolo == "Chef",
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
                IsChef = creazione.IsChef,
                NumeroLike = 0
            };

            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ModificaCreazione(int id, [FromBody] UpdateCreazioneDTO dto)
        {
            var creazione = await _context.Creazioni.FindAsync(id);
            if (creazione == null)
                return NotFound();

            creazione.Nome = dto.Nome;
            creazione.Descrizione = dto.Descrizione;

            await _context.SaveChangesAsync();

            return Ok($"Creazione {id} modificata con successo");
        }

        [HttpDelete]
        public async Task<IActionResult> EliminaCreazione([FromBody] DeleteCreazioneDTO dto)
        {
            var creazione = await _context.Creazioni.FindAsync(dto.Id);
            if (creazione == null)
                return NotFound();

            _context.Creazioni.Remove(creazione);
            await _context.SaveChangesAsync();

            return Ok($"Creazione {dto.Id} eliminata");
        }
    }
}
