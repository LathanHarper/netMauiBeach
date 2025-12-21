using Foundation;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Xaml.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using XAMLDebuggingTechniques.Infrastructure;

namespace XAMLDebuggingTechniques
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        /// <summary>
        /// Creates and returns the configured MAUI application used by iOS.
        /// </summary>
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIKit.UIApplication application, Foundation.NSDictionary launchOptions)
        {
            // Culture: set predictable defaults early on iOS.
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            // Global safety nets: catch unobserved and unhandled exceptions.
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;

            // Surface XAML binding failures during development.
#if DEBUG
            BindingDiagnostics.BindingFailed += OnBindingFailed;
#endif

            return base.FinishedLaunching(application, launchOptions);
        }

        // --- Diagnostics & Hardening Hooks (iOS host scope) ---
        private static void UnobservedTaskExceptionHandler(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                var orchestrator = Microsoft.Maui.Controls.Application.Current.GetService<ApplicationLifetime.IAppOrchestrator>();
                if (orchestrator != null && e.Exception is AggregateException agg)
                    orchestrator.ReportUnobservedTaskException(agg, "iOS TaskScheduler");
            }
            catch { /* best effort */ }
            e.SetObserved();
            var err = $"\n*** iOS UNOBSERVED TASK EXCEPTION (observed, continuing) ***\n{e.Exception}\n*** END ***\n";
            Debug.WriteLine(err);
        }

        private static void UnhandledExceptionHandler(object? sender, UnhandledExceptionEventArgs e)
        {
            Exception? ex = e.ExceptionObject as Exception;
            try
            {
                var orchestrator = Microsoft.Maui.Controls.Application.Current.GetService<ApplicationLifetime.IAppOrchestrator>();
                if (orchestrator != null && ex is not null)
                    orchestrator.ReportUnhandledException(ex, "iOS AppDomain");
            }
            catch { /* best effort */ }
            var err = "\n*** iOS UNHANDLED EXCEPTION (process will terminate) ***\n";
            if (ex is not null)
            {
                err += $"HRESULT: {ex.HResult}\n";
                if (ex.Message is not null) err += $"-- MESSAGE --\n{ex.Message}\n";
                if (ex.StackTrace is not null) err += $"-- STACKTRACE --\n{ex.StackTrace}\n";
                if (ex.Source is not null) err += $"-- SOURCE --\n{ex.Source}\n";
                if (ex.InnerException is not null) err += $"-- INNER --\n{ex.InnerException}\n";
            }
            else
            {
                err += $"DETAILS: {e.ExceptionObject}";
            }
            err += "\n*** END ***\n";
            Debug.WriteLine(err);
        }

        private static void OnBindingFailed(object? sender, BindingBaseErrorEventArgs e)
        {
            try
            {
                var line = e?.XamlSourceInfo?.LineNumber;
                var uri = e?.XamlSourceInfo?.SourceUri;
                var bindingEx = e as BindingErrorEventArgs;
                var prop = bindingEx?.MessageArgs is { Length: > 0 } ? bindingEx.MessageArgs[0] : null;
                try
                {
                    var orchestrator = Microsoft.Maui.Controls.Application.Current.GetService<ApplicationLifetime.IAppOrchestrator>();
                    orchestrator?.ReportBindingFailure(prop?.ToString(), uri, line, e?.Message ?? "Binding failure");
                }
                catch { /* best effort */ }
                Debug.WriteLine($"[iOS BindingFailed] {prop} at {uri} line {line}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[iOS BindingFailed] Logging error: {ex.Message}");
            }
        }
    }
}
