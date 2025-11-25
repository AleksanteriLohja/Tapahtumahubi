// src/Tapahtumahubi.App/AppShell.xaml.cs
using Microsoft.Maui.Controls;

namespace Tapahtumahubi.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Reitti muokkaus-/luontisivulle
            Routing.RegisterRoute(nameof(NewEventPage), typeof(NewEventPage));
            Routing.RegisterRoute(nameof(ParticipantsPage), typeof(ParticipantsPage));
            Routing.RegisterRoute(nameof(AddEditParticipantPage), typeof(AddEditParticipantPage));

        }
    }
}
