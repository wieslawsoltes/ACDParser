using System.Collections.Generic;

namespace ACDParser.Model;

public class AcdState : AcdBase
{
    public string? Name { get; set; }
    public List<string> Animations { get; set; } = new();
}
