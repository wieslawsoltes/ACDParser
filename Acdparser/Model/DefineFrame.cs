using System.Collections.Generic;

namespace ACDParser.Model;

public class DefineFrame : Define
{
    public int Duration { get; set; }
    public int ExitBranch { get; set; }
    public string? SoundEffect { get; set; }
    public List<DefineImage> Images { get; set; } = new();
    public DefineBranching? Branching { get; set; }
}
