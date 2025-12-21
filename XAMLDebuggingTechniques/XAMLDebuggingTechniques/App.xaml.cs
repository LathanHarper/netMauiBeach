using Prism;
using Prism.Ioc;
using System.Diagnostics;
using Microsoft.Maui.Controls.Xaml.Diagnostics;
using ApplicationLifetime;

namespace XAMLDebuggingTechniques
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

#if DEBUG
            // Surface XAML binding failures from shared app scope while developing.
            BindingDiagnostics.BindingFailed += OnBindingFailed;
#endif
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
                    // Resolve orchestrator via Prism's container locator (safer in Prism apps)
                    var orchestrator = Prism.Ioc.ContainerLocator.Container.Resolve<IAppOrchestrator>();
                    orchestrator?.ReportBindingFailure(prop?.ToString(), uri, line, e?.Message ?? "Binding failure");
                }
                catch { /* best effort */ }
                Debug.WriteLine($"[BindingFailed] {prop} at {uri} line {line}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BindingFailed] Logging error: {ex.Message}");
            }
        }
#endif

        // Prism handles app startup and navigation via UsePrism(OnAppStart)
        // so we don't override CreateWindow or set MainPage here.
    }
}