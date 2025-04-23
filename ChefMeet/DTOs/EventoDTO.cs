namespace ChefMeet.Models.DTOs
{
    public class EventoDTO
    {
        public int Id { get; set; }
        public string Titolo { get; set; }
        public string Descrizione { get; set; }
        public DateTime Data { get; set; }
        public decimal Prezzo { get; set; }
        public string Immagine { get; set; }
        public string ChefNome { get; set; }
        public string ChefUserId { get; set; }
    }
}
