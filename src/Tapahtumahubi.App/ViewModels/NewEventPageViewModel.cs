using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls; // Application & Shell
using Tapahtumahubi.Domain;
using Tapahtumahubi.Infrastructure;

namespace Tapahtumahubi.App.ViewModels;

public partial class NewEventPageViewModel : BaseViewModel
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    [ObservableProperty] private int id; // 0 = uusi
    [ObservableProperty] private string? titleText;
    [ObservableProperty] private string? location;
    [ObservableProperty] private DateTime eventDate = DateTime.Today;
    [ObservableProperty] private TimeSpan eventTime = new(18, 0, 0);
    [ObservableProperty] private string? description;
    [ObservableProperty] private int maxParticipants = 50;

    private const int TitleMax = 200;
    private const int LocationMax = 200;
    private const int MaxParticipantsUpper = 1000;

    public bool IsEdit => Id > 0;

    public NewEventPageViewModel(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
        Title = "Uusi / Muokkaa tapahtumaa";
    }

    public void SetEditingId(int? eventId) => Id = eventId ?? 0;

    public async Task InitializeAsync()
    {
        if (!IsEdit) return;

        using var db = await _dbFactory.CreateDbContextAsync();
        var ev = await db.Events.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
        if (ev is null) return;

        TitleText = ev.Title;
        Location = ev.Location;
        EventDate = ev.StartTime.Date;
        EventTime = ev.StartTime.TimeOfDay;
        Description = ev.Description;
        MaxParticipants = ev.MaxParticipants;
    }

    private List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(TitleText))
            errors.Add("Otsikko on pakollinen.");
        if ((TitleText?.Length ?? 0) > TitleMax)
            errors.Add($"Otsikon enimmäispituus on {TitleMax} merkkiä.");
        if ((Location?.Length ?? 0) > LocationMax)
            errors.Add($"Sijainnin enimmäispituus on {LocationMax} merkkiä.");
        if (MaxParticipants < 1)
            errors.Add("Osallistujien lukumäärän on oltava vähintään 1.");
        if (MaxParticipants > MaxParticipantsUpper)
            errors.Add($"Osallistujien maksimimäärä on {MaxParticipantsUpper}.");

        var start = EventDate.Date + EventTime;
        if (start < DateTime.Now.AddMinutes(-1))
            errors.Add("Tapahtuma ei voi olla menneisyydessä.");

        return errors;
    }

    // Yksipainikkeinen info/virheilmoitus: DisplayAlert(title, message, cancel) -> Task
    private static Task Alert(string title, string msg, string ok = "OK") =>
        (Application.Current?.MainPage != null
            ? Application.Current.MainPage.DisplayAlert(title, msg, ok)
            : Task.CompletedTask);

    [RelayCommand]
    private async Task SaveAsync()
    {
        var errors = Validate();
        if (errors.Count > 0)
        {
            await Alert("Virhe", string.Join("\n", errors));
            return;
        }

        var start = EventDate.Date + EventTime;
        using var db = await _dbFactory.CreateDbContextAsync();

        if (IsEdit)
        {
            var toUpdate = await db.Events.FirstOrDefaultAsync(x => x.Id == Id);
            if (toUpdate is null)
            {
                await Alert("Virhe", "Tapahtumaa ei löytynyt muokattavaksi.");
                return;
            }

            toUpdate.Title = TitleText!.Trim();
            toUpdate.Location = (Location ?? "").Trim();
            toUpdate.StartTime = start;
            toUpdate.Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim();
            toUpdate.MaxParticipants = MaxParticipants;
        }
        else
        {
            db.Events.Add(new Event
            {
                Title = TitleText!.Trim(),
                Location = (Location ?? "").Trim(),
                StartTime = start,
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                MaxParticipants = MaxParticipants
            });
        }

        await db.SaveChangesAsync();
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CancelAsync() => await Shell.Current.GoToAsync("..");
}