using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Data.Converters;
using SkiaSharp;

namespace Acdparser;

public class ImageConverter : IValueConverter
{
    public static ImageConverter Instance = new();

    public static string BasePath = "";

    private static Dictionary<string, SKBitmap?> ImageCache = new();

    public static SKBitmap? ImageLoader(string basePath, string fileName)
    {
        var imagePath = GetImagePath(basePath, fileName);
        if (File.Exists(imagePath))
        {
            using var stream = File.Open(imagePath, FileMode.Open);
            var bitmap = SKBitmap.Decode(stream);
            // TODO: DefineCharacter.Transparency
            // TODO: DefineCharacter.ColorTable
            return bitmap;
        }

        return null;
    }

    private static string GetImagePath(string basePath, string fileName)
    {
        var imagePath = OperatingSystem.IsWindows()
            ? $"{basePath}\\{fileName}"
            : $"{basePath}/{fileName.Replace('\\', '/')}";
        return imagePath;
    }

    public static SKBitmap? ToBitmap(string fileName)
    {
        if (ImageCache.TryGetValue(fileName, out var bitmap))
        {
            return bitmap;
        }
        else
        {
            bitmap = ImageLoader(BasePath, fileName);
            if (bitmap is { })
            {
                ImageCache[fileName] = bitmap;
                return bitmap;
            }
        }

        return null;
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string fileName)
        {
            var bitmap = ToBitmap(fileName);
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
