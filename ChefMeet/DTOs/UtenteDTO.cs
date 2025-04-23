namespace ChefMeet.Models.DTOs
{
    public class UtenteDTO
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }
        public string Ruolo { get; set; }
        public int? ChefId { get; set; }
        public string? ImmagineProfilo { get; set; } 
    }
}
