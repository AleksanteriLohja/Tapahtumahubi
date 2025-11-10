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

        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            await DisplayAlert("Virhe", "Otsikko on pakollinen.", "OK");
            return;
        }

        int max = _model.MaxParticipants;
        _ = int.TryParse(MaxEntry.Text, out max);
        var start = DatePicker.Date + TimePicker.Time;

        using var db = await _dbFactory.CreateDbContextAsync();
        var ev = await db.Events.FirstAsync(x => x.Id == _eventId);

        ev.Title = TitleEntry.Text!.Trim();
        ev.Location = (LocationEntry.Text ?? "").Trim();
        ev.StartTime = start;
        ev.Description = string.IsNullOrWhiteSpace(DescEditor.Text) ? null : DescEditor.Text.Trim();
        ev.MaxParticipants = max > 0 ? max : 50;

        await db.SaveChangesAsync();
        await DisplayAlert("OK", "Muutokset tallennettu.", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancel(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}