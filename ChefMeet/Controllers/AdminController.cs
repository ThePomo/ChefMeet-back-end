using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChefMeet.Models;
using ChefMeet.Models.DTOs;
using ChefMeet.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChefMeet.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // 📌 GET - Lista utenti
        [HttpGet("utenti")]
        public IActionResult GetUtenti()
        {
            var utenti = _userManager.Users.Select(u => new ApplicationUserDTO
            {
                Id = u.Id,
                Nome = u.Nome,
                Cognome = u.Cognome,
                Email = u.Email,
                Ruolo = u.Ruolo
            }).ToList();
            return Ok(utenti);
        }

        // 📌 GET - Singolo utente
        [HttpGet("utente/{id}")]
        public async Task<IActionResult> GetUtente(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            var dto = new ApplicationUserDTO
            {
                Id = user.Id,
                Nome = user.Nome,
                Cognome = user.Cognome,
                Email = user.Email,
                Ruolo = user.Ruolo
            };
            return Ok(dto);
        }

        // 📌 POST - Crea utente
        [HttpPost("crea-utente")]
        public async Task<IActionResult> CreaUtente([FromBody] ApplicationUserDTO dto)
        {
            var newUser = new ApplicationUser
            {
                Nome = dto.Nome,
                Cognome = dto.Cognome,
                Email = dto.Email,
                UserName = dto.Email,
                Ruolo = dto.Ruolo
            };

            var result = await _userManager.CreateAsync(newUser, "Password123!");
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(newUser, dto.Ruolo);
            dto.Id = newUser.Id;
            return Ok(dto);
        }

        [HttpPut("modifica-utente/{id}")]
        public async Task<IActionResult> ModificaUtente(string id, [FromBody] ApplicationUserDTO modifiche)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // restituisce gli errori di validazione
          

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("Utente non trovato");

            user.Nome = modifiche.Nome;
            user.Cognome = modifiche.Cognome;
            user.Email = modifiche.Email;
            user.UserName = modifiche.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors);

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(modifiche.Ruolo))
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return BadRequest(removeResult.Errors);

                var addResult = await _userManager.AddToRoleAsync(user, modifiche.Ruolo);
                if (!addResult.Succeeded)
                    return BadRequest(addResult.Errors);
            }

            user.Ruolo = modifiche.Ruolo;
            await _userManager.UpdateAsync(user);

            return Ok($"Utente {id} modificato con successo");
        }



        // 📌 DELETE - Elimina utente
        [HttpDelete("elimina-utente/{id}")]
        public async Task<IActionResult> EliminaUtente(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return NotFound($"Utente con ID {id} non trovato");

                // Elimina entità collegate
                var prenotazioni = _context.Prenotazioni.Where(p => p.UtenteId == id); var prenotazioniDisp = _context.PrenotazioniDisponibilita.Where(p => p.UserId == id);
                var prenotazioniDisponibilità = _context.PrenotazioniDisponibilita.Where(p => p.UserId == id);
                var like = _context.Likes.Where(l => l.UtenteId == id);
                var creazioni = _context.Creazioni.Where(c => c.CreatoreId == id);

                var chef = await _context.Chefs.FirstOrDefaultAsync(c => c.UserId == id);
                var eventi = chef != null
                    ? _context.Eventi.Where(e => e.ChefId == chef.Id)
                    : Enumerable.Empty<Evento>();

                var disponibilita = chef != null
                    ? _context.DisponibilitaChef.Where(d => d.ChefId == chef.Id)
                    : Enumerable.Empty<DisponibilitaChef>();

                _context.Prenotazioni.RemoveRange(prenotazioni);
                _context.PrenotazioniDisponibilita.RemoveRange(prenotazioniDisp);
                _context.Likes.RemoveRange(like);
                _context.Creazioni.RemoveRange(creazioni);
                _context.Eventi.RemoveRange(eventi);
                _context.DisponibilitaChef.RemoveRange(disponibilita);

                if (chef != null)
                    _context.Chefs.Remove(chef);

                await _context.SaveChangesAsync();

                // Rimuove ruoli e l'utente
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Any())
                    await _userManager.RemoveFromRolesAsync(user, userRoles);

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    return BadRequest($"Errore durante l'eliminazione: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                return Ok($"Utente {id} eliminato con successo");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Errore di database: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore generico: {ex.Message}");
            }
        }
    }
}
