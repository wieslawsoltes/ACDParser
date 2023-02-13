using System;
using Acdparser.Services;
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

    private void ParseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var path = PathTextBox.Text;
        if (path is { })
        {
            var acd = AcdLoader.Load(path);
            if (acd is { })
            {
                DataContext = acd;
            }
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
