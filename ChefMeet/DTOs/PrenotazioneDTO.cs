namespace ChefMeet.Models.DTOs
{
    public class PrenotazioneDTO
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public string EventoTitolo { get; set; }
        public string UtenteNome { get; set; }
        public DateTime DataPrenotazione { get; set; }

        public string EventoImmagine { get; set; }
    }
}
