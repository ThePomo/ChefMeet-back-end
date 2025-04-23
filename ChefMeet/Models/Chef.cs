namespace ChefMeet.Models
{
    public class Chef
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string Biografia { get; set; }
        public string Città { get; set; }
        public string ImmagineProfilo { get; set; }

        public ApplicationUser Utente { get; set; }

        public List<Creazione> Creazioni { get; set; }
        public List<Evento> Eventi { get; set; }
    }
}
