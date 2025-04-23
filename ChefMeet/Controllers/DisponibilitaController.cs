using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ChefMeet.Data;
using ChefMeet.Models;
using ChefMeet.Models.DTOs;

namespace ChefMeet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisponibilitaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DisponibilitaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📌 GET - Disponibilità future dello chef loggato
        [HttpGet]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> GetMieDisponibilita()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chef = await _context.Chefs.FirstOrDefaultAsync(c => c.UserId == userId);
            if (chef == null) return NotFound("Chef non trovato.");

            var oggi = DateTime.Today;

            var disponibilita = await _context.DisponibilitaChef
                .Where(d => d.ChefId == chef.Id && d.Data >= oggi)
                .Select(d => new DisponibilitaDTO
                {
                    Data = d.Data,
                    OraInizio = d.OraInizio,
                    OraFine = d.OraFine,
                    ÈDisponibile = d.ÈDisponibile
                })
                .ToListAsync();

            return Ok(disponibilita);
        }

        // 📌 POST - Aggiungi disponibilità con controllo sovrapposizione
        [HttpPost]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> AggiungiDisponibilita(DisponibilitaDTO dto)

        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chef = await _context.Chefs.FirstOrDefaultAsync(c => c.UserId == userId);
            if (chef == null) return NotFound("Chef non trovato.");

            // Verifica sovrapposizione orari
            var sovrapposta = await _context.DisponibilitaChef.AnyAsync(d =>
                d.ChefId == chef.Id &&
                d.Data == dto.Data &&
                d.OraInizio < dto.OraFine &&
                dto.OraInizio < d.OraFine);

            if (sovrapposta)
                return BadRequest("Hai già una disponibilità sovrapposta per questo orario.");

            var disponibilita = new DisponibilitaChef
            {
                ChefId = chef.Id,
                Data = dto.Data,
                OraInizio = dto.OraInizio,
                OraFine = dto.OraFine,
                ÈDisponibile = dto.ÈDisponibile
            };

            _context.DisponibilitaChef.Add(disponibilita);
            await _context.SaveChangesAsync();

            return Ok("Disponibilità aggiunta.");
        }

        // 📌 PUT - Modifica disponibilità
        [HttpPut("{id}")]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> ModificaDisponibilita(int id, [FromBody] DisponibilitaDTO dto)
        {
            var disponibilita = await _context.DisponibilitaChef.FindAsync(id);
            if (disponibilita == null) return NotFound();

            // Verifica sovrapposizione con altri slot (escludendo sé stesso)
            var chefId = disponibilita.ChefId;
            var sovrapposta = await _context.DisponibilitaChef.AnyAsync(d =>
                d.Id != id &&
                d.ChefId == chefId &&
                d.Data == dto.Data &&
                d.OraInizio < dto.OraFine &&
                dto.OraInizio < d.OraFine);

            if (sovrapposta)
                return BadRequest("Esiste già una disponibilità sovrapposta.");

            disponibilita.Data = dto.Data;
            disponibilita.OraInizio = dto.OraInizio;
            disponibilita.OraFine = dto.OraFine;
            disponibilita.ÈDisponibile = dto.ÈDisponibile;

            await _context.SaveChangesAsync();
            return Ok("Disponibilità aggiornata.");
        }

        // 📌 DELETE - Elimina disponibilità
        [HttpDelete("{id}")]
        [Authorize(Roles = "Chef")]
        public async Task<IActionResult> EliminaDisponibilita(int id)
        {
            var disponibilita = await _context.DisponibilitaChef.FindAsync(id);
            if (disponibilita == null)
                return NotFound();

            _context.DisponibilitaChef.Remove(disponibilita);
            await _context.SaveChangesAsync();

            return Ok("Disponibilità eliminata.");
        }

        // 📌 GET - Disponibilità pubblica di uno chef (visibile agli utenti)
        [HttpGet("pubblica/{chefId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDisponibilitaPubblica(int chefId)
        {
            var oggi = DateTime.Today;

            var disponibilita = await _context.DisponibilitaChef
                .Where(d => d.ChefId == chefId && d.Data >= oggi && d.ÈDisponibile)
                .Select(d => new DisponibilitaDTO
                {
                    Data = d.Data,
                    OraInizio = d.OraInizio,
                    OraFine = d.OraFine,
                    ÈDisponibile = d.ÈDisponibile
                })
                .ToListAsync();

            return Ok(disponibilita);
        }
    }
}
