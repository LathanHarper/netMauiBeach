using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.Collections.ObjectModel;

namespace XAMLDebuggingTechniques.ViewModels
{
    public class SampleLauncherPageViewModel : BindableBase, INavigationAware
    {
        private readonly INavigationService _navigationService;

        private string _title = "XAML Debugging Techniques";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ObservableCollection<SampleItem> Samples { get; }

        public DelegateCommand<SampleItem> NavigateCommand { get; }

        public SampleLauncherPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateCommand = new DelegateCommand<SampleItem>(OnNavigate);

            // Lineup of all demo pages - like flavors on the board
            Samples = new ObservableCollection<SampleItem>
            {
                new SampleItem { Title = "Binding Basics", Description = "Property-path bindings, UpdateSourceEventName, OneWay vs TwoWay", NavigationKey = "BindingBasicsPage" },
                new SampleItem { Title = "BindingContext Lost", Description = "Diagnose where BindingContext vanishes mid-session", NavigationKey = "BindingContextLostPage" },
                new SampleItem { Title = "Missing Converter", Description = "Type-mismatch wipeouts and IValueConverter fixes", NavigationKey = "MissingConverterPage" },
                new SampleItem { Title = "Navigation Fail", Description = "Debug why routes don't resolve", NavigationKey = "NavigationFailPage" },
                new SampleItem { Title = "VisualState Stall", Description = "When to bail on VisualStateManager and use converters", NavigationKey = "VisualStateStallPage" },
                new SampleItem { Title = "TemplateBinding Mystery", Description = "ControlTemplate scoping and RelativeSource gotchas", NavigationKey = "TemplateBindingMysteryPage" },
                new SampleItem { Title = "Control Swap Lab", Description = "Simple first indicator to verify the problem is not in a control", NavigationKey = "ControlSwapLabPage" },
                new SampleItem { Title = "Debug Overlay Demo", Description = "Live binding/property values via custom overlays", NavigationKey = "DebugOverlayDemoPage" }
            };
        }

        private async void OnNavigate(SampleItem sample)
        {
            if (sample == null) return;
            try
            {
                //Fred Navigate
                var retval = await _navigationService.NavigateAsync(sample.NavigationKey);
                if (!retval.Success)
                {
                    var dum = true;
                }
            }
            catch (Exception ex)
            {
                var dum = true;
                throw;
            }
        }

        public void OnNavigatedTo(INavigationParameters parameters) { }
        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }

    public class SampleItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string NavigationKey { get; set; }
    }
}
