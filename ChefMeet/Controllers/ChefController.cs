using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChefMeet.Data;
using ChefMeet.Models;
using ChefMeet.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ChefMeet.Controllers
{
    [Authorize(Roles = "Chef")]
    [ApiController]
    [Route("api/[controller]")]
    public class ChefController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChefController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📌 GET - Dettagli di uno chef
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChefById(int id)
        {
            var chef = await _context.Chefs
                .Include(c => c.Utente)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chef == null)
                return NotFound();

            var dto = new ChefDTO
            {
                Id = chef.Id,
                Nome = chef.Utente.Nome,
                Cognome = chef.Utente.Cognome,
                Email = chef.Utente.Email,
                Bio = chef.Biografia,
                Città = chef.Città,
                ImmagineProfilo = chef.ImmagineProfilo,
                UserId = chef.UserId // ✅ aggiunto per confrontare nel frontend
            };

            return Ok(dto);
        }

        // 📌 GET - Ottieni chef tramite UserId (string)
        [HttpGet("byUser/{userId}")]
        public async Task<IActionResult> GetChefByUserId(string userId)
        {
            var chef = await _context.Chefs
                .Include(c => c.Utente)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (chef == null)
                return BadRequest("Nessuno chef trovato per questo utente.");

            var dto = new ChefDTO
            {
                Id = chef.Id,
                Nome = chef.Utente.Nome,
                Cognome = chef.Utente.Cognome,
                Email = chef.Utente.Email,
                Bio = chef.Biografia,
                Città = chef.Città,
                ImmagineProfilo = chef.ImmagineProfilo,
                UserId = chef.UserId // ✅ aggiunto anche qui
            };

            return Ok(dto);
        }

        // 📌 POST - Crea chef
        [HttpPost]
        public async Task<IActionResult> CreaChef([FromBody] ChefDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return BadRequest("Utente non trovato.");

            var chef = new Chef
            {
                UserId = user.Id,
                Biografia = dto.Bio,
                Città = dto.Città,
                ImmagineProfilo = dto.ImmagineProfilo
            };

            _context.Chefs.Add(chef);
            await _context.SaveChangesAsync();

            dto.Id = chef.Id;
            dto.UserId = user.Id;
            return Ok(dto);
        }

        // 📌 PUT - Modifica chef
        [HttpPut("{id}")]
        public async Task<IActionResult> ModificaChef(int id, [FromBody] ChefDTO dto)
        {
            var chef = await _context.Chefs.Include(c => c.Utente).FirstOrDefaultAsync(c => c.Id == id);
            if (chef == null)
                return NotFound();

            chef.Biografia = dto.Bio;
            chef.Città = dto.Città;
            chef.ImmagineProfilo = dto.ImmagineProfilo;
            chef.Utente.Nome = dto.Nome;
            chef.Utente.Cognome = dto.Cognome;

            await _context.SaveChangesAsync();
            return Ok($"Chef {id} aggiornato");
        }

        // 📌 DELETE - Elimina chef
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminaChef(int id)
        {
            var chef = await _context.Chefs.FindAsync(id);
            if (chef == null)
                return NotFound();

            _context.Chefs.Remove(chef);
            await _context.SaveChangesAsync();
            return Ok($"Chef {id} eliminato");
        }
    }
}
