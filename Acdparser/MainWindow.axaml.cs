using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Acdparser;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        if (OperatingSystem.IsWindows())
        {
            PathTextBox.Text = @"c:\Users\Administrator\Downloads\ACS\clippitMS\CLIPPIT ACS Decompiled\CLIPPIT.acd";
        }

        if (OperatingSystem.IsMacOS())
        {
            PathTextBox.Text = "/Users/wieslawsoltes/Documents/GitHub/Acdparser/clippitMS/CLIPPIT ACS Decompiled/CLIPPIT.acd";
        }
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

            var basePath = Path.GetDirectoryName(path);
            if (basePath is { })
            {
                ImageConverter.BasePath = basePath;
            }

            if (acd.Character is { })
            {
                var colorTableFileName = acd.Character.ColorTable;
                if (colorTableFileName is { })
                {
                    // TODO:
                    // var colorTable = ImageConverter.ToBitmap(colorTableFileName);
                }
            }

            DataContext = acd;

            Console.WriteLine($"animations: {totalAnimations}, frames: {totalFrames}, images: {totalImages}");
        }
    }

    private void ParseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var path = PathTextBox.Text;
        if (path is { })
        {
            ParseAsdFile(path);
        }
    }

    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 1)
        {
            var item = e.AddedItems[0];
            //Console.WriteLine($"{item}");
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
