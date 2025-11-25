using Tapahtumahubi.App.ViewModels;

namespace Tapahtumahubi.App;

public partial class NewEventPage : ContentPage, IQueryAttributable
{
    private readonly NewEventPageViewModel _vm;

    public NewEventPage(NewEventPageViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("EventId", out var v) && v is int id)
            _vm.SetEditingId(id);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
    }
}
