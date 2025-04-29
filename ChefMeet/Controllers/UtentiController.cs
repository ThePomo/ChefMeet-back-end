using ChefMeet.Data;
using ChefMeet.DTOs;
using ChefMeet.Models;
using ChefMeet.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChefMeet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtentiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public UtentiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // 📌 GET: /api/Utenti/ricerca?nome=...
        [HttpGet("ricerca")]
        [AllowAnonymous]
        public async Task<IActionResult> RicercaUtenti([FromQuery] string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest("Parametro nome mancante.");

            var utenti = await _context.Users
                .Where(u => u.Nome.Contains(nome) || u.Cognome.Contains(nome))
                .ToListAsync();

            var utentiDTO = new List<UtenteDTO>();

            foreach (var utente in utenti)
            {
                var ruolo = (await _userManager.GetRolesAsync(utente)).FirstOrDefault() ?? "Utente";
                var chef = await _context.Chefs.FirstOrDefaultAsync(c => c.UserId == utente.Id);

                utentiDTO.Add(new UtenteDTO
                {
                    Id = utente.Id,
                    Nome = utente.Nome,
                    Cognome = utente.Cognome,
                    Email = utente.Email,
                    Ruolo = ruolo,
                    ChefId = chef?.Id,
                    ImmagineProfilo = utente.ImmagineProfilo
                });
            }

            return Ok(utentiDTO);
        }

        // 📌 GET: /api/Utenti/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUtenteById(string id)
        {
            var utente = await _context.Users.FindAsync(id);
            if (utente == null) return NotFound("Utente non trovato.");

            var ruolo = (await _userManager.GetRolesAsync(utente)).FirstOrDefault() ?? "Utente";

            return Ok(new UtenteDTO
            {
                Id = utente.Id,
                Nome = utente.Nome,
                Cognome = utente.Cognome,
                Email = utente.Email,
                Ruolo = ruolo,
                ImmagineProfilo = utente.ImmagineProfilo,
                ChefId = await _context.Chefs
                    .Where(c => c.UserId == utente.Id)
                    .Select(c => (int?)c.Id)
                    .FirstOrDefaultAsync()
            });
        }

        // 📌 PUT: /api/Utenti/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> ModificaProfilo(string id, [FromForm] UpdateProfiloDTO dto, IFormFile? immagineProfilo)
        {
            var utente = await _userManager.FindByIdAsync(id);
            if (utente == null) return NotFound("Utente non trovato.");

            if (!string.IsNullOrWhiteSpace(dto.Nome))
                utente.Nome = dto.Nome;

            if (!string.IsNullOrWhiteSpace(dto.Cognome))
                utente.Cognome = dto.Cognome;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                utente.Email = dto.Email;

            if (immagineProfilo != null && immagineProfilo.Length > 0)
            {
                var folderPath = Path.Combine(_env.WebRootPath, "uploads", "utenti");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(immagineProfilo.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await immagineProfilo.CopyToAsync(stream);
                }

                utente.ImmagineProfilo = $"/uploads/utenti/{fileName}";
            }

            await _userManager.UpdateAsync(utente);
            return Ok("Profilo aggiornato con successo.");
        }
    }
}
