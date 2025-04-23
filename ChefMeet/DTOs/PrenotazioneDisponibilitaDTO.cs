namespace ChefMeet.DTOs
{
    public class PrenotazioneDisponibilitaDTO
    {
        public int Id { get; set; }
        public int DisponibilitaChefId { get; set; }
        public string UserId { get; set; }
        public DateTime DataPrenotazione { get; set; }
    }
}
