using Tapahtumahubi.Infrastructure;

namespace Tapahtumahubi.App;

public partial class AddEditParticipantPage : ContentPage, IQueryAttributable
{
    private readonly IParticipantService _svc;

    public string PageTitle { get; set; } = "Lisää osallistuja";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";

    private int _eventId;
    private int? _participantId;

    public AddEditParticipantPage(IParticipantService svc)
    {
        InitializeComponent();
        _svc = svc;
        BindingContext = this;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("eventId", out var idObj) && idObj is int id) _eventId = id;
        if (query.TryGetValue("participantId", out var pid) && pid is int p) _participantId = p;
        if (query.TryGetValue("name", out var n) && n is string ns) Name = ns;
        if (query.TryGetValue("email", out var em) && em is string es) Email = es;

        PageTitle = _participantId.HasValue ? "Muokkaa osallistujaa" : "Lisää osallistuja";
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Email));
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            var name = NameEntry.Text?.Trim() ?? "";
            var email = EmailEntry.Text?.Trim() ?? "";

            if (_participantId.HasValue)
                await _svc.UpdateAsync(_participantId.Value, name, email);
            else
                _ = await _svc.AddAsync(_eventId, name, email);

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", ex.Message, "OK");
        }
    }
}