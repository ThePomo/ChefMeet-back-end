namespace ChefMeet.Models.DTOs
{
    public class ChefDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string Città { get; set; }
        public string? ImmagineProfilo { get; set; }

        public string UserId { get; set; }  

    }
}
