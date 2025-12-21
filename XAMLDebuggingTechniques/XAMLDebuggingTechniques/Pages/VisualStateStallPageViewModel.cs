using System;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace XAMLDebuggingTechniques.ViewModels
{
    public class VisualStateStallPageViewModel : BindableBase, INavigationAware
    {
        private string _title = "VisualState Stall – choose your tactic";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private double _waveHeight = 3.5;
        public double WaveHeight
        {
            get => _waveHeight;
            set
            {
                if (SetProperty(ref _waveHeight, value))
                {
                    UpdateStateFlags();
                }
            }
        }

        private bool _areStatesEnabled = true;
        public bool AreStatesEnabled
        {
            get => _areStatesEnabled;
            set
            {
                if (SetProperty(ref _areStatesEnabled, value))
                {
                    UpdateStateFlags();
                    VisualStateLog.Add($"States toggled -> {(value ? "Enabled" : "Disabled")} at {DateTime.Now:HH:mm:ss}");
                }
            }
        }

        private bool _isCalmState;
        public bool IsCalmState
        {
            get => _isCalmState;
            set => SetProperty(ref _isCalmState, value);
        }

        private bool _isFunState;
        public bool IsFunState
        {
            get => _isFunState;
            set => SetProperty(ref _isFunState, value);
        }

        private bool _isXlState;
        public bool IsXlState
        {
            get => _isXlState;
            set => SetProperty(ref _isXlState, value);
        }

        public ObservableCollection<string> VisualStateLog { get; } = new();

        public DelegateCommand RandomizeWaveCommand { get; }
        public DelegateCommand ResetWaveCommand { get; }

        private readonly Random _random = new();

        public VisualStateStallPageViewModel()
        {
            RandomizeWaveCommand = new DelegateCommand(RandomizeWave);
            ResetWaveCommand = new DelegateCommand(() => WaveHeight = 3.5);
            UpdateStateFlags();
        }

        private void RandomizeWave()
        {
            WaveHeight = Math.Round(_random.NextDouble() * 12, 1);
            VisualStateLog.Add($"Wave updated to {WaveHeight} ft at {DateTime.Now:HH:mm:ss}");
        }

        private void UpdateStateFlags()
        {
            if (!AreStatesEnabled)
            {
                IsCalmState = IsFunState = IsXlState = false;
                return;
            }

            IsCalmState = WaveHeight < 4;
            IsFunState = WaveHeight >= 4 && WaveHeight < 8;
            IsXlState = WaveHeight >= 8;
        }

        public void OnNavigatedTo(INavigationParameters parameters) { }
        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }
}
