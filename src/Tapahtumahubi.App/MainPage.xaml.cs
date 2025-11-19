using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using Tapahtumahubi.Domain;
using Tapahtumahubi.Infrastructure;

// Alias selkeyttämään osallistujanäkymän navigoinnin parametreja
using EventEntity = Tapahtumahubi.Domain.Event;

namespace Tapahtumahubi.App;

public partial class MainPage : ContentPage
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private ObservableCollection<Event> _items = new();

    public MainPage(IDbContextFactory<AppDbContext> dbFactory)
    {
        InitializeComponent();
        _dbFactory = dbFactory;
        EventsList.ItemsSource = _items;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();
    }

    private async Task LoadData()
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var eventsFromDb = await db.Events
            .AsNoTracking()
            .OrderBy(e => e.StartTime)
            .ToListAsync();

        _items = new ObservableCollection<Event>(eventsFromDb);
        _items.CollectionChanged += async (s, e) =>
        {
            await AnimateButtonState(DeleteAllButton, _items.Count > 0);
        };
        EventsList.ItemsSource = _items;
        DeleteAllButton.IsEnabled = _items.Count > 0;
    }

    private void OnSearchChanged(object sender, TextChangedEventArgs e)
    {
        var q = e.NewTextValue?.Trim().ToLowerInvariant() ?? "";
        if (string.IsNullOrEmpty(q))
        {
            EventsList.ItemsSource = _items;
            return;
        }

        EventsList.ItemsSource = _items.Where(x =>
            (x.Title ?? "").ToLower().Contains(q) ||
            (x.Location ?? "").ToLower().Contains(q));
    }

    private async void OnNewEvent(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NewEventPage));
    }

    // Napautus = muokkaus
    private async void OnItemTapped(object? sender, TappedEventArgs e)
    {
        var ev = e.Parameter as Event
                 ?? (sender as BindableObject)?.BindingContext as Event;
        if (ev is null) return;

        await Shell.Current.GoToAsync(nameof(NewEventPage), new Dictionary<string, object>
        {
            ["EventId"] = ev.Id
        });
    }

    // Muokkaa-nappi
    private async void OnEdit(object sender, EventArgs e)
    {
        var ev = (sender as Button)?.CommandParameter as Event
                 ?? (sender as BindableObject)?.BindingContext as Event;
        if (ev is null)
        {
            await DisplayAlert("Virhe", "Rivin tietoa ei saatu.", "OK");
            return;
        }

        await Shell.Current.GoToAsync(nameof(NewEventPage), new Dictionary<string, object>
        {
            ["EventId"] = ev.Id
        });
    }

    // Osallistujat-nappi (UUSI)
    private async void OnOpenParticipants(object sender, EventArgs e)
    {
        if (sender is Button b && b.CommandParameter is EventEntity ev)
        {
            await Shell.Current.GoToAsync(nameof(ParticipantsPage), new Dictionary<string, object>
            {
                { "eventId", ev.Id },
                { "eventTitle", ev.Title },
                { "maxParticipants", ev.MaxParticipants }
            });
        }
    }

    // Poista-nappi
    private async void OnDelete(object sender, EventArgs e)
    {
        var ev = (sender as Button)?.CommandParameter as Event
                 ?? (sender as BindableObject)?.BindingContext as Event;
        if (ev is null)
        {
            await DisplayAlert("Virhe", "Rivin tietoa ei saatu.", "OK");
            return;
        }

        var ok = await DisplayAlert("Poista", $"Poistetaanko \"{ev.Title}\"?", "Poista", "Peruuta");
        if (!ok) return;

        try
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            var toRemove = await db.Events.FirstOrDefaultAsync(x => x.Id == ev.Id);
            if (toRemove is null)
            {
                await DisplayAlert("Huom", "Tapahtumaa ei löytynyt (saatettiin jo poistaa).", "OK");
                return;
            }

            db.Events.Remove(toRemove);
            await db.SaveChangesAsync();

            var itemInList = _items.FirstOrDefault(x => x.Id == ev.Id);
            if (itemInList != null)
                _items.Remove(itemInList);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Poisto epäonnistui: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteAll(object sender, EventArgs e)
    {
        bool ok = await DisplayAlert(
            "Poista kaikki",
            "Haluatko varmasti poistaa KAIKKI tapahtumat?",
            "Poista kaikki",
            "Peruuta");

        if (!ok)
            return;

        try
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            db.Events.RemoveRange(db.Events);
            await db.SaveChangesAsync();
            _items.Clear();
            await DisplayAlert("OK", "Kaikki tapahtumat poistettu.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Poisto epäonnistui: {ex.Message}", "OK");
        }
    }

    private async Task AnimateButtonState(Button btn, bool isEnabled)
    {
        double targetOpacity = isEnabled ? 1.0 : 0.4;
        await btn.FadeTo(targetOpacity, 250, Easing.CubicOut);
        btn.IsEnabled = isEnabled;
    }
}