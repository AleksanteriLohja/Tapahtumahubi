using Tapahtumahubi.Domain;
using Xunit;
namespace Tapahtumahubi.Tests;

public class ParticipantTest
{
    [Fact]
    public void Valid_Data_Passes_Validation() {
        var participant = new Participant
        {
            Name = "name",
            Email="test@email.com"
        };
        var isvalid = participant.Validate(out var errors);
        Assert.True(isvalid);
        Assert.Empty(errors);
    }


    [Fact]
    public void Invalid_Email_Fails_Validation()
    {
        var participant = new Participant
        {
            Name = "name",
            Email = "invalidEmail"
        };
        var isvalid = participant.Validate(out var errors);
            Assert.False(isvalid);
        Assert.Contains("Sähköpostin muoto on virheellinen", errors);
    }

    [Fact]
    public void Too_Long_Name_Fails_Validation() {
        var participant = new Participant { 
            Name= new string ('A',201),
            Email= "test@email.com"
        };
        var isvalid = participant.Validate(out var errors);
        Assert.False(isvalid);
        Assert.Contains("Nimi saa olla enintään 200 merkkiä", errors);
    }


    [Fact]
    public void Empty_Name_Fails_Validation()
    {
        var participant = new Participant
        {
            Name = "",
            Email = "test@email.com"
        };
        var isvalid = participant.Validate(out var errors);
        Assert.False(isvalid);
        Assert.Contains("Nimi on pakollinen", errors);
    }

    [Fact]
    public void Empty_Email_Fails_Validation()
    {
        var participant = new Participant
        {
            Name = "name",
            Email = ""
        };
        var isvalid = participant.Validate(out var errors);
        Assert.False(isvalid);
        Assert.Contains("Sähköposti on pakollinen", errors);
    }

}
