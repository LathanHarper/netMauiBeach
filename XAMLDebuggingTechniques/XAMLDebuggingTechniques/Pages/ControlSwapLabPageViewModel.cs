using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using XAMLDebuggingTechniques.Infrastructure.Interfaces;

namespace XAMLDebuggingTechniques.ViewModels
{
    public class ControlSwapLabPageViewModel : BindableBase, INavigationAware, ITemplateRecorder
    {
        private string _title = "Control Swap Lab – swap boards mid-wave";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        // existing fields left for compatibility
        private string _selectedSurface = "Map";
        public string SelectedSurface
        {
            get => _selectedSurface;
            set => SetProperty(ref _selectedSurface, value);
        }

        private string _lastTemplateUsed = "Line";
        public string LastTemplateUsed
        {
            get => _lastTemplateUsed;
            set => SetProperty(ref _lastTemplateUsed, value);
        }

        public IReadOnlyList<string> SurfaceModes { get; } = new[] { "Map", "Media", "List" };
        public ObservableCollection<TelemetryTile> TelemetryTiles { get; } = new();
        public ObservableCollection<string> SwapLog { get; } = new();

        public DelegateCommand<string> SwitchSurfaceCommand { get; }

        // New fin-related properties
        public ObservableCollection<SurfFin> Fins { get; } = new();

        private SurfFin? _selectedFin;
        public SurfFin? SelectedFin
        {
            get => _selectedFin;
            set => SetProperty(ref _selectedFin, value);
        }

        public DelegateCommand<string> SetFinColorCommand { get; }

        public ControlSwapLabPageViewModel()
        {
            SwitchSurfaceCommand = new DelegateCommand<string>(SwitchSurface);
            SetFinColorCommand = new DelegateCommand<string>(SetFinColor);

            // telemetry preserved for other demos
            TelemetryTiles.Add(new TelemetryTile("Wave Period", "14 sec", TelemetryTileType.Line));
            TelemetryTiles.Add(new TelemetryTile("Wind", "7 kts", TelemetryTileType.Gauge));
            TelemetryTiles.Add(new TelemetryTile("Rip Alert", "Strong", TelemetryTileType.Badge));
            TelemetryTiles.Add(new TelemetryTile("Crowd", "Low", TelemetryTileType.Badge));

            // Initialize fins (Left, Center, Right)
            Fins.Add(new SurfFin("Left", "#ef4444", 3.5));
            Fins.Add(new SurfFin("Center", "#f59e0b", 4.2));
            Fins.Add(new SurfFin("Right", "#0ea5e9", 3.0));

            // default selection
            SelectedFin = Fins.Count > 0 ? Fins[0] : null;
        }

        private void SetFinColor(string? colorHex)
        {
            if (string.IsNullOrWhiteSpace(colorHex) || SelectedFin == null)
                return;

            SelectedFin.FinColor = colorHex;
        }

        private void SwitchSurface(string? surface)
        {
            if (string.IsNullOrWhiteSpace(surface))
            {
                return;
            }

            SelectedSurface = surface;
            SwapLog.Add($"Surface switched to {surface} at {DateTime.Now:HH:mm:ss}");
        }

        public void RecordTemplate(string templateName)
        {
            LastTemplateUsed = templateName;
        }

        public void OnNavigatedTo(INavigationParameters parameters) { }
        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }

    public enum TelemetryTileType
    {
        Line,
        Gauge,
        Badge
    }

    public record TelemetryTile(string Title, string Reading, TelemetryTileType TileType);

    // Model for fins used by the CollectionView
    public class SurfFin : BindableBase
    {
        public SurfFin(string position, string finColor, double length)
        {
            Position = position;
            _finColor = finColor;
            _length = length;
        }

        private string _position = string.Empty;
        public string Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        private string _finColor = "#ffffff";
        public string FinColor
        {
            get => _finColor;
            set => SetProperty(ref _finColor, value);
        }

        private double _length;
        public double Length
        {
            get => _length;
            set => SetProperty(ref _length, value);
        }
    }
}
