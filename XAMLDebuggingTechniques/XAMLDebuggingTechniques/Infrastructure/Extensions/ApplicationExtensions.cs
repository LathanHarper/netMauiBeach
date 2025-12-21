using System;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace XAMLDebuggingTechniques.Infrastructure
{
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Safely retrieves the IServiceProvider once the MAUI Application is wired.
        /// Returns null during very early platform boot before the MauiContext is available.
        /// </summary>
        [DebuggerStepThrough]
        public static IServiceProvider? GetServices(this Application? app)
        {
            // When the handler and MauiContext are ready, prefer those services.
            var services = app?.Handler?.MauiContext?.Services;
            return services;
        }

        /// <summary>
        /// Resolves a service from the Application service provider when available,
        /// with an early-boot fallback.
        /// </summary>
        [DebuggerStepThrough]
        public static TService? GetService<TService>(this Application? app) where TService : class
        {
            return app.GetServices()?.GetService(typeof(TService)) as TService;
        }
    }
}
