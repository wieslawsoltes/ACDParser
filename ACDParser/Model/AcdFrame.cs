using System.Collections.Generic;

namespace ACDParser.Model;

public class AcdFrame : AcdBase
{
    public int Duration { get; set; }
    public int ExitBranch { get; set; }
    public string? SoundEffect { get; set; }
    public List<AcdImage> Images { get; set; } = new();
    public AcdBranching? Branching { get; set; }
}
