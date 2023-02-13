using System.Collections.Generic;

namespace Acdparser.Model;

public class DefineState : Define
{
    public string? Name { get; set; }
    public List<string> Animations { get; set; } = new();
}
