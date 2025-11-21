using Microsoft.Extensions.Logging;

namespace Tapahtumahubi.App.Logging
{
    public static class SimpleFileLoggerExtensions
    {
        public static ILoggingBuilder AddSimpleFile(
            this ILoggingBuilder builder,
            string path,
            LogLevel minLevel = LogLevel.Information)
        {
            builder.AddProvider(new SimpleFileLoggerProvider(path, minLevel));
            return builder;
        }
    }
}