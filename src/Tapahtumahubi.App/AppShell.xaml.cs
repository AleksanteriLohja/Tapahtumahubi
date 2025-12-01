// src/Tapahtumahubi.App/AppShell.xaml.cs
using Microsoft.Maui.Controls;

namespace Tapahtumahubi.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // --- Reitit sivuille, joihin navigoidaan push-tyylisesti ---
            Routing.RegisterRoute(nameof(NewEventPage), typeof(NewEventPage));
            Routing.RegisterRoute(nameof(ParticipantsPage), typeof(ParticipantsPage));
            Routing.RegisterRoute(nameof(AddEditParticipantPage), typeof(AddEditParticipantPage));

            // CalendarPage on TabBarissa nimettynä sisällöksi, joten reititys ei ole pakollinen.
            // Jos haluat silti mahdollistaa GoToAsync(nameof(CalendarPage)), pidä tämä mukana:
            Routing.RegisterRoute(nameof(CalendarPage), typeof(CalendarPage));
        }
    }
}