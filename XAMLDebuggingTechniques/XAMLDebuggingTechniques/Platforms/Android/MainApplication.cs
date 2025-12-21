using Android.App;
using Android.Runtime;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Xaml.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using XAMLDebuggingTechniques.Infrastructure;

namespace XAMLDebuggingTechniques
{
    [Application]
    public class MainApplication : MauiApplication
    {
        /// <summary>
        /// Android process entry point for the MAUI host. This ties the Android app
        /// lifecycle to the cross-platform <see cref="MauiApp"/> created in <see cref="MauiProgram"/>.
        /// </summary>
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            // Culture: set predictable defaults early on Android.
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            // Global safety nets: catch unobserved and unhandled exceptions.
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidUnhandledExceptionRaiser;

#if DEBUG
            // Surface XAML binding failures during development.
            BindingDiagnostics.BindingFailed += OnBindingFailed;
#endif
        }

        /// <summary>
        /// Creates the shared MAUI application instance.
        /// </summary>
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        // --- Diagnostics & Hardening Hooks (Android host scope) ---
        private static void UnobservedTaskExceptionHandler(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                var orchestrator = Microsoft.Maui.Controls.Application.Current.GetService<ApplicationLifetime.IAppOrchestrator>();
                if (orchestrator != null && e.Exception is AggregateException agg)
                    orchestrator.ReportUnobservedTaskException(agg, "Android TaskScheduler");
            }
            catch { /* best effort */ }
            e.SetObserved();
            var err = $"\n*** ANDROID UNOBSERVED TASK EXCEPTION (observed, continuing) ***\n{e.Exception}\n*** END ***\n";
            Debug.WriteLine(err);
        }

        private static void UnhandledExceptionHandler(object? sender, UnhandledExceptionEventArgs e)
        {
            Exception? ex = e.ExceptionObject as Exception;
            try
            {
                var orchestrator = Microsoft.Maui.Controls.Application.Current.GetService<ApplicationLifetime.IAppOrchestrator>();
                if (orchestrator != null && ex is not null)
                    orchestrator.ReportUnhandledException(ex, "Android AppDomain");
            }
            catch { /* best effort */ }
            var err = "\n*** ANDROID UNHANDLED EXCEPTION (process will terminate) ***\n";
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

        private static void AndroidUnhandledExceptionRaiser(object? sender, RaiseThrowableEventArgs e)
        {
            try
            {
                var orchestrator = Microsoft.Maui.Controls.Application.Current.GetService<ApplicationLifetime.IAppOrchestrator>();
                if (orchestrator != null && e.Exception is not null)
                    orchestrator.ReportUnhandledException(e.Exception, "AndroidEnvironment");
            }
            catch { /* best effort */ }
            var err = $"\n*** ANDROID RAISED UNHANDLED EXCEPTION ***\n{e.Exception}\n*** END ***\n";
            Debug.WriteLine(err);
            // Do not set e.Handled = true by default—avoid masking fatal conditions.
        }

#if DEBUG
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
                Debug.WriteLine($"[Android BindingFailed] {prop} at {uri} line {line}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Android BindingFailed] Logging error: {ex.Message}");
            }
        }
#endif
    }
}
