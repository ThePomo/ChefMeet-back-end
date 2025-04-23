namespace ChefMeet.Models
{
    public class Like
    {
        public int Id { get; set; }

        public string UtenteId { get; set; }
        public ApplicationUser Utente { get; set; }

        public int CreazioneId { get; set; }
        public Creazione Creazione { get; set; }
    }
}
