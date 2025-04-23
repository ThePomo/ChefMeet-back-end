namespace ChefMeet.Models.DTOs
{
    public class CreazioneDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public string Immagine { get; set; }
        public string Autore { get; set; }
        public bool IsChef { get; set; }
        public int NumeroLike { get; set; } 
    }
}

