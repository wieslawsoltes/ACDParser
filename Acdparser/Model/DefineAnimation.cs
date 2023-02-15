using System.Collections.Generic;

namespace ACDParser.Model;

public class DefineAnimation : Define
{
    public string? Name { get; set; }
    public TransitionType TransitionType { get; set; }
    public List<DefineFrame> Frames { get; set; } = new();
}
