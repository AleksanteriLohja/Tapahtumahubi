using Tapahtumahubi.App.ViewModels;

namespace Tapahtumahubi.App;

public partial class CalendarPage : ContentPage
{
    public CalendarPage(CalendarPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CalendarPageViewModel vm)
            await vm.InitializeAsync();
    }
}