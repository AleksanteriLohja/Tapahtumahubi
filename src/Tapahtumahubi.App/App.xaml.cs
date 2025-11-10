// src/Tapahtumahubi.App/App.xaml.cs
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Tapahtumahubi.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Käynnistetään sovellus AppShelliin
            MainPage = new AppShell();

            // Globaali virhelokitus
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogCrash(e.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException");

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogCrash(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private static void LogCrash(Exception? ex, string source)
        {
            try
            {
                // Kirjoitetaan debugin ajaksi työpöydälle, jotta löydät lokin varmasti
                var path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "Tapahtumahubi_crash.log");

                var msg =
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {source}{Environment.NewLine}" +
                    $"{ex}{Environment.NewLine}{Environment.NewLine}";

                File.AppendAllText(path, msg);
            }
            catch
            {
                // Ei estä kaatumista eikä tee mitään, jos lokitus epäonnistuu
            }
        }
    }
}