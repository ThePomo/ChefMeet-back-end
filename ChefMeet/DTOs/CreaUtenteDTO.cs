using System.ComponentModel.DataAnnotations;

namespace ChefMeet.Models.DTOs
{
    public class CreaUtenteDTO
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public string Cognome { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string Ruolo { get; set; }
    }
}
