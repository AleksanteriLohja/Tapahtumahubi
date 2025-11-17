using Microsoft.EntityFrameworkCore;
using Tapahtumahubi.Infrastructure;
using Tapahtumahubi.Domain;

namespace Tapahtumahubi.App;

[QueryProperty(nameof(EventId), "id")]
public partial class EditEventPage : ContentPage
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private int _eventId;
    private Event? _model;

    public EditEventPage(IDbContextFactory<AppDbContext> dbFactory)
    {
        InitializeComponent();
        _dbFactory = dbFactory;
    }

    public string? EventId
    {
        get => _eventId.ToString();
        set
        {
            if (int.TryParse(value, out var id))
            {
                _eventId = id;
                _ = LoadAsync();
            }
        }
    }

    private async Task LoadAsync()
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        _model = await db.Events.FirstOrDefaultAsync(x => x.Id == _eventId);
        if (_model is null)
        {
            await DisplayAlert("Virhe", "Tapahtumaa ei löytynyt.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        TitleEntry.Text = _model.Title;
        LocationEntry.Text = _model.Location;
        DatePicker.Date = _model.StartTime.Date;
        TimePicker.Time = _model.StartTime.TimeOfDay;
        DescEditor.Text = _model.Description ?? "";
        MaxEntry.Text = _model.MaxParticipants.ToString();
    }

    private async void OnSave(object sender, EventArgs e)
    {
        if (_model is null) return;

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
        var ev = await db.Events.FirstAsync(x => x.Id == _eventId);

        ev.Title = TitleEntry.Text!.Trim();
        ev.Location = (LocationEntry.Text ?? "").Trim();
        ev.StartTime = start;
        ev.Description = string.IsNullOrWhiteSpace(DescEditor.Text) ? null : DescEditor.Text.Trim();
        ev.MaxParticipants = max;

        await db.SaveChangesAsync();
        await DisplayAlert("OK", "Muutokset tallennettu.", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancel(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
