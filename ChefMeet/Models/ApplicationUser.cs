using Microsoft.AspNetCore.Identity;

namespace ChefMeet.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Ruolo { get; set; }
        public string? ImmagineProfilo { get; set; }
        public Chef Chef { get; set; } 
    }
}
