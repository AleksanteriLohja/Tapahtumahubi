namespace Tapahtumahubi.Domain;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime StartTime { get; set; } = DateTime.Now;
    public string Location { get; set; } = "";
    public string? Description { get; set; }
    public int MaxParticipants { get; set; } = 50;

    public List<Participant> Participants { get; set; } = new();
}