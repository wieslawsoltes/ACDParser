using System;
using System.Collections.Generic;

namespace ACDParser.Model;

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
