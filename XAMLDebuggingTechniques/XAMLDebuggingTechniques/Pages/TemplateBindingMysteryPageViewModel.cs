using System.Collections.ObjectModel;
using Prism.Mvvm;
using Prism.Navigation;

namespace XAMLDebuggingTechniques.ViewModels
{
    public class TemplateBindingMysteryPageViewModel : BindableBase, INavigationAware
    {
        private string _title = "TemplateBinding Mystery – no more spooky";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public TemplateCardModel SurfCard { get; } = new("Surf Telemetry", "TemplateBinding keeps this reusable.", "🏄");
        public TemplateCardModel LifeguardCard { get; } = new("RelativeSource Lifeguard", "TemplatedParent + ancestor bindings", "🛟");

        private string _templateFacts = "TemplateBinding hits dependency properties, while RelativeSource chases ancestors.";
        public string TemplateFacts
        {
            get => _templateFacts;
            set => SetProperty(ref _templateFacts, value);
        }

        public ObservableCollection<TemplateDiagnostic> TemplateDiagnostics { get; } = new();

        public TemplateBindingMysteryPageViewModel()
        {
            TemplateDiagnostics.Add(new TemplateDiagnostic("TemplateBinding", "Accesses the templated control's properties."));
            TemplateDiagnostics.Add(new TemplateDiagnostic("RelativeSource TemplatedParent", "Perfect for nested ContentPresenters."));
            TemplateDiagnostics.Add(new TemplateDiagnostic("RelativeSource AncestorType", "Hop up to ContentPage for shared context."));
        }

        public void OnNavigatedTo(INavigationParameters parameters) { }
        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }

    public record TemplateCardModel(string Title, string Subtitle, string Icon);
    public record TemplateDiagnostic(string Topic, string Notes);
}
