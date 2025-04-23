using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChefMeet.Models;
using ChefMeet.Models.DTOs;
using ChefMeet.Data;
using Microsoft.AspNetCore.Identity;

namespace ChefMeet.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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

        // 📌 PUT - Modifica utente
        [HttpPut("modifica-utente/{id}")]
        public async Task<IActionResult> ModificaUtente(string id, [FromBody] ApplicationUserDTO modifiche)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Nome = modifiche.Nome;
            user.Cognome = modifiche.Cognome;
            user.Email = modifiche.Email;
            user.UserName = modifiche.Email;
            user.Ruolo = modifiche.Ruolo;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"Utente {id} modificato");
        }

        // 📌 DELETE - Elimina utente
        [HttpDelete("elimina-utente/{id}")]
        public async Task<IActionResult> EliminaUtente(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"Utente {id} eliminato");
        }
    }
}
