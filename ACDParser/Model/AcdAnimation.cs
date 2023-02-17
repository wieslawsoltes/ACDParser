using System.Collections.Generic;

namespace ACDParser.Model;

public class AcdAnimation : AcdBase
{
    public string? Name { get; set; }
    public AcdTransitionType AcdTransitionType { get; set; }
    public List<AcdFrame> Frames { get; set; } = new();
}
