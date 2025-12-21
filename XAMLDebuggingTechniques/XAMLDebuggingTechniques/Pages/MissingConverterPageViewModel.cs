using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Prism.Navigation;

namespace XAMLDebuggingTechniques.ViewModels
{
    public class MissingConverterPageViewModel : BindableBase, INavigationAware
    {
        private string _title = "Missing Converter – keep the stoke";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isSurfReportClean = true;
        public bool IsSurfReportClean
        {
            get => _isSurfReportClean;
            set => SetProperty(ref _isSurfReportClean, value);
        }

        private WaveRating _selectedRating = WaveRating.Fun;
        public WaveRating SelectedRating
        {
            get => _selectedRating;
            set => SetProperty(ref _selectedRating, value);
        }

        private double _sprayIntensity = 12;
        public double SprayIntensity
        {
            get => _sprayIntensity;
            set => SetProperty(ref _sprayIntensity, value);
        }

        public IReadOnlyList<WaveRating> Ratings { get; } = Enum.GetValues<WaveRating>();
        public ObservableCollection<ConverterSpec> ConverterCatalog { get; } = new();

        public MissingConverterPageViewModel()
        {
            ConverterCatalog.Add(new ConverterSpec("SurfBoolToColorConverter", "bool -> Color"));
            ConverterCatalog.Add(new ConverterSpec("WaveRatingToGlyphConverter", "enum -> icon text"));
            ConverterCatalog.Add(new ConverterSpec("ScalarToThicknessConverter", "double -> Thickness"));
        }

        public void OnNavigatedTo(INavigationParameters parameters) { }
        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }

    public enum WaveRating
    {
        Flat,
        Fun,
        Firing
    }

    public record ConverterSpec(string Name, string Target);
}
