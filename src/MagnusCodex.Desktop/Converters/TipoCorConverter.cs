using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MagnusCodex.Desktop.Converters;

/// <summary>
/// Converte o tipo de carta em uma cor de fundo para o badge.
/// Forma = roxo, Transmutação = verde, Modificador = laranja.
/// </summary>
public class TipoCorConverter : IValueConverter
{
    public static readonly TipoCorConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "Forma"        => new SolidColorBrush(Color.Parse("#7C3AED")),
            "Transmutação" => new SolidColorBrush(Color.Parse("#059669")),
            "Modificador"  => new SolidColorBrush(Color.Parse("#D97706")),
            _              => new SolidColorBrush(Color.Parse("#585B70"))
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
