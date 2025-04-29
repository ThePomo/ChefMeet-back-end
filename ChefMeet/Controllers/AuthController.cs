using ChefMeet.DTOs;
using ChefMeet.Helpers;
using ChefMeet.Interfaces;
using ChefMeet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace ChefMeet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _authService = authService;
            _userManager = userManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto, IFormFile? immagineProfilo)
        {
            // ✅ Controllo validazione lato server
            if (!ModelState.IsValid)
            {
                var errori = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { errore = $"Errore nei dati forniti: {string.Join(" | ", errori)}" });
            }
            string? imagePath = null;
            try
            {
                if (immagineProfilo != null && immagineProfilo.Length > 0)
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "utenti");
                    Directory.CreateDirectory(folderPath);
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(immagineProfilo.FileName);
                    var filePath = Path.Combine(folderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await immagineProfilo.CopyToAsync(stream);
                    }
                    imagePath = $"/uploads/utenti/{fileName}";
                }
                var result = await _authService.RegisterAsync(dto, imagePath);
                if (result == "Registrazione completata.")
                    return Ok(new { messaggio = result });
                return BadRequest(new { errore = result });
            }
            catch (Exception ex)
            {
                // ✅ Log dell'errore per il debug
                Console.WriteLine("Errore durante la registrazione: " + ex.Message);
                return StatusCode(500, new { errore = "Errore interno durante la registrazione." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                Console.WriteLine($"Tentativo di login per: {dto.Email}");

                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    Console.WriteLine("Utente non trovato");
                    return Unauthorized(new { errore = "Credenziali non valide." });
                }

                Console.WriteLine($"Utente trovato con ID: {user.Id}");
                var passwordValida = await _userManager.CheckPasswordAsync(user, dto.Password);
                Console.WriteLine($"Verifica password: {passwordValida}");

                if (!passwordValida)
                    return Unauthorized(new { errore = "Password errata." });

                var ruoli = await _userManager.GetRolesAsync(user);
                var token = JwtHelper.GenerateJwtToken(user, _config, ruoli);
                var userInfo = new UserInfoDTO
                {
                    Id = user.Id,
                    Nome = user.Nome,
                    Cognome = user.Cognome,
                    Email = user.Email,
                    Ruolo = user.Ruolo
                };
                return Ok(new { token, user = userInfo });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il login: {ex.Message}");
                return StatusCode(500, new { errore = "Errore interno durante il login." });
            }
        }

        [HttpPost("reset-admin")]
        public async Task<IActionResult> ResetAdmin()
        {
            try
            {
                var adminEmail = "admin@chefmeet.com";
                var nuovaPassword = "Admin123!"; // Scegli una password sicura

                var admin = await _userManager.FindByEmailAsync(adminEmail);
                if (admin == null)
                    return NotFound(new { errore = "Admin non trovato" });

                // Genera un token di reset
                var token = await _userManager.GeneratePasswordResetTokenAsync(admin);

                // Imposta una nuova password
                var result = await _userManager.ResetPasswordAsync(admin, token, nuovaPassword);

                if (result.Succeeded)
                    return Ok(new { messaggio = $"Password admin reimpostata a: {nuovaPassword}" });
                else
                    return BadRequest(new { errore = $"Errore nella reimpostazione: {string.Join(", ", result.Errors.Select(e => e.Description))}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il reset della password admin: {ex.Message}");
                return StatusCode(500, new { errore = "Errore interno durante il reset della password." });
            }
        }
    }
}