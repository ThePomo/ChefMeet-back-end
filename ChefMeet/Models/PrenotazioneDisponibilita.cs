using System.ComponentModel.DataAnnotations;

namespace ChefMeet.Models
{
    public class PrenotazioneDisponibilita
    {
        public int Id { get; set; }

        public int DisponibilitaChefId { get; set; }
        public DisponibilitaChef DisponibilitaChef { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser Utente { get; set; }

        public DateTime DataPrenotazione { get; set; } = DateTime.Now;
    }
}
