using System;
using System.Globalization;
using Acdparser.Services;
using Avalonia;
using Avalonia.Data.Converters;

namespace Acdparser;

public class ImageConverter : IValueConverter
{
    public static ImageConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string fileName)
        {
            var bitmap = ImageLoader.ToBitmap(fileName);
            if (bitmap is { })
            {
                return bitmap;
            }
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
