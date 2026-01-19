using Avalonia.Data.Converters;
using Avalonia.Media;
using IssueRunner.Models;
using System;
using System.Globalization;

namespace IssueRunner.Gui.Converters;

/// <summary>
/// Converts ChangeType enum to a color for display.
/// </summary>
public class ChangeTypeToColorConverter : IValueConverter
{
    public static readonly ChangeTypeToColorConverter Instance = new();
    
    // Cache brushes for performance
    private static readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Color.Parse("#333333"));
    private static readonly SolidColorBrush FixedBrush = new SolidColorBrush(Color.Parse("#4CAF50"));
    private static readonly SolidColorBrush RegressionBrush = new SolidColorBrush(Color.Parse("#F44336"));
    private static readonly SolidColorBrush CompileToFailBrush = new SolidColorBrush(Color.Parse("#FF9800"));
    private static readonly SolidColorBrush OtherBrush = new SolidColorBrush(Color.Parse("#9E9E9E"));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Always return a brush - never return null
        if (value is not ChangeType changeType || changeType == ChangeType.None)
        {
            return DefaultBrush;
        }

        return changeType switch
        {
            ChangeType.Fixed => FixedBrush,
            ChangeType.Regression => RegressionBrush,
            ChangeType.CompileToFail => CompileToFailBrush,
            ChangeType.Other => OtherBrush,
            _ => DefaultBrush
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
