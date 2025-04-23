using Microsoft.AspNetCore.Http;

namespace ChefMeet.Models.FormModels
{
    public class CreazioneFormModel
    {
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public IFormFile Immagine { get; set; }
    }
}
