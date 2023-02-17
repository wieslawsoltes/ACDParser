using System;
using System.Collections.Generic;
using System.IO;
using ACDParser.Model;

namespace ACDParser.Services;

public static class AcdParser
{
    public  static Acd? ParseStream(Stream stream)
    {
        var lineSeparators = new [] { '\r', '\n' };
        var lineTrimChars = new [] {' ', '\t'};

        using var reader = new StreamReader(stream);
        var text = reader.ReadToEnd();
        var lines = text.Split(lineSeparators, StringSplitOptions.RemoveEmptyEntries);

        var acd = new Acd();
        var defines = new Stack<AcdBase>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim(lineTrimChars);
            if (trimmed.StartsWith("//"))
            {
                continue;
            }

            if (trimmed.StartsWith("AcdCharacter"))
            {
                var character = new AcdCharacter();
                defines.Push(character);
            }
            else if (trimmed.StartsWith("EndCharacter"))
            {
                var character = defines.Pop() as AcdCharacter;
                acd.Character = character;
            }
            else if (trimmed.StartsWith("AcdInfo"))
            {
                var info = new AcdInfo();
                var index = "AcdInfo".Length + 1;
                var length = trimmed.Length - index;
                var id = trimmed.Substring(index, length);
                info.Id = id;
                defines.Push(info);
            }
            else if (trimmed.StartsWith("EndInfo"))
            {
                var info = defines.Pop() as AcdInfo;
                var character = defines.Peek() as AcdCharacter;
                if (info is { } && character is { })
                {
                    character.Infos.Add(info);
                }
            }
            else if (trimmed.StartsWith("AcdBalloon"))
            {
                var balloon = new AcdBalloon();
                defines.Push(balloon);
            }
            else if (trimmed.StartsWith("EndBalloon"))
            {
                var balloon = defines.Pop() as AcdBalloon;
                acd.Balloon = balloon;
            }
            else if (trimmed.StartsWith("AcdAnimation"))
            {
                var animation = new AcdAnimation();
                var index = "AcdAnimation".Length + 1;
                var length = trimmed.Length - index;
                var name = trimmed.Substring(index, length).Trim('"');
                animation.Name = name;
                defines.Push(animation);
            }
            else if (trimmed.StartsWith("EndAnimation"))
            {
                var animation = defines.Pop() as AcdAnimation;
                if (animation is { })
                {
                    acd.Animations.Add(animation);
                }
            }
            else if (trimmed.StartsWith("AcdFrame"))
            {
                var frame = new AcdFrame();
                defines.Push(frame);
            }
            else if (trimmed.StartsWith("EndFrame"))
            {
                var frame = defines.Pop() as AcdFrame;
                var animation = defines.Peek() as AcdAnimation;
                if (frame is { } && animation is { })
                {
                    animation.Frames.Add(frame);
                }
            }
            else if (trimmed.StartsWith("AcdImage"))
            {
                var image = new AcdImage();
                defines.Push(image);
            }
            else if (trimmed.StartsWith("EndImage"))
            {
                var image = defines.Pop() as AcdImage;
                var frame = defines.Peek() as AcdFrame;
                if (image is { } && frame is { })
                {
                    frame.Images.Add(image);
                }
            }
            else if (trimmed.StartsWith("AcdBranching"))
            {
                var branching = new AcdBranching();
                defines.Push(branching);   
            }
            else if (trimmed.StartsWith("EndBranching"))
            {
                var branching = defines.Pop() as AcdBranching;
                var frame = defines.Peek() as AcdFrame;
                if (branching is { } && frame is { })
                {
                    frame.Branching = branching;
                }
            }
            else if (trimmed.StartsWith("AcdState"))
            {
                var state = new AcdState();
                var index = "AcdState".Length + 1;
                var length = trimmed.Length - index;
                var name = trimmed.Substring(index, length).Trim('"');
                state.Name = name;
                defines.Push(state);  
            }
            else if (trimmed.StartsWith("EndState"))
            {
                var state = defines.Pop() as AcdState;
                if (state is { })
                {
                    acd.States.Add(state);
                }
            }
            else
            {
                if (!ParseProperties(trimmed, defines))
                {
                    return null;
                }
            }
        }

        return acd;
    }

    private static (string? Key, string? Value) ParseKeyValue(string data)
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

    private  static bool ParseProperties(string trimmed, Stack<AcdBase> defines)
    {
        var acdBase = defines.Peek();

        var (key, value) = ParseKeyValue(trimmed);
        if (key is null || value is null)
        {
            Console.WriteLine($"ERROR: {trimmed}");
            return false;
        }

        switch (acdBase)
        {
            case AcdCharacter character:
            {
                switch (key)
                {
                    case "GUID":
                    {
                        character.GUID = Guid.Parse(value);
                        break;
                    }
                    case "Width":
                    {
                        character.Width = int.Parse(value);
                        break;
                    }
                    case "Height":
                    {
                        character.Height = int.Parse(value);
                        break;
                    }
                    case "Transparency":
                    {
                        character.Transparency = int.Parse(value);
                        break;
                    }
                    case "DefaultFrameDuration":
                    {
                        character.DefaultFrameDuration = int.Parse(value);
                        break;
                    }
                    case "Style":
                    {
                        // TODO:
                        break;
                    }
                    case "ColorTable":
                    {
                        character.ColorTable = value;
                        break;
                    }
                    default:
                    {
                        Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                        return false;
                    }
                }
                break;
            }
            case AcdInfo info:
            {
                switch (key)
                {
                    case "Name":
                    {
                        info.Name = value;
                        break;
                    }
                    case "Description":
                    {
                        info.Description = value;
                        break;
                    }
                    case "ExtraData":
                    {
                        info.ExtraData = value;
                        break;
                    }
                    default:
                    {
                        Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                        return false;
                    }
                }
                break;
            }
            case AcdBalloon balloon:
            {
                switch (key)
                {
                    case "NumLines":
                    {
                        balloon.NumLines = int.Parse(value);
                        break;
                    }
                    case "CharsPerLine":
                    {
                        balloon.CharsPerLine = int.Parse(value);
                        break;
                    }
                    case "FontName":
                    {
                        balloon.FontName = value;
                        break;
                    }
                    case "FontHeight":
                    {
                        balloon.FontHeight = int.Parse(value);
                        break;
                    }
                    case "ForeColor":
                    {
                        balloon.ForeColor = value;
                        break;
                    }
                    case "BackColor":
                    {
                        balloon.BackColor = value;
                        break;
                    }
                    case "BorderColor":
                    {
                        balloon.BorderColor = value;
                        break;
                    }
                    default:
                    {
                        Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                        return false;
                    }
                }
                break;
            }
            case AcdAnimation animation:
            {
                switch (key)
                {
                    case "AcdTransitionType":
                    {
                        var transitionTypeByte = byte.Parse(value);
                        switch (transitionTypeByte)
                        {
                            case 0x00:
                            {
                                animation.AcdTransitionType = AcdTransitionType.Return;
                                break;
                            }
                            case 0x01:
                            {
                                animation.AcdTransitionType = AcdTransitionType.Branching;
                                break;
                            }
                            case 0x02:
                            {
                                animation.AcdTransitionType = AcdTransitionType.None;
                                break;
                            }
                            default:
                            {
                                Console.WriteLine($"Unknown AcdTransitionType: {transitionTypeByte}");
                                break;
                            }
                        }
                        break;
                    }
                    default:
                    {
                        Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                        return false;
                    }
                }
                break;
            }
            case AcdFrame frame:
            {
                switch (key)
                {
                    case "Duration":
                    {
                        frame.Duration = int.Parse(value);
                        break;
                    }
                    case "ExitBranch":
                    {
                        frame.ExitBranch = int.Parse(value);
                        break;
                    }
                    case "SoundEffect":
                    {
                        frame.SoundEffect = value;
                        break;
                    }
                    default:
                    {
                        Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                        return false;
                    }
                }
                break;
            }
            case AcdImage image:
            {
                switch (key)
                {
                    case "Filename":
                    {
                        image.Filename = value;
                        break;
                    }
                    default:
                    {
                        Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                        return false;
                    }
                }
                break;
            }
            case AcdBranching branching:
            {
                switch (key)
                {
                    case "BranchTo":
                    {
                        if (branching.Branches is { })
                        {
                            branching.Branches.Add(new AcdBranch() {BranchTo = int.Parse(value)});
                        }
                        break;
                    }
                    case "Probability":
                    {
                        if (branching.Branches is { })
                        {
                            branching.Branches[^1].Probability = int.Parse(value);
                        }

                        break;
                    }
                    default:
                    {
                        Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                        return false;
                    }
                }
                break;
            }
            case AcdState state:
            {
                switch (key)
                {
                    case "Animation":
                    {
                        state.Animations.Add(value);
                        break;
                    }
                    default:
                    {
                        Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                        return false;
                    }
                }
                break;
            }
            default:
            {
                Console.WriteLine($"ERROR: Unknown type: {trimmed}");
                return false;
            }
        }

        return true;
    }
}
