using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace IssueRunner.Gui.Converters;

/// <summary>
/// Converts boolean to text for display.
/// </summary>
public class BoolToTextConverter : IValueConverter
{
    public static readonly BoolToTextConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            // When ShowDiffOnly is false, show "Diff"
            // When ShowDiffOnly is true, show "Normal"
            return boolValue ? "Normal" : "Diff";
        }
        return "Diff"; // Default
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

