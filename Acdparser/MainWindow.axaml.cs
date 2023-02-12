using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace Acdparser;

public class ImageConverter : IValueConverter
{
    public static ImageConverter Instance = new();

    public static string BasePath = "";

    private static Dictionary<string, SKBitmap?> ImageCache = new();

    private static SKBitmap? ImageLoader(string basePath, string fileName)
    {
        var imagePath = OperatingSystem.IsWindows() 
            ? $"{basePath}\\{fileName}" 
            : $"{basePath}/{fileName.Replace('\\', '/')}";
        if (File.Exists(imagePath))
        {
            using var stream = File.Open(imagePath, FileMode.Open);
            var bitmap = SKBitmap.Decode(stream);
            return bitmap;
        }

        return null;
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string fileName)
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
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ParseAsdFile(string path)
    {
        using var stream = File.OpenRead(path);
        var acd = AcdParser.ParseAcd(stream);
        if (acd is { })
        {
            var totalAnimations = acd.Animations.Count;
            var totalFrames = acd.Animations.SelectMany(x => x.Frames).Count();
            var totalImages = acd.Animations.SelectMany(x => x.Frames).SelectMany(x => x.Images).Count();

            DataContext = acd;

            Console.WriteLine($"animations: {totalAnimations}, frames: {totalFrames}, images: {totalImages}");
        }
    }

    private void ParseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        
        //var path = @"c:\Users\Administrator\Downloads\ACS\clippitMS\CLIPPIT ACS Decompiled\CLIPPIT.acd";
        var path = "/Users/wieslawsoltes/Documents/GitHub/Acdparser/clippitMS/CLIPPIT ACS Decompiled/CLIPPIT.acd";

        var basePath = Path.GetDirectoryName(path);
        if (basePath is { })
        {
            ImageConverter.BasePath = basePath;
        }
        
        ParseAsdFile(path);
    }

    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 1)
        {
            var item = e.AddedItems[0];
            Console.WriteLine($"{item}");
            if (item is TreeViewItem treeViewItem)
            {
                DefineContentControl.Content = treeViewItem.DataContext;
            }
            else if (item is Define define)
            {
                DefineContentControl.Content = define;
            }
        }
        else
        {
            DefineContentControl.Content = null;
        }
    }
}
