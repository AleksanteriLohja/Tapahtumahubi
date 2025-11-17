// src/Tapahtumahubi.App/NewEventPage.xaml.cs
using Microsoft.EntityFrameworkCore;
using Tapahtumahubi.Domain;
using Tapahtumahubi.Infrastructure;

namespace Tapahtumahubi.App;

[QueryProperty(nameof(EventId), "EventId")]
public partial class NewEventPage : ContentPage
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    // Navigaation välittämä tunniste. Kun arvo asetetaan, yritetään ladata olemassa oleva.
    private int _eventId;
    public int EventId
    {
        get => _eventId;
        set
        {
            _eventId = value;
            _ = LoadIfEditing(); // käynnistä lataus taustalla
        }
    }

    // Pitää viitteen ladattuun entiteettiin muokkausta varten
    private Event? _loaded;

    public NewEventPage(IDbContextFactory<AppDbContext> dbFactory)
    {
        InitializeComponent();
        _dbFactory = dbFactory;

        DatePicker.Date = DateTime.Today;
        TimePicker.Time = new TimeSpan(18, 0, 0);
    }

    private async Task LoadIfEditing()
    {
        if (EventId <= 0) return;

        using var db = await _dbFactory.CreateDbContextAsync();
        _loaded = await db.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == EventId);
        if (_loaded == null)
        {
            await DisplayAlert("Huom", "Muokattavaa tapahtumaa ei löytynyt.", "OK");
            return;
        }

        // Päivitä UI muokkaustilaan
        HeaderLabel.Text = "Muokkaa tapahtumaa";
        TitleEntry.Text = _loaded.Title;
        LocationEntry.Text = _loaded.Location;
        DatePicker.Date = _loaded.StartTime.Date;
        TimePicker.Time = _loaded.StartTime.TimeOfDay;
        DescEditor.Text = _loaded.Description ?? "";
        MaxEntry.Text = _loaded.MaxParticipants.ToString();
    }

    private async void OnSave(object sender, EventArgs e)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
            validationErrors.Add("Otsikko on pakollinen");
        else if (TitleEntry.Text.Length > 200)
            validationErrors.Add("Otsikon enimmäispituus on 200 merkkiä");

        if (LocationEntry.Text?.Length > 200)
            validationErrors.Add("Sijainnin enimmäispituus on 200 merkkiä");

        if (!int.TryParse(MaxEntry.Text, out int max) || max < 1)
            validationErrors.Add("Osallistujien lukumäärän on oltava vähintään 1");

        if (validationErrors.Any())
        {
            await DisplayAlert("Virhe", string.Join("\n", validationErrors), "OK");
            return;
        }

        var start = DatePicker.Date + TimePicker.Time;

        using var db = await _dbFactory.CreateDbContextAsync();

        if (_loaded is null && EventId <= 0)
        {
            // LISÄYS
            var ev = new Event
            {
                Title = TitleEntry.Text!.Trim(),
                Location = (LocationEntry.Text ?? "").Trim(),
                StartTime = start,
                Description = string.IsNullOrWhiteSpace(DescEditor.Text) ? null : DescEditor.Text.Trim(),
                MaxParticipants = max
            };

            db.Events.Add(ev);
            await db.SaveChangesAsync();
            await DisplayAlert("OK", "Tapahtuma tallennettu.", "OK");
        }
        else
        {
            // MUOKKAUS
            var toUpdate = await db.Events.FirstOrDefaultAsync(x => x.Id == EventId);
            if (toUpdate is null)
            {
                await DisplayAlert("Virhe", "Tapahtumaa ei löytynyt muokattavaksi.", "OK");
                return;
            }

            toUpdate.Title = TitleEntry.Text!.Trim();
            toUpdate.Location = (LocationEntry.Text ?? "").Trim();
            toUpdate.StartTime = start;
            toUpdate.Description = string.IsNullOrWhiteSpace(DescEditor.Text) ? null : DescEditor.Text.Trim();
            toUpdate.MaxParticipants = max;

            await db.SaveChangesAsync();
            await DisplayAlert("OK", "Muutokset tallennettu.", "OK");
        }

        // Palaa listaan
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancel(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
