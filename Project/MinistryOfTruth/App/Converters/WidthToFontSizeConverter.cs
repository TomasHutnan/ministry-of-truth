using System.Globalization;

namespace App.Converters;

public class WidthToFontSizeConverter : IValueConverter
{
    public double DefaultRatio { get; set; } = 0.1;
    public double MinFontSize { get; set; } = 12;
    public double MaxFontSize { get; set; } = 180;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double width || width <= 0)
        {
            return MinFontSize;
        }

        var ratio = DefaultRatio;
        var min = MinFontSize;
        var max = MaxFontSize;

        if (parameter is double ratioParam)
        {
            ratio = ratioParam;
        }
        else if (parameter is string s && !string.IsNullOrWhiteSpace(s))
        {
            var parts = s.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0 && double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedRatio))
            {
                ratio = parsedRatio;
            }

            if (parts.Length > 1 && double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedMin))
            {
                min = parsedMin;
            }

            if (parts.Length > 2 && double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedMax))
            {
                max = parsedMax;
            }
        }

        var size = width * ratio;
        return Math.Clamp(size, min, max);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
