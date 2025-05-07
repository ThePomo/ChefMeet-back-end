using Microsoft.AspNetCore.Http;

namespace ChefMeet.Models
{
    public class ChefFormModel
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Bio { get; set; }
        public string Città { get; set; }
        public IFormFile? ImmagineProfilo { get; set; }
    }
}
