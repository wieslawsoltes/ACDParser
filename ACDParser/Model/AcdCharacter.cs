using System;
using System.Collections.Generic;

namespace ACDParser.Model;

public class AcdCharacter : AcdBase
{
    public Guid? GUID { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Transparency { get; set; }
    public int DefaultFrameDuration { get; set; }
    public AcdCharacterStyle Style { get; set; }
    public string? ColorTable { get; set; }
    public List<AcdInfo> Infos { get; set; } = new();
}
