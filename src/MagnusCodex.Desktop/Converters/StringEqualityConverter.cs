using System.Globalization;
using Avalonia.Data.Converters;

namespace MagnusCodex.Desktop.Converters;

/// <summary>
/// Retorna true se o valor for igual ao parâmetro (string).
/// Usado para marcar RadioButtons com binding em coleções.
/// </summary>
public class StringEqualityConverter : IValueConverter
{
    public static readonly StringEqualityConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value?.ToString() == parameter?.ToString();

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Quando o RadioButton é marcado (IsChecked = true), devolve o parâmetro como valor
        if (value is true)
            return parameter?.ToString() ?? string.Empty;
        return Avalonia.Data.BindingOperations.DoNothing;
    }
}
