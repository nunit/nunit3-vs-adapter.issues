using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace IssueRunner.Gui.Converters;

/// <summary>
/// Converts boolean to a color for display.
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    public static readonly BoolToColorConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue && boolValue)
        {
            return new SolidColorBrush(Color.Parse("#0078D4")); // Blue when true
        }
        return new SolidColorBrush(Color.Parse("#6C757D")); // Grey when false
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

