using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Xaml.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using XAMLDebuggingTechniques.Infrastructure;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace XAMLDebuggingTechniques.WinUI
{
    /// <summary>
    /// WinUI entry point for the .NET MAUI application on Windows.
    /// </summary>
    /// <remarks>
    /// This class bridges the Windows AppModel (WinUI 3) with the MAUI application.
    /// It delegates application creation to <see cref="MauiProgram.CreateMauiApp"/>.
    /// Note: In this repository, Windows assets may be conditionally excluded from
    /// compilation depending on the target frameworks configured in the project file.
    /// </remarks>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Creates a new instance of the Windows application host and initializes
        /// the underlying XAML infrastructure.
        /// </summary>
        public App()
        {
            // Initialize WinUI component graph.
            this.InitializeComponent();

            // Set a stable culture early for predictable parsing/formatting.
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            // Global safety nets: catch unobserved and unhandled exceptions.
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;

            // Surface XAML binding failures to logs while developing.
#if DEBUG
            BindingDiagnostics.BindingFailed += OnBindingFailed;
#endif
        }

        /// <summary>
        /// Creates and returns the configured MAUI application.
        /// </summary>
        /// <returns>
        /// A fully configured <see cref="MauiApp"/> instance produced by
        /// <see cref="MauiProgram.CreateMauiApp"/>.
        /// </returns>
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        // --- Diagnostics & Hardening Hooks (Windows host scope) ---
        private static void UnobservedTaskExceptionHandler(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            // Prevent process crash on finalizer thread and log details.
            try
            {
                var orchestrator = Microsoft.Maui.Controls.Application.Current.GetService<ApplicationLifetime.IAppOrchestrator>();
                if (orchestrator != null && e.Exception is AggregateException agg)
                    orchestrator.ReportUnobservedTaskException(agg, "Windows TaskScheduler");
            }
            catch { /* best effort */ }
            e.SetObserved();
            var err = $"\n*** UNOBSERVED TASK EXCEPTION (observed, continuing) ***\n{e.Exception}\n*** END ***\n";
            Debug.WriteLine(err);
        }

        private static void UnhandledExceptionHandler(object? sender, System.UnhandledExceptionEventArgs e)
        {
            Exception? ex = e.ExceptionObject as Exception;
            try
            {
                var orchestrator = Microsoft.Maui.Controls.Application.Current.GetService<ApplicationLifetime.IAppOrchestrator>();
                if (orchestrator != null && ex is not null)
                    orchestrator.ReportUnhandledException(ex, "Windows AppDomain");
            }
            catch { /* best effort */ }
            var err = "\n*** UNHANDLED EXCEPTION (process will terminate) ***\n";
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
                    //orchestrator?.ReportBindingFailure(prop, uri, line, e?.Message ?? "Binding failure");
                }
                catch { /* best effort */ }
                Debug.WriteLine($"[BindingFailed] {prop} at {uri} line {line}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BindingFailed] Logging error: {ex.Message}");
            }
        }
    }

}
