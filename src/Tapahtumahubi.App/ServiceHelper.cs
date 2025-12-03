using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace Tapahtumahubi.App
{
    public static class ServiceHelper
    {
        public static IServiceProvider Current =>
            Application.Current?.Handler?.MauiContext?.Services
            ?? throw new InvalidOperationException("Service provider is not initialized.");

        public static T GetRequiredService<T>() where T : notnull =>
            Current.GetRequiredService<T>();
    }
}