using System.Globalization;
using Microsoft.Maui.Controls;

namespace XAMLDebuggingTechniques.Converters
{
    public class WaveHeightToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double height)
            {
                if (height < 4)
                {
                    return Color.FromArgb("#bfdbfe");
                }

                if (height < 8)
                {
                    return Color.FromArgb("#fed7aa");
                }

                return Color.FromArgb("#fecaca");
            }

            return Color.FromArgb("#e2e8f0");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
    }

    public class WaveHeightToBorderThicknessConverter : IValueConverter
    {
        public double Scale { get; set; } = 0.2;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double height)
            {
                return height * Scale;
            }

            return 1d;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double thickness && Scale != 0)
            {
                return thickness / Scale;
            }

            return 0d;
        }
    }
}
