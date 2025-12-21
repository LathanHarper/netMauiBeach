using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace XAMLDebuggingTechniques.ViewModels
{
    public class BindingBasicsPageViewModel : BindableBase, INavigationAware
    {
        private string _title = "Binding Basics – Tune your leash";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _surferName = "Keani";
        public string SurferName
        {
            get => _surferName;
            set
            {
                if (SetProperty(ref _surferName, value))
                {
                    RefreshBindingPulse();
                }
            }
        }

        private SurferProfile _activeSurfer = new() { Name = "Coach Alana" };
        public SurferProfile ActiveSurfer
        {
            get => _activeSurfer;
            set
            {
                if (SetProperty(ref _activeSurfer, value))
                {
                    RefreshBindingPulse();
                }
            }
        }

        private int _waveCounter = 3;
        public int WaveCounter
        {
            get => _waveCounter;
            set
            {
                if (SetProperty(ref _waveCounter, value))
                {
                    RefreshBindingPulse();
                }
            }
        }

        private double _boardLengthFeet = 6.2;
        public double BoardLengthFeet
        {
            get => _boardLengthFeet;
            set
            {
                if (SetProperty(ref _boardLengthFeet, value))
                {
                    RefreshBindingPulse();
                }
            }
        }

        public ObservableCollection<BindingPulseRow> BindingPulseEntries { get; } = new();

        public DelegateCommand CoachSetBoardCommand { get; }

        public BindingBasicsPageViewModel()
        {
            CoachSetBoardCommand = new DelegateCommand(SetBoardFromCoach);
            RefreshBindingPulse();
        }

        private void SetBoardFromCoach()
        {
            BoardLengthFeet = 7.0;
        }

        private void RefreshBindingPulse()
        {
            BindingPulseEntries.Clear();
            BindingPulseEntries.Add(new BindingPulseRow("SurferName", SurferName));
            BindingPulseEntries.Add(new BindingPulseRow("ActiveSurfer.Name", ActiveSurfer?.Name ?? "—"));
            BindingPulseEntries.Add(new BindingPulseRow("WaveCounter", WaveCounter.ToString()));
            BindingPulseEntries.Add(new BindingPulseRow("BoardLengthFeet", BoardLengthFeet.ToString("0.0")));
        }

        public void OnNavigatedTo(INavigationParameters parameters) { }
        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }

    public class SurferProfile : BindableBase
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }

    public record BindingPulseRow(string Path, string Value);
}
