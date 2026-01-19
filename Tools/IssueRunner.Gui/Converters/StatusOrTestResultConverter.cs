using Avalonia.Data.Converters;
using IssueRunner.Gui.ViewModels;
using System;
using System.Globalization;

namespace IssueRunner.Gui.Converters;

/// <summary>
/// Converts IssueListItem to display StatusDisplay if available, otherwise TestResult.
/// </summary>
public sealed class StatusOrTestResultConverter : IValueConverter
{
    public static readonly StatusOrTestResultConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IssueListItem item)
        {
            return item.StatusDisplay ?? item.TestResult;
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
