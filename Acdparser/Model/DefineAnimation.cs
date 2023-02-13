using System.Collections.Generic;

namespace Acdparser.Model;

public class DefineAnimation : Define
{
    public string? Name { get; set; }
    public int TransitionType { get; set; }
    public List<DefineFrame> Frames { get; set; } = new();
}
