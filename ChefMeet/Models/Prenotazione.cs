namespace ChefMeet.Models
{
    public class Prenotazione
    {
        public int Id { get; set; }

        public string UtenteId { get; set; }
        public ApplicationUser Utente { get; set; }

        public int EventoId { get; set; }
        public Evento Evento { get; set; }

        public DateTime DataPrenotazione { get; set; }
    }
}
