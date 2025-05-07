using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChefMeet.Data;
using ChefMeet.Models;
using ChefMeet.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ChefMeet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChefController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChefController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔍 Ottieni chef per ID
        [HttpGet("{id}")]
        [Authorize]
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
                UserId = chef.UserId
            };

            return Ok(dto);
        }

        // 🔍 Ottieni chef per UserId
        [HttpGet("byUser/{userId}")]
        [Authorize(Roles = "Chef,Utente,Admin")]
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
                UserId = chef.UserId
            };

            return Ok(dto);
        }

        // 🆕 Crea nuovo profilo Chef
        [HttpPost]
        [Authorize(Roles = "Chef")]
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

        // ✏️ Modifica profilo Chef con supporto upload immagine
        [HttpPut("{id}")]
        [Authorize(Roles = "Chef")]
        [Consumes("multipart/form-data")]
        [ApiExplorerSettings(IgnoreApi = true)] 
        public async Task<IActionResult> ModificaChef(int id, [FromForm] ChefFormModel form)
        {
            var chef = await _context.Chefs.Include(c => c.Utente).FirstOrDefaultAsync(c => c.Id == id);
            if (chef == null)
                return NotFound();

            chef.Utente.Nome = form.Nome;
            chef.Utente.Cognome = form.Cognome;
            chef.Biografia = form.Bio;
            chef.Città = form.Città;

            if (form.ImmagineProfilo != null && form.ImmagineProfilo.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(form.ImmagineProfilo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await form.ImmagineProfilo.CopyToAsync(stream);
                }

                chef.ImmagineProfilo = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync();
            return Ok($"Chef {id} aggiornato con successo.");
        }


        // ❌ Elimina profilo Chef
        [HttpDelete("{id}")]
        [Authorize(Roles = "Chef")]
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
