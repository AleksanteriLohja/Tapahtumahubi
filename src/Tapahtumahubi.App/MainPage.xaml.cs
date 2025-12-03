using Tapahtumahubi.App.ViewModels;

namespace Tapahtumahubi.App
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel _vm = null!;

        public MainPage()
        {
            InitializeComponent();
            _vm = ServiceHelper.GetRequiredService<MainPageViewModel>();
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.InitializeAsync();
        }
    }
}