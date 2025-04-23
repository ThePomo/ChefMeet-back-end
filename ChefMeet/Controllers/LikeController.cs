using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ChefMeet.Models;
using ChefMeet.Models.DTOs;
using ChefMeet.Data;

namespace ChefMeet.Controllers
{
    [Authorize(Roles = "Chef,Utente")]
    [ApiController]
    [Route("api/[controller]")]
    public class LikeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LikeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 📌 POST - Metti un like a una creazione
        [HttpPost("{creazioneId}")]
        public async Task<IActionResult> MettiLike(int creazioneId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var esiste = await _context.Likes
                .AnyAsync(l => l.CreazioneId == creazioneId && l.UtenteId == userId);

            if (esiste)
                return BadRequest("Hai già messo like a questa creazione.");

            var like = new Like
            {
                CreazioneId = creazioneId,
                UtenteId = userId
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            return Ok("Like aggiunto.");
        }

        // 📌 DELETE - Rimuovi il like
        [HttpDelete("{creazioneId}")]
        public async Task<IActionResult> RimuoviLike(int creazioneId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.CreazioneId == creazioneId && l.UtenteId == userId);

            if (like == null)
                return NotFound("Like non trovato.");

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return Ok("Like rimosso.");
        }

        // 📌 GET - Ottieni tutti i like di un utente
        [HttpGet("utente/{utenteId}")]
        public async Task<IActionResult> GetLikeUtente(string utenteId)
        {
            var likes = await _context.Likes
                .Include(l => l.Creazione)
                .Include(l => l.Utente)
                .Where(l => l.UtenteId == utenteId)
                .Select(l => new LikeDTO
                {
                    Id = l.Id,
                    CreazioneId = l.CreazioneId,
                    UtenteNome = l.Utente.Nome + " " + l.Utente.Cognome
                })
                .ToListAsync();

            return Ok(likes);
        }

        // 📌 GET - Ottieni tutti i like per una creazione
        [HttpGet("creazione/{creazioneId}")]
        public async Task<IActionResult> GetLikePerCreazione(int creazioneId)
        {
            var likes = await _context.Likes
                .Include(l => l.Utente)
                .Where(l => l.CreazioneId == creazioneId)
                .Select(l => new LikeDTO
                {
                    Id = l.Id,
                    CreazioneId = l.CreazioneId,
                    UtenteNome = l.Utente.Nome + " " + l.Utente.Cognome
                })
                .ToListAsync();

            return Ok(likes);
        }
    }
}
