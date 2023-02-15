using System;
using System.Collections.Generic;
using System.IO;
using ACDParser.Model;

namespace ACDParser.Services;

public static class AcdParser
{
    public static Acd? ParseAcd(Stream stream)
    {
        var lineSeparators = new [] { '\r', '\n' };
        var lineTrimChars = new [] {' ', '\t'};

        using var reader = new StreamReader(stream);
        var text = reader.ReadToEnd();
        var lines = text.Split(lineSeparators, StringSplitOptions.RemoveEmptyEntries);

        var acd = new Acd();
        var defines = new Stack<Define>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim(lineTrimChars);
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
                if (info is { } && character is { })
                {
                    character.Infos.Add(info);
                }
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
                var index = "DefineAnimation".Length + 1;
                var length = trimmed.Length - index;
                var name = trimmed.Substring(index, length).Trim('"');
                animation.Name = name;
                defines.Push(animation);
            }
            else if (trimmed.StartsWith("EndAnimation"))
            {
                var animation = defines.Pop() as DefineAnimation;
                if (animation is { })
                {
                    acd.Animations.Add(animation);
                }
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
                if (frame is { } && animation is { })
                {
                    animation.Frames.Add(frame);
                }
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
                if (image is { } && frame is { })
                {
                    frame.Images.Add(image);
                }
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
                if (branching is { } && frame is { })
                {
                    frame.Branching = branching;
                }
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
                if (state is { })
                {
                    acd.States.Add(state);
                }
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
                                // TOOD:
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
                                return null;
                            }
                        }
                        break;
                    }
                    case DefineInfo info:
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
                                return null;
                            }
                        }
                        break;
                    }
                    case DefineBalloon balloon:
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
                                return null;
                            }
                        }
                        break;
                    }
                    case DefineAnimation animation:
                    {
                        switch (key)
                        {
                            case "TransitionType":
                            {
                                var transitionTypeByte = byte.Parse(value);
                                switch (transitionTypeByte)
                                {
                                    case 0x00:
                                    {
                                        animation.TransitionType = TransitionType.Return;
                                        break;
                                    }
                                    case 0x01:
                                    {
                                        animation.TransitionType = TransitionType.Branching;
                                        break;
                                    }
                                    case 0x02:
                                    {
                                        animation.TransitionType = TransitionType.None;
                                        break;
                                    }
                                    default:
                                    {
                                        Console.WriteLine($"Unknown TransitionType: {transitionTypeByte}");
                                        break;
                                    }
                                }
                                break;
                            }
                            default:
                            {
                                Console.WriteLine($"ERROR: Unknown key: {trimmed}");
                                return null;
                            }
                        }
                        break;
                    }
                    case DefineFrame frame:
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
                                return null;
                            }
                        }
                        break;
                    }
                    case DefineImage image:
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
                                return null;
                            }
                        }
                        break;
                    }
                    case DefineBranching branching:
                    {
                        switch (key)
                        {
                            case "BranchTo":
                            {
                                if (branching.Branches is { })
                                {
                                    branching.Branches.Add(new Branch() {BranchTo = int.Parse(value)});
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
                                return null;
                            }
                        }
                        break;
                    }
                    case DefineState state:
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
                                return null;
                            }
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
