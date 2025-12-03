using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Tapahtumahubi.Domain;
using Tapahtumahubi.Infrastructure;

namespace Tapahtumahubi.App.ViewModels;

public partial class CalendarPageViewModel : BaseViewModel
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    [ObservableProperty] 
    private DateTime selectedDate = DateTime.Today;

    // Ryhmät, joita CollectionView näyttää IsGrouped="True" -tilassa
    public ObservableCollection<CalendarDayGroup> Days { get; } = new();

    public CalendarPageViewModel(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
        Title = "Kalenteri";
    }

    public async Task InitializeAsync() => await LoadAsync();

    [RelayCommand]
    private async Task TodayAsync()
    {
        SelectedDate = DateTime.Today;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            using var db = await _dbFactory.CreateDbContextAsync();

            var start = SelectedDate.Date.AddDays(-7);
            var end   = SelectedDate.Date.AddDays(14);

            var items = await db.Events.AsNoTracking()
                .Where(e => e.StartTime >= start && e.StartTime < end)
                .OrderBy(e => e.StartTime)
                .ToListAsync();

            // RYHMITYS: luo ryhmät niin, että jokainen ryhmä *itsessään* on IEnumerable<Event>
            var groups = items
                .GroupBy(e => e.StartTime.Date)
                .OrderBy(g => g.Key)
                .Select(g => new CalendarDayGroup(g.Key, g))
                .ToList();

            Days.Clear();
            foreach (var g in groups) Days.Add(g);
        }
        finally
        {
            IsBusy = false;
        }
    }
}

/// <summary>
/// Korjattu ryhmätyyppi: perii ObservableCollection&lt;Event&gt;,
/// jolloin CollectionView osaa renderöidä ryhmän ilman kaatumista.
/// </summary>
public sealed class CalendarDayGroup : ObservableCollection<Event>
{
    public DateTime Date { get; }
    public string DateString => Date.ToString("dddd dd.MM.yyyy");

    public CalendarDayGroup(DateTime date, IEnumerable<Event> items) : base(items)
    {
        Date = date;
    }
}