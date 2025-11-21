using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Tapahtumahubi.App.Logging
{
    public sealed class SimpleFileLoggerProvider : ILoggerProvider
    {
        private readonly LogLevel _min;
        private readonly object _gate = new();
        private readonly StreamWriter _writer;

        public SimpleFileLoggerProvider(string path, LogLevel minLevel)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            _writer = new StreamWriter(new FileStream(
                path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            { AutoFlush = true };

            _min = minLevel;
        }

        public ILogger CreateLogger(string categoryName)
            => new SimpleFileLogger(categoryName, _writer, _min, _gate);

        public void Dispose() => _writer.Dispose();

        private sealed class SimpleFileLogger : ILogger
        {
            private readonly string _category;
            private readonly StreamWriter _owner;
            private readonly LogLevel _min;
            private readonly object _gate;

            public SimpleFileLogger(string category, StreamWriter owner, LogLevel min, object gate)
            {
                _category = category;
                _owner = owner;
                _min = min;
                _gate = gate;
            }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull
                => NullScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => logLevel >= _min;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                                    Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel)) return;

                var msg = formatter(state, exception);
                var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {_category} :: {msg}";
                if (exception != null) line += Environment.NewLine + exception;

                lock (_gate)
                {
                    _owner.WriteLine(line);
                }
            }

            private sealed class NullScope : IDisposable
            {
                public static readonly NullScope Instance = new();
                public void Dispose() { }
            }
        }
    }
}