using ChefMeet.Models;

public class DisponibilitaChef
{
    public int Id { get; set; }

    public int ChefId { get; set; }
    public Chef Chef { get; set; }

    public DateTime Data { get; set; }
    public TimeSpan OraInizio { get; set; }
    public TimeSpan OraFine { get; set; }

    public bool ÈDisponibile { get; set; } = true;
}
