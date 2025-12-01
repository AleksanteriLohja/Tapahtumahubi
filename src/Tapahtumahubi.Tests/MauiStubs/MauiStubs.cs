#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Maui.Controls
{
    public class Page
    {
        public virtual Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
            => Task.FromResult(true);

        public virtual Task DisplayAlert(string title, string message, string cancel)
            => Task.CompletedTask;
    }

    public class Application
    {
        public static Application Current { get; set; } = new Application();
        public Page MainPage { get; set; } = new Page();
    }

    public class Shell
    {
        public static Shell Current { get; set; } = new Shell();

        public virtual Task GoToAsync(string route) => Task.CompletedTask;

        public virtual Task GoToAsync(string route, IDictionary<string, object> parameters)
            => Task.CompletedTask;
    }
}

namespace Tapahtumahubi.App
{
    public class NewEventPage { }
    public class ParticipantsPage { }
    public class AddEditParticipantPage { }
    public class CalendarPage { }
}