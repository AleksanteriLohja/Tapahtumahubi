using System.Collections.ObjectModel;
using Tapahtumahubi.Domain;
using Tapahtumahubi.Infrastructure;

namespace Tapahtumahubi.App;

public partial class ParticipantsPage : ContentPage, IQueryAttributable
{
    private readonly IParticipantService _svc;

    public ObservableCollection<Participant> Participants { get; } = new();
    public string PageTitle { get; set; } = "Osallistujat";
    private int _eventId;
    private int _maxParticipants = int.MaxValue;

    public string CounterText => $"{Participants.Count} / {_maxParticipants}";

    public ParticipantsPage(IParticipantService svc)
    {
        InitializeComponent();
        _svc = svc;
        BindingContext = this;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("eventId", out var idObj) && idObj is int id) _eventId = id;
        if (query.TryGetValue("eventTitle", out var titleObj) && titleObj is string title && !string.IsNullOrWhiteSpace(title))
            PageTitle = $"Osallistujat – {title}";
        if (query.TryGetValue("maxParticipants", out var maxObj) && maxObj is int max && max > 0)
            _maxParticipants = max;

        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(CounterText));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        Participants.Clear();
        var list = await _svc.ListByEventAsync(_eventId);
        foreach (var p in list) Participants.Add(p);
        OnPropertyChanged(nameof(CounterText));
    }

    private async void OnRefreshClicked(object sender, EventArgs e) => await LoadAsync();

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (Participants.Count >= _maxParticipants)
        {
            await DisplayAlert("Tapahtuma täynnä", "Et voi lisätä enempää osallistujia.", "OK");
            return;
        }

        await Shell.Current.GoToAsync(nameof(AddEditParticipantPage),
            new Dictionary<string, object> { { "eventId", _eventId } });
    }

    // UUSI: selkeät napit desktopille
    private async void OnEditClick(object sender, EventArgs e)
    {
        if (sender is Button b && b.CommandParameter is Participant p)
        {
            await Shell.Current.GoToAsync(nameof(AddEditParticipantPage),
                new Dictionary<string, object> {
                    { "participantId", p.Id },
                    { "eventId", _eventId },
                    { "name", p.Name },
                    { "email", p.Email }
                });
        }
    }

    private async void OnDeleteClick(object sender, EventArgs e)
    {
        if (sender is Button b && b.CommandParameter is Participant p)
        {
            var ok = await DisplayAlert("Vahvista poisto",
                $"Poistetaanko {p.Name} ({p.Email})?", "Poista", "Peruuta");
            if (!ok) return;

            try
            {
                await _svc.DeleteAsync(p.Id);
                Participants.Remove(p);
                OnPropertyChanged(nameof(CounterText));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", ex.Message, "OK");
            }
        }
    }
}