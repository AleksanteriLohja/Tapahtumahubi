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



    public bool Validate(out List<string> errors)
    {
        errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Title))
            errors.Add("Otsikko on pakollinen");
        else if (Title.Length > 200)
            errors.Add("Otsikon enimmäispituus on 200 merkkiä");

        if (Location?.Length > 200)
            errors.Add("Sijainnin enimmäispituus on 200 merkkiä");

        if (MaxParticipants < 1)
            errors.Add("Osallistujien lukumäärän on oltava vähintään 1");

        return errors.Count == 0;
    }
}
