using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Acdparser;

public class Acd
{
    public DefineCharacter? Character { get; set; }
    public DefineBalloon? Balloon { get; set; }
    public List<DefineAnimation> Animations { get; set; } = new();
    public List<DefineState> States { get; set; } = new();
}

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
    public string? SoundEffect { get; set; }
    public List<DefineImage> Images { get; set; } = new();
    public DefineBranching? Branching { get; set; }
}

public class DefineImage : Define
{
    public string? Filename { get; set; }
}

public class DefineBranching : Define
{
    public List<Branch>? Branches { get; set; } = new();
}

public class Branch : Define
{
    public int BranchTo { get; set; }
    public int Probability { get; set; }
}

public class DefineState : Define
{
    public string? Name { get; set; }
    public List<string> Animations { get; set; } = new();
}

public static class AcdParser
{
    public static Acd? ParseAcd(string path)
    {
        using var file = File.OpenText(path);
        var text = file.ReadToEnd();
        var lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var acd = new Acd();
        var defines = new Stack<Define>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim(new char[] { ' ', '\t' });

            if (trimmed.StartsWith("//"))
            {
                continue;
            }

            if (trimmed.StartsWith("DefineCharacter"))
            {
                var character = new DefineCharacter();
                defines.Push(character);
            }
            else if (trimmed.StartsWith("EndCharacter"))
            {
                var character = defines.Pop() as DefineCharacter;
                acd.Character = character;
            }
            else if (trimmed.StartsWith("DefineInfo"))
            {
                var info = new DefineInfo();
                var index = "DefineInfo".Length + 1;
                var length = trimmed.Length - index;
                var id = trimmed.Substring(index, length);
                info.Id = id;
                defines.Push(info);
            }
            else if (trimmed.StartsWith("EndInfo"))
            {
                var info = defines.Pop() as DefineInfo;
                var character = defines.Peek() as DefineCharacter;
                character.Infos.Add(info);
            }
            else if (trimmed.StartsWith("DefineBalloon"))
            {
                var balloon = new DefineBalloon();
                defines.Push(balloon);
            }
            else if (trimmed.StartsWith("EndBalloon"))
            {
                var balloon = defines.Pop() as DefineBalloon;
                acd.Balloon = balloon;
            }
            else if (trimmed.StartsWith("DefineAnimation"))
            {
                var animation = new DefineAnimation();
                defines.Push(animation);
            }
            else if (trimmed.StartsWith("EndAnimation"))
            {
                var animation = defines.Pop() as DefineAnimation;
                acd.Animations.Add(animation);
            }
            else if (trimmed.StartsWith("DefineFrame"))
            {
                var frame = new DefineFrame();
                defines.Push(frame);
            }
            else if (trimmed.StartsWith("EndFrame"))
            {
                var frame = defines.Pop() as DefineFrame;
                var animation = defines.Peek() as DefineAnimation;
                animation.Frames.Add(frame);
            }
            else if (trimmed.StartsWith("DefineImage"))
            {
                var image = new DefineImage();
                defines.Push(image);
            }
            else if (trimmed.StartsWith("EndImage"))
            {
                var image = defines.Pop() as DefineImage;
                var frame = defines.Peek() as DefineFrame;
                frame.Images.Add(image);
            }
            else if (trimmed.StartsWith("DefineBranching"))
            {
                var branching = new DefineBranching();
                defines.Push(branching);   
            }
            else if (trimmed.StartsWith("EndBranching"))
            {
                var branching = defines.Pop() as DefineBranching;
                var frame = defines.Peek() as DefineFrame;
                frame.Branching = branching;
            }
            else if (trimmed.StartsWith("DefineState"))
            {
                var state = new DefineState();
                var index = "DefineState".Length + 1;
                var length = trimmed.Length - index;
                var name = trimmed.Substring(index, length).Trim('"');
                state.Name = name;
                defines.Push(state);  
            }
            else if (trimmed.StartsWith("EndState"))
            {
                var state = defines.Pop() as DefineState;
                acd.States.Add(state);
            }
            else
            {
                var define = defines.Peek();

                static (string? Key, string? Value) Parse(string data)
                {
                    var eqIndex = data.IndexOf('=');
                    if (eqIndex < 0)
                    {
                        return (null, null);
                    }
                    var index = eqIndex + 2;
                    var length = data.Length - index;
                    var key = data.Substring(0, eqIndex - 1);
                    var value = data.Substring(index, length).Trim('"');
                    return (key, value);
                }

                var (key, value) = Parse(trimmed);
                if (key is null || value is null)
                {
                    Console.WriteLine($"ERROR: {trimmed}");
                    return null;
                }

                switch (define)
                {
                    case DefineCharacter character:
                    {
                        switch (key)
                        {
                            case "GUID":
                                character.GUID = Guid.Parse(value);
                                break;
                            case "Width":
                                character.Width = int.Parse(value);
                                break;
                            case "Height":
                                character.Height = int.Parse(value);
                                break;
                            case "Transparency":
                                character.Transparency = int.Parse(value);
                                break;
                            case "DefaultFrameDuration":
                                character.DefaultFrameDuration = int.Parse(value);
                                break;
                            case "Style":
                                // TOOD:
                                break;
                            case "ColorTable":
                                character.ColorTable = value;
                                break;
                            default:
                                Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                                return null;
                        }
                        break;
                    }
                    case DefineInfo info:
                    {
                        switch (key)
                        {
                            case "Name":
                                info.Name = value;
                                break;
                            case "Description":
                                info.Description = value;
                                break;
                            case "ExtraData":
                                info.ExtraData = value;
                                break;
                            default:
                                Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                                return null;
                        }
                        break;
                    }
                    case DefineBalloon balloon:
                    {
                        switch (key)
                        {
                            case "NumLines":
                                balloon.NumLines = int.Parse(value);
                                break;
                            case "CharsPerLine":
                                balloon.CharsPerLine = int.Parse(value);
                                break;
                            case "FontName":
                                balloon.FontName = value;
                                break;
                            case "FontHeight":
                                balloon.FontHeight = int.Parse(value);
                                break;
                            case "ForeColor":
                                balloon.ForeColor = value;
                                break;
                            case "BackColor":
                                balloon.BackColor = value;
                                break;
                            case "BorderColor":
                                balloon.BorderColor = value;
                                break;
                            default:
                                Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                                return null;
                        }
                        break;
                    }
                    case DefineAnimation animation:
                    {
                        switch (key)
                        {
                            case "TransitionType":
                                animation.TransitionType = int.Parse(value);
                                break;
                            default:
                                Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                                return null;
                        }
                        break;
                    }
                    case DefineFrame frame:
                    {
                        switch (key)
                        {
                            case "Duration":
                                frame.Duration = int.Parse(value);
                                break;
                            case "ExitBranch":
                                frame.ExitBranch = int.Parse(value);
                                break;
                            case "SoundEffect":
                                frame.SoundEffect = value;
                                break;
                            default:
                                Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                                return null;
                        }
                        break;
                    }
                    case DefineImage image:
                    {
                        switch (key)
                        {
                            case "Filename":
                                image.Filename = value;
                                break;
                            default:
                                Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                                return null;
                        }
                        break;
                    }
                    case DefineBranching branching:
                    {
                        switch (key)
                        {
                            case "BranchTo":
                                branching.Branches.Add(new Branch()
                                {
                                    BranchTo = int.Parse(value)
                                });
                                break;
                            case "Probability":
                                branching.Branches[^1].Probability = int.Parse(value);
                                break;
                            default:
                                Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                                return null;
                        }
                        break;
                    }
                    case DefineState state:
                    {
                        switch (key)
                        {
                            case "Animation":
                                state.Animations.Add(value);
                                break;
                            default:
                                Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                                return null;
                        }
                        break;
                    }
                    default:
                    {
                        Console.WriteLine($"ERROR: Unknown type: {trimmed}");
                        return null;
                    }
                }
            }
        }

        return acd;
    }
}

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
        var acd = AcdParser.ParseAcd(path);
        if (acd is { })
        {
            var totalAnimations = acd.Animations.Count;
            var totalFrames = acd.Animations.SelectMany(x => x.Frames).Count();
            var totalImages = acd.Animations.SelectMany(x => x.Frames).SelectMany(x => x.Images).Count();

            Console.WriteLine($"animations: {totalAnimations}, frames: {totalFrames}, images: {totalImages}");
        }
    }
}
