using System.Collections.Generic;

namespace ACDParser.Model;

public class Acd
{
    public AcdCharacter? Character { get; set; }
    public AcdBalloon? Balloon { get; set; }
    public List<AcdAnimation> Animations { get; set; } = new();
    public List<AcdState> States { get; set; } = new();
}
