using System.Text.Json.Serialization;

namespace ChefMeet.DTOs
{
    public class RegisterDto
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Ruolo { get; set; }

        public string? Bio { get; set; }

        [JsonPropertyName("città")]
        public string? Citta { get; set; } 
    }
}
