using System.ComponentModel.DataAnnotations;

namespace ChefMeet.Models.DTOs
{
    public class ApplicationUserDTO
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Il campo Nome è obbligatorio")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Il campo Cognome è obbligatorio")]
        public string Cognome { get; set; }

        [Required(ErrorMessage = "Il campo Email è obbligatorio")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Il campo Ruolo è obbligatorio")]
        public string Ruolo { get; set; }
    }
}
