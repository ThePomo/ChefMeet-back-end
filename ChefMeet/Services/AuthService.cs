using ChefMeet.DTOs;
using ChefMeet.Helpers;
using ChefMeet.Interfaces;
using ChefMeet.Models;
using ChefMeet.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ChefMeet.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _context = context;
        }

        public async Task<string> RegisterAsync(RegisterDto dto, string? immagineProfiloPath)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return "Utente già registrato con questa email.";

            var newUser = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Nome = dto.Nome,
                Cognome = dto.Cognome,
                Ruolo = dto.Ruolo,
                ImmagineProfilo = immagineProfiloPath
            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);

            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                return $"Errore nella creazione utente: {errorMessages}";
            }

            if (!await _roleManager.RoleExistsAsync(dto.Ruolo))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(dto.Ruolo));
                if (!roleResult.Succeeded)
                    return "Impossibile creare il ruolo.";
            }

            await _userManager.AddToRoleAsync(newUser, dto.Ruolo);

            // ✅ Se ruolo = Chef, crea anche il profilo Chef
            if (dto.Ruolo == "Chef")
            {
                var nuovoChef = new Chef
                {
                    UserId = newUser.Id,
                    Biografia = dto.Bio ?? "",
                    Città = dto.Citta ?? "",
                    ImmagineProfilo = immagineProfiloPath // ✅ anche nel profilo Chef
                };

                _context.Chefs.Add(nuovoChef);
                await _context.SaveChangesAsync();
            }

            return "Registrazione completata.";
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return null;

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
                return null;

            // ✅ Ottieni i ruoli dell'utente
            var ruoli = await _userManager.GetRolesAsync(user);

            // ✅ Passali al metodo del JWT
            return JwtHelper.GenerateJwtToken(user, _config, ruoli);
        }
    }
}