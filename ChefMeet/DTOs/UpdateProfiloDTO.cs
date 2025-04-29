namespace ChefMeet.DTOs
{
    public class UpdateProfiloDTO
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
    
        public string? Email { get; set; }
        
        public string? Bio { get; set; }
        public string? Citta { get; set; }
    }
}
