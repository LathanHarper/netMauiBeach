using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace XAMLDebuggingTechniques.ViewModels
{
    public class BindingContextLostPageViewModel : BindableBase, INavigationAware
    {
        private string _title = "BindingContext Lost – keep the leash tight";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private SurfSet _activeSet = new()
        {
            Name = "Sunrise Reef",
            Condition = "glassy",
            PrimaryCoach = "Aunty Lani"
        };
        public SurfSet ActiveSet
        {
            get => _activeSet;
            set => SetProperty(ref _activeSet, value);
        }

        private bool _isSignalVisible;
        public bool IsSignalVisible
        {
            get => _isSignalVisible;
            set => SetProperty(ref _isSignalVisible, value);
        }

        private string _modalMessage = "Sharing BindingContext straight from the page root so nothing snaps mid-ride.";
        public string ModalMessage
        {
            get => _modalMessage;
            set => SetProperty(ref _modalMessage, value);
        }

        private int _popoverAttachCount;
        public int PopoverAttachCount
        {
            get => _popoverAttachCount;
            set => SetProperty(ref _popoverAttachCount, value);
        }

        private int _templateAttachCount;
        public int TemplateAttachCount
        {
            get => _templateAttachCount;
            set => SetProperty(ref _templateAttachCount, value);
        }

        private HandOffSlide? _selectedSlide;
        public HandOffSlide? SelectedSlide
        {
            get => _selectedSlide;
            set => SetProperty(ref _selectedSlide, value);
        }

        public ObservableCollection<WaveReading> WaveReadings { get; } = new();
        public ObservableCollection<HandOffSlide> ContextHandOffs { get; } = new();
        public ObservableCollection<ContextBreadcrumb> Breadcrumbs { get; } = new();

        public DelegateCommand TogglePopoverVisibilityCommand { get; }
        private void ToggleSignal()
        {
            IsSignalVisible = !IsSignalVisible;
            if (IsSignalVisible)
            {
                PopoverAttachCount++;
                Breadcrumbs.Add(new ContextBreadcrumb("Popup", "Shared BindingContext engaged"));
            }
            else
            {
                Breadcrumbs.Add(new ContextBreadcrumb("Popup", "Context released"));
            }
        }


        public DelegateCommand ShuffleReadingsCommand { get; }

        private readonly Random _random = new();

        public BindingContextLostPageViewModel()
        {
            TogglePopoverVisibilityCommand = new DelegateCommand(ToggleSignal);
            ShuffleReadingsCommand = new DelegateCommand(ShuffleReadings);

            WaveReadings.Add(new WaveReading("Inside Bowl", 3.4));
            WaveReadings.Add(new WaveReading("Channel", 2.1));
            WaveReadings.Add(new WaveReading("Wide Peak", 4.2));

            ContextHandOffs.Add(new HandOffSlide("Page -> Popup", "Share BindingContext via x:Reference so overlays stay synced."));
            ContextHandOffs.Add(new HandOffSlide("Parent -> Template", "Keep template BindingContext aligned or controls go null."));
            ContextHandOffs.Add(new HandOffSlide("Navigation -> Child", "Pass parameters through BindingContext during navigation."));

            Breadcrumbs.Add(new ContextBreadcrumb("Page", "BindingContext initialized"));
            Breadcrumbs.Add(new ContextBreadcrumb("CollectionView", "Item template inherits BindingContext"));
        }

        

        private void ShuffleReadings()
        {
            foreach (var reading in WaveReadings)
            {
                reading.Height = Math.Round(2 + _random.NextDouble() * 3, 1);
                reading.LastUpdated = DateTime.Now;
            }

            TemplateAttachCount++;
            Breadcrumbs.Add(new ContextBreadcrumb("Template", "BindingContext refreshed"));
        }

        public void OnNavigatedTo(INavigationParameters parameters) { }
        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }

    public class SurfSet : BindableBase
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _condition = string.Empty;
        public string Condition
        {
            get => _condition;
            set => SetProperty(ref _condition, value);
        }

        private string _primaryCoach = string.Empty;
        public string PrimaryCoach
        {
            get => _primaryCoach;
            set => SetProperty(ref _primaryCoach, value);
        }
    }

    public class WaveReading : BindableBase
    {
        public WaveReading(string location, double height)
        {
            Location = location;
            Height = height;
        }

        private string _location;
        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        private double _height;
        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        private DateTime _lastUpdated = DateTime.Now;
        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set => SetProperty(ref _lastUpdated, value);
        }
    }

    public record HandOffSlide(string Title, string Notes);
    public record ContextBreadcrumb(string Source, string Message);
}
