#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Maui.Controls
{
    // Minimi, jotta Application.Current?.MainPage?.DisplayAlert(...) toimii
    public class Page
    {
        public virtual Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
            => Task.FromResult(true);
    }

    public class Application
    {
        // Ei-nullable: poistaa CS8602-varoitukset
        public static Application Current { get; set; } = new Application();
        public Page MainPage { get; set; } = new Page();
    }

    public class Shell
    {
        // Ei-nullable: poistaa CS8602-varoitukset
        public static Shell Current { get; set; } = new Shell();

        public virtual Task GoToAsync(string route) => Task.CompletedTask;

        public virtual Task GoToAsync(string route, IDictionary<string, object> parameters)
            => Task.CompletedTask;

        // NewEventPageViewModel kutsuu tätä
        public virtual Task DisplayAlert(string title, string message, string cancel)
            => Task.CompletedTask;
    }
}

// Sivustubit, jotta nameof(NewEventPage) / nameof(ParticipantsPage) kääntyy
namespace Tapahtumahubi.App
{
    public class NewEventPage { }
    public class ParticipantsPage { }
}