using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace XAMLDebuggingTechniques.ViewModels
{
    public class DebugOverlayDemoPageViewModel : BindableBase, INavigationAware
    {
        private string _title = "Debug Overlay Demo – slap on a HUD";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isHudVisible = true;
        public bool IsHudVisible
        {
            get => _isHudVisible;
            set => SetProperty(ref _isHudVisible, value);
        }

        public SessionTelemetry Telemetry { get; } = new()
        {
            SurferName = "Makoa",
            WaveHeight = 5.4,
            IsClean = true
        };

        public ObservableCollection<BindingChip> BindingChips { get; } = new();
        public ObservableCollection<BindingSnapshot> Snapshots { get; } = new();

        public DelegateCommand CaptureSnapshotCommand { get; }

        public DebugOverlayDemoPageViewModel()
        {
            CaptureSnapshotCommand = new DelegateCommand(CaptureSnapshot);
            Telemetry.PropertyChanged += HandleTelemetryChanged;
            RefreshChips();
        }

        private void HandleTelemetryChanged(object? sender, PropertyChangedEventArgs e)
        {
            RefreshChips();
        }

        private void RefreshChips()
        {
            BindingChips.Clear();
            BindingChips.Add(new BindingChip("SurferName", Telemetry.SurferName, DateTime.Now));
            BindingChips.Add(new BindingChip("WaveHeight", Telemetry.WaveHeight.ToString("0.0"), DateTime.Now));
            BindingChips.Add(new BindingChip("IsClean", Telemetry.IsClean.ToString(), DateTime.Now));
        }

        private void CaptureSnapshot()
        {
            var description = $"{Telemetry.SurferName} | {Telemetry.WaveHeight:0.0}ft | Clean={Telemetry.IsClean}";
            Snapshots.Add(new BindingSnapshot(DateTime.Now, description));
        }

        public void OnNavigatedTo(INavigationParameters parameters) { }
        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }

    public class SessionTelemetry : BindableBase
    {
        private string _surferName = string.Empty;
        public string SurferName
        {
            get => _surferName;
            set => SetProperty(ref _surferName, value);
        }

        private double _waveHeight;
        public double WaveHeight
        {
            get => _waveHeight;
            set => SetProperty(ref _waveHeight, value);
        }

        private bool _isClean;
        public bool IsClean
        {
            get => _isClean;
            set => SetProperty(ref _isClean, value);
        }
    }

    public record BindingChip(string Path, string Value, DateTime Timestamp);
    public record BindingSnapshot(DateTime Timestamp, string Description);
}
