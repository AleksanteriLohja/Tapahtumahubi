using Tapahtumahubi.Domain;
using Xunit;

namespace Tapahtumahubi.Tests;

public class EventTests
{
    [Fact]
    public void Default_MaxParticipants_Is_50()
    {
        var ev = new Event();
        Assert.Equal(50, ev.MaxParticipants);
    }

    [Fact]
    public void Can_Set_Basic_Fields()
    {
        var ev = new Event

        {
            Title = "Testi",
            Location = "Paikka",
            Description = "Kuvaus",
            MaxParticipants = 10
        };

        var isValid = ev.Validate(out var errors);

        Assert.True(isValid);
        Assert.Empty(errors);
    }
    [Fact]
    public void Empty_Title_Fails_Validation()
    {
        var ev = new Event { Title = "" };
        var isValid = ev.Validate(out var errors);
        Assert.False(isValid);

        Assert.Contains("Otsikko on pakollinen", errors);
    }
    [Fact]
    public void Too_Long_Title_Fails_Validation()
    {
        var ev = new Event { Title = new string('A', 201) };
        var isValid = ev.Validate(out var errors);
        Assert.False(isValid);
        Assert.Contains("Otsikon enimmäispituus on 200 merkkiä", errors);
    }
    [Fact]
    public void Too_Long_Location_Fails_Validation()
    {
        var ev = new Event { Location = new string('B', 201) };
        var isValid = ev.Validate(out var errors);
        Assert.False(isValid);
        Assert.Contains("Sijainnin enimmäispituus on 200 merkkiä", errors);
    }

    [Fact]
    public void Invalid_MaxParticipants_Fails_Validation()
    {
        var ev = new Event { MaxParticipants = 0 };
        var isValid = ev.Validate(out var errors);
        Assert.False(isValid);
        Assert.Contains("Osallistujien lukumäärän on oltava vähintään 1", errors);
    }


}
