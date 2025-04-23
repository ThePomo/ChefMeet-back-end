using Microsoft.AspNetCore.Http;

namespace ChefMeet.Models.FormModels
{
    public class EventoFormModel
    {
        public string Titolo { get; set; }
        public string Descrizione { get; set; }
        public DateTime Data { get; set; }
        public decimal Prezzo { get; set; }
        public IFormFile? Immagine { get; set; }
    }
}
