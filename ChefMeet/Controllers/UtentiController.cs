using ChefMeet.Data;
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

        public UtentiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
    }
}
