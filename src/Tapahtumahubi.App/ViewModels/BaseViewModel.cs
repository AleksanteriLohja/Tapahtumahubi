using CommunityToolkit.Mvvm.ComponentModel;

namespace Tapahtumahubi.App.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? title;
}
