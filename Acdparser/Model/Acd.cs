using System.Collections.Generic;

namespace Acdparser;

public class Acd
{
    public DefineCharacter? Character { get; set; }
    public DefineBalloon? Balloon { get; set; }
    public List<DefineAnimation> Animations { get; set; } = new();
    public List<DefineState> States { get; set; } = new();
}
