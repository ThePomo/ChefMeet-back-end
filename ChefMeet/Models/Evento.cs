namespace ChefMeet.Models
{
    public class Evento
    {
        public int Id { get; set; }
        public string Titolo { get; set; }
        public string Descrizione { get; set; }
        public DateTime Data { get; set; }
        public decimal Prezzo { get; set; }
        public string Immagine { get; set; }

        public int ChefId { get; set; }
        public Chef Chef { get; set; }

        public List<Prenotazione> Prenotazioni { get; set; }
    }
}
