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
        var bases = new Stack<AcdBase>();

        foreach (var line in lines)
        {
            var data = line.Trim(lineTrimChars);
            if (data.StartsWith("//"))
            {
                continue;
            }

            if (data.StartsWith("DefineCharacter"))
            {
                var character = new AcdCharacter();
                bases.Push(character);
            }
            else if (data.StartsWith("EndCharacter"))
            {
                var character = bases.Pop() as AcdCharacter;
                acd.Character = character;
            }
            else if (data.StartsWith("DefineInfo"))
            {
                var info = new AcdInfo();
                var index = "DefineInfo".Length + 1;
                var length = data.Length - index;
                var id = data.Substring(index, length);
                info.Id = id;
                bases.Push(info);
            }
            else if (data.StartsWith("EndInfo"))
            {
                if (bases.Pop() is AcdInfo info && bases.Peek() is AcdCharacter character)
                {
                    character.Infos.Add(info);
                }
            }
            else if (data.StartsWith("DefineBalloon"))
            {
                var balloon = new AcdBalloon();
                bases.Push(balloon);
            }
            else if (data.StartsWith("EndBalloon"))
            {
                var balloon = bases.Pop() as AcdBalloon;
                acd.Balloon = balloon;
            }
            else if (data.StartsWith("DefineAnimation"))
            {
                var animation = new AcdAnimation();
                var index = "DefineAnimation".Length + 1;
                var length = data.Length - index;
                var name = data.Substring(index, length).Trim('"');
                animation.Name = name;
                bases.Push(animation);
            }
            else if (data.StartsWith("EndAnimation"))
            {
                if (bases.Pop() is AcdAnimation animation)
                {
                    acd.Animations.Add(animation);
                }
            }
            else if (data.StartsWith("DefineFrame"))
            {
                var frame = new AcdFrame();
                bases.Push(frame);
            }
            else if (data.StartsWith("EndFrame"))
            {
                if (bases.Pop() is AcdFrame frame && bases.Peek() is AcdAnimation animation)
                {
                    animation.Frames.Add(frame);
                }
            }
            else if (data.StartsWith("DefineImage"))
            {
                var image = new AcdImage();
                bases.Push(image);
            }
            else if (data.StartsWith("EndImage"))
            {
                if (bases.Pop() is AcdImage image && bases.Peek() is AcdFrame frame)
                {
                    frame.Images.Add(image);
                }
            }
            else if (data.StartsWith("DefineBranching"))
            {
                var branching = new AcdBranching();
                bases.Push(branching);   
            }
            else if (data.StartsWith("EndBranching"))
            {
                if (bases.Pop() is AcdBranching branching && bases.Peek() is AcdFrame frame)
                {
                    frame.Branching = branching;
                }
            }
            else if (data.StartsWith("DefineState"))
            {
                var state = new AcdState();
                var index = "DefineState".Length + 1;
                var length = data.Length - index;
                var name = data.Substring(index, length).Trim('"');
                state.Name = name;
                bases.Push(state);  
            }
            else if (data.StartsWith("EndState"))
            {
                if (bases.Pop() is AcdState state)
                {
                    acd.States.Add(state);
                }
            }
            else
            {
                if (!ParseProperties(data, bases))
                {
                    return null;
                }
            }
        }

        acd.Animations.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Name, y.Name));

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

    private  static bool ParseProperties(string data, Stack<AcdBase> bases)
    {
        var acdBase = bases.Peek();
        var (key, value) = ParseKeyValue(data);
        if (key is null || value is null)
        {
            Console.WriteLine($"ERROR: {data}");
            return false;
        }

        switch (acdBase)
        {
            case AcdCharacter character:
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
                        // TODO:
                        break;
                    case "ColorTable":
                        character.ColorTable = value;
                        break;
                    default:
                        Console.WriteLine($"ERROR: Unknown key: {data}");
                        return false;
                }
                break;
            }
            case AcdInfo info:
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
                        Console.WriteLine($"ERROR: Unknown key: {data}");
                        return false;
                }
                break;
            }
            case AcdBalloon balloon:
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
                        Console.WriteLine($"ERROR: Unknown key: {data}");
                        return false;
                }
                break;
            }
            case AcdAnimation animation:
            {
                switch (key)
                {
                    case "TransitionType":
                    {
                        var transitionTypeByte = byte.Parse(value);
                        switch (transitionTypeByte)
                        {
                            case 0x00:
                                animation.AcdTransitionType = AcdTransitionType.Return;
                                break;
                            case 0x01:
                                animation.AcdTransitionType = AcdTransitionType.Branching;
                                break;
                            case 0x02:
                                animation.AcdTransitionType = AcdTransitionType.None;
                                break;
                            default:
                                Console.WriteLine($"Unknown TransitionType: {transitionTypeByte}");
                                break;
                        }
                        break;
                    }
                    default:
                        Console.WriteLine($"ERROR: Unknown key: {data}");
                        return false;
                }
                break;
            }
            case AcdFrame frame:
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
                        Console.WriteLine($"ERROR: Unknown key: {data}");
                        return false;
                }
                break;
            }
            case AcdImage image:
            {
                switch (key)
                {
                    case "Filename":
                        image.Filename = value;
                        break;
                    default:
                        Console.WriteLine($"ERROR: Unknown key: {data}");
                        return false;
                }
                break;
            }
            case AcdBranch _:
            {
                break;
            }
            case AcdBranching branching:
            {
                switch (key)
                {
                    case "BranchTo":
                        branching.Branches?.Add(new AcdBranch { BranchTo = int.Parse(value) });
                        break;
                    case "Probability":
                        if (branching.Branches is { })
                        {
                            branching.Branches[^1].Probability = int.Parse(value);
                        }
                        break;
                    default:
                        Console.WriteLine($"ERROR: Unknown key: {data}");
                        return false;
                }
                break;
            }
            case AcdState state:
            {
                switch (key)
                {
                    case "Animation":
                        state.Animations.Add(value);
                        break;
                    default:
                        Console.WriteLine($"ERROR: Unknown key: {data}");
                        return false;
                }
                break;
            }
            default:
            {
                Console.WriteLine($"ERROR: Unknown type: {data}");
                return false;
            }
        }

        return true;
    }
}
