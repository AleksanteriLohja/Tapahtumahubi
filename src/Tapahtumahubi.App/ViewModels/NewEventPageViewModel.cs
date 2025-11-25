using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
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

    [RelayCommand]
    private async Task SaveAsync()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(TitleText)) errors.Add("Otsikko on pakollinen");
        if ((TitleText?.Length ?? 0) > 200) errors.Add("Otsikon enimmäispituus on 200 merkkiä");
        if ((Location?.Length ?? 0) > 200) errors.Add("Sijainnin enimmäispituus on 200 merkkiä");
        if (MaxParticipants < 1) errors.Add("Osallistujien lukumäärän on oltava vähintään 1");

        if (errors.Count > 0)
        {
            await Shell.Current.DisplayAlert("Virhe", string.Join("\n", errors), "OK");
            return;
        }

        var start = EventDate.Date + EventTime;

        using var db = await _dbFactory.CreateDbContextAsync();

        if (IsEdit)
        {
            var toUpdate = await db.Events.FirstOrDefaultAsync(x => x.Id == Id);
            if (toUpdate is null)
            {
                await Shell.Current.DisplayAlert("Virhe", "Tapahtumaa ei löytynyt muokattavaksi.", "OK");
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
