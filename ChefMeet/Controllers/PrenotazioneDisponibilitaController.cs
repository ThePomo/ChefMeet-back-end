using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ChefMeet.Data;
using ChefMeet.Models;
using ChefMeet.DTOs;

namespace ChefMeet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrenotazioneDisponibilitaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PrenotazioneDisponibilitaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📌 POST - Crea una nuova prenotazione
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreaPrenotazione([FromBody] PrenotazioneDisponibilitaDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var disponibilita = await _context.DisponibilitaChef.FindAsync(dto.DisponibilitaChefId);
            if (disponibilita == null || !disponibilita.ÈDisponibile)
                return BadRequest("La disponibilità non è valida o è già stata prenotata.");

            disponibilita.ÈDisponibile = false;

            var prenotazione = new PrenotazioneDisponibilita
            {
                UserId = userId,
                DisponibilitaChefId = dto.DisponibilitaChefId,
                DataPrenotazione = DateTime.Now
            };

            _context.PrenotazioniDisponibilita.Add(prenotazione);
            await _context.SaveChangesAsync();

            return Ok("Prenotazione effettuata.");
        }

        // 📌 GET - Visualizza le prenotazioni dell'utente loggato
        [HttpGet("mie")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMiePrenotazioni()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var prenotazioni = await _context.PrenotazioniDisponibilita
                .Include(p => p.DisponibilitaChef)
                .ThenInclude(d => d.Chef)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return Ok(prenotazioni);
        }

        // 📌 GET - Prenotazioni ricevute dallo chef loggato
        [HttpGet("ricevute")]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> GetPrenotazioniRicevute()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chef = await _context.Chefs.FirstOrDefaultAsync(c => c.UserId == userId);

            if (chef == null)
                return BadRequest("Chef non trovato.");

            var prenotazioni = await _context.PrenotazioniDisponibilita
                .Include(p => p.DisponibilitaChef)
                .Where(p => p.DisponibilitaChef.ChefId == chef.Id)
                .ToListAsync();

            return Ok(prenotazioni);
        }

        // 📌 PUT - Modifica prenotazione (es. per aggiornamenti futuri)
        [HttpPut("{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ModificaPrenotazione(int id, [FromBody] PrenotazioneDisponibilitaDTO dto)
        {
            var prenotazione = await _context.PrenotazioniDisponibilita.FindAsync(id);
            if (prenotazione == null)
                return NotFound();

            prenotazione.DisponibilitaChefId = dto.DisponibilitaChefId;

            await _context.SaveChangesAsync();
            return Ok("Prenotazione aggiornata.");
        }

        // 📌 DELETE - Elimina prenotazione
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> EliminaPrenotazione(int id)
        {
            var prenotazione = await _context.PrenotazioniDisponibilita.FindAsync(id);
            if (prenotazione == null)
                return NotFound();

            var disponibilita = await _context.DisponibilitaChef.FindAsync(prenotazione.DisponibilitaChefId);
            if (disponibilita != null)
                disponibilita.ÈDisponibile = true;

            _context.PrenotazioniDisponibilita.Remove(prenotazione);
            await _context.SaveChangesAsync();

            return Ok("Prenotazione eliminata.");
        }
    }
}
