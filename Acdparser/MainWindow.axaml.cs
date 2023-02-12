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
