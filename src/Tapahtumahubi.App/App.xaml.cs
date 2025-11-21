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
#if DEBUG
        private readonly string _logPath;
        private static readonly object _sync = new();

        private void WriteLog(string message)
        {
            try
            {
                var line = $"{DateTime.Now:yyyy-MM-dd HH.mm.ss.fff} {message}{Environment.NewLine}";
                lock (_sync)
                {
                    File.AppendAllText(_logPath, line);
                }
            }
            catch
            {
                // best-effort
            }
        }
#endif

        public App()
        {
#if DEBUG
            // PAKOTETAAN DEBUG-LOKI TÄHÄN POLKUUN RIIPPUMATTA packaged/unpackaged-tilasta:
            var baseDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Tapahtumahubi.App");
            var logsDir = Path.Combine(baseDir, "logs");
            Directory.CreateDirectory(logsDir);
            _logPath = Path.Combine(logsDir, "app.log");

            WriteLog($"[APP] ctor start; AppData={baseDir}");
            WriteLog("[APP] smoke test");
#else
            // Release: käytä MAUI:n AppDataDirectorya
            var logsDir = Path.Combine(FileSystem.AppDataDirectory, "logs");
            Directory.CreateDirectory(logsDir);
            var _logPath = Path.Combine(logsDir, "app.log");
#endif
            InitializeComponent();
#if DEBUG
            WriteLog("[APP] InitializeComponent OK");
#endif

            MainPage = new AppShell();
#if DEBUG
            WriteLog($"[APP] MainPage set -> {MainPage?.GetType().Name ?? "(null)"}");
#endif

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogCrash(e.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException");

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogCrash(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
#if DEBUG
            WriteLog("[WIN] CreateWindow start");
#endif
            var w = base.CreateWindow(activationState);
            w.Title = "Tapahtumahubi";

            // Varmistetaan näkyvä koko ja sijainti
            w.Width = 1100;
            w.Height = 700;
            w.X = 100;
            w.Y = 100;
#if DEBUG
            WriteLog("[WIN] CreateWindow OK");
#endif
            return w;
        }

        private void LogCrash(Exception? ex, string source)
        {
#if DEBUG
            try { WriteLog($"[APP] CRASH {source}{Environment.NewLine}{ex}"); } catch { }
#endif
            try
            {
                Microsoft.Maui.ApplicationModel.MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Current?.MainPage != null)
                    {
                        await Current.MainPage.DisplayAlert(
                            "Virhe",
                            "Sovellus kohtasi odottamattoman virheen. Tapahtuma on kirjattu lokiin (Debug).",
                            "OK");
                    }
                });
            }
            catch { /* ignore */ }
        }
    }
}
