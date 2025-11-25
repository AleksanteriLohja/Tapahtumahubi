using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Tapahtumahubi.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                ShowCrash(e.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException");

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                ShowCrash(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var w = base.CreateWindow(activationState);
            w.Title = "Tapahtumahubi";

            // Näkyvä koko ja sijainti
            w.Width = 1100;
            w.Height = 700;
            w.X = 100;
            w.Y = 100;

            return w;
        }

        private void ShowCrash(Exception? ex, string source)
        {
            try
            {
                Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Current?.MainPage != null)
                    {
                        await Current.MainPage.DisplayAlert(
                            "Virhe",
                            "Sovellus kohtasi odottamattoman virheen. Tapahtuma on kirjattu lokiin.",
                            "OK");
                    }
                });

                // Kirjaa virhe Serilogiin
                Serilog.Log.Error(ex, "CRASH: {Source}", source);
            }
            catch { /* ignore */ }
        }
    }
}
