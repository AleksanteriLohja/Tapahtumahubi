// src/Tapahtumahubi.App/ViewModels/MainPageViewModel.cs 
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Tapahtumahubi.Domain;
using Tapahtumahubi.Infrastructure;

namespace Tapahtumahubi.App.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public ObservableCollection<Event> Events { get; } = new();
    private List<Event> _all = new();

    [ObservableProperty] private string? searchText;

    public bool IsListEmpty => Events.Count == 0;
    public bool HasItems => Events.Count > 0;

    public MainPageViewModel(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
        Title = "Tapahtumat";
    }

    public async Task InitializeAsync() => await LoadAsync();

    partial void OnSearchTextChanged(string? value) => ApplyFilter();

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            using var db = await _dbFactory.CreateDbContextAsync();
            var now = DateTime.Now;

            _all = await db.Events
                .AsNoTracking()
                .OrderUpcomingFirst(now) // tulevat ensin, sitten StartTime nousevasti 
                .ToListAsync();

            Events.Clear();
            foreach (var e in _all) Events.Add(e);

            OnPropertyChanged(nameof(IsListEmpty));
            OnPropertyChanged(nameof(HasItems));
            ApplyFilter();
        }
        finally { IsBusy = false; }
    }

    private void ApplyFilter()
    {
        IEnumerable<Event> src = _all;
        var q = (SearchText ?? string.Empty).Trim();

        if (!string.IsNullOrWhiteSpace(q))
            src = _all.AsQueryable().Search(q!).ToList();

        Events.Clear();
        foreach (var e in src) Events.Add(e);

        OnPropertyChanged(nameof(IsListEmpty));
        OnPropertyChanged(nameof(HasItems));
    }

    [RelayCommand]
    private async Task NewEventAsync() =>
        await Shell.Current.GoToAsync(nameof(NewEventPage));

    [RelayCommand]
    private async Task EditAsync(Event? ev)
    {
        if (ev is null) return;

        await Shell.Current.GoToAsync(nameof(NewEventPage), new Dictionary<string, object>
        {
            ["EventId"] = ev.Id
        });
    }

    [RelayCommand]
    private async Task OpenParticipantsAsync(Event? ev)
    {
        if (ev is null) return;

        await Shell.Current.GoToAsync(nameof(ParticipantsPage), new Dictionary<string, object>
        {
            ["eventId"] = ev.Id,
            ["eventTitle"] = ev.Title,
            ["maxParticipants"] = ev.MaxParticipants
        });
    }

    // Yhteinen, null-turvallinen Confirm-apu: 
    private static Task<bool> Confirm(string title, string message, string accept, string cancel)
=>
        (Application.Current?.MainPage?.DisplayAlert(title, message, accept, cancel)
            ?? Task.FromResult(false));

    [RelayCommand]
    private async Task DeleteAsync(Event? ev)
    {
        if (ev is null) return;

        var ok = await Confirm("Poista", $"Poistetaanko \"{ev.Title ?? string.Empty}\"?", "Poista",
"Peruuta");
        if (!ok) return;

        using var db = await _dbFactory.CreateDbContextAsync();
        var toRemove = await db.Events.FirstOrDefaultAsync(x => x.Id == ev.Id);
        if (toRemove is null) return;

        db.Events.Remove(toRemove);
        await db.SaveChangesAsync();

        _all.RemoveAll(x => x.Id == ev.Id);
        ApplyFilter();
    }

    [RelayCommand]
    private async Task DeleteAllAsync()
    {
        var ok = await Confirm("Poista kaikki", "Haluatko varmasti poistaa KAIKKI tapahtumat?",
"Poista kaikki", "Peruuta");
        if (!ok) return;

        using var db = await _dbFactory.CreateDbContextAsync();
        db.Events.RemoveRange(db.Events);
        await db.SaveChangesAsync();

        _all.Clear();
        ApplyFilter();
    }
}