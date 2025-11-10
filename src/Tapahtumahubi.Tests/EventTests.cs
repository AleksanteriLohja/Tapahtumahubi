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
            Description = "Kuvaus"
        };

        Assert.Equal("Testi", ev.Title);
        Assert.Equal("Paikka", ev.Location);
        Assert.Equal("Kuvaus", ev.Description);
    }
}