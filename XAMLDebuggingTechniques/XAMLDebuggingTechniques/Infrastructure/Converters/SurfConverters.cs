using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace XAMLDebuggingTechniques.Converters
{
    public class SurfBoolToColorConverter : IValueConverter
    {
        public Color TrueColor { get; set; } = Color.FromArgb("#0ea5e9");
        public Color FalseColor { get; set; } = Color.FromArgb("#94a3b8");

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool flag)
            {
                return flag ? TrueColor : FalseColor;
            }

            return FalseColor;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
    }

    public class WaveRatingToGlyphConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                ViewModels.WaveRating.Flat => "🌊",
                ViewModels.WaveRating.Fun => "🤙",
                ViewModels.WaveRating.Firing => "⚡",
                _ => "?"
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
    }

    public class ScalarToThicknessConverter : IValueConverter
    {
        public double Multiplier { get; set; } = 0.5;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double scalar)
            {
                var actual = scalar * Multiplier;
                return new Thickness(actual);
            }

            return new Thickness(0);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                return thickness.Left / Multiplier;
            }

            return 0d;
        }
    }

    public class StringEqualsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string input && parameter is string target)
            {
                return string.Equals(input, target, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
