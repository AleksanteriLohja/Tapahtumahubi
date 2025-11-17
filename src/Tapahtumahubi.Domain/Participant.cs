namespace Tapahtumahubi.Domain;

public class Participant
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public int EventId { get; set; }


    public bool Validate(out List<string> errors)
    {
        errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Nimi on pakollinen");
        else if (Name.Length > 200)
            errors.Add("Nimi saa olla enintään 200 merkkiä");

        if (string.IsNullOrWhiteSpace(Email))
            errors.Add("Sähköposti on pakollinen");
        else if (!IsValidEmail(Email))
            errors.Add("Sähköpostin muoto on virheellinen");

        return errors.Count == 0;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
