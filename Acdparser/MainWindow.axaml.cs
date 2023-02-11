using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Acdparser;

public abstract class Define
{
}

[Flags]
public enum CharacterStyle
{
    AXS_VOICE_NONE,
    AXS_BALLOON_ROUNDRECT
}

public class DefineCharacter : Define
{
    public Guid? GUID { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Transparency { get; set; }
    public int DefaultFrameDuration { get; set; }
    public CharacterStyle Style { get; set; }
    public string? ColorTable { get; set; }
    public List<DefineInfo> Infos { get; set; } = new();
    public DefineBalloon? Balloon { get; set; }
    public List<DefineAnimation> Animations { get; set; } = new();
    public List<DefineState> States { get; set; } = new();
}

public class DefineInfo : Define
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ExtraData { get; set; }
}

public class DefineBalloon : Define
{
    public int NumLines { get; set; }
    public int CharsPerLine { get; set; }
    public string? FontName { get; set; }
    public int FontHeight { get; set; }
    public string? ForeColor { get; set; }
    public string? BackColor { get; set; }
    public string? BorderColor { get; set; }
}

public class DefineAnimation : Define
{
    public string? Name { get; set; }
    public int TransitionType { get; set; }
    public List<DefineFrame> Frames { get; set; } = new();
}

public class DefineFrame : Define
{
    public int Duration { get; set; }
    public int ExitBranch { get; set; }
    public DefineImage? Image { get; set; }
    public DefineBranching? Branching { get; set; }
}

public class DefineImage : Define
{
    public string? Filename { get; set; }
}

public class DefineBranching : Define
{
    public Branch? Branches { get; set; }
}

public class Branch : Define
{
    public int BranchTo { get; set; }
    public int Probability { get; set; }
}

public class DefineState : Define
{
    public List<string> Animations { get; set; } = new();
}

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ParseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var path = @"c:\Users\Administrator\Downloads\ACS\clippitMS\CLIPPIT ACS Decompiled\CLIPPIT.acd";

        var file = File.OpenText(path);

        var text = file.ReadToEnd();

        var lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        var defines = new Stack<Define>();

        foreach (var line in lines)
        {
            Console.WriteLine(line);

            var trimmed = line.Trim();

            var currentDefine = defines.Peek();

            switch (currentDefine)
            {
                case DefineCharacter character:
                {
                    break;
                }
                case DefineInfo info:
                {
                    break;
                }
                case DefineBalloon balloon:
                {
                    break;
                }
                case DefineAnimation animation:
                {
                    break;
                }
                case DefineFrame frame:
                {
                    break;
                }
                case DefineImage image:
                {
                    break;
                }
                case DefineBranching branching:
                {
                    break;
                }
                case DefineState state:
                {
                    break;
                }
            }

            if (trimmed.StartsWith("//"))
            {
                continue;
            }
            else if (trimmed.StartsWith("DefineCharacter"))
            {
                // GUID
                // Width
                // Height
                // Transparency
                // DefaultFrameDuration
                // Style
                // ColorTable
            }
            else if (trimmed.StartsWith("EndCharacter"))
            {
                
            }
            else if (trimmed.StartsWith("DefineInfo"))
            {

            }
            else if (trimmed.StartsWith("EndInfo"))
            {
                
            }
            else if (trimmed.StartsWith("DefineBalloon"))
            {
                
            }
            else if (trimmed.StartsWith("EndBalloon"))
            {
                
            }
            else if (trimmed.StartsWith("DefineAnimation"))
            {
                
            }
            else if (trimmed.StartsWith("EndAnimation"))
            {
                
            }
            else if (trimmed.StartsWith("DefineFrame"))
            {
                
            }
            else if (trimmed.StartsWith("EndFrame"))
            {
                
            }
            else if (trimmed.StartsWith("DefineImage"))
            {
                
            }
            else if (trimmed.StartsWith("EndImage"))
            {
                
            }
        }
    }
}