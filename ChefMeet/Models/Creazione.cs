namespace ChefMeet.Models
{
    public class Creazione
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public string Immagine { get; set; }

        public string CreatoreId { get; set; } 
        public ApplicationUser Creatore { get; set; }

        public int? ChefId { get; set; }
        public Chef? Chef { get; set; }
        public bool IsChef { get; set; }

        public List<Like> Likes { get; set; }
    }
}
