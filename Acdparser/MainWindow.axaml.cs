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

    private void ParseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        //var path = @"c:\Users\Administrator\Downloads\ACS\clippitMS\CLIPPIT ACS Decompiled\CLIPPIT.acd";
        var path = "/Users/wieslawsoltes/Documents/GitHub/Acdparser/clippitMS/CLIPPIT ACS Decompiled/CLIPPIT.acd";
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
}
