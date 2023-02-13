using System.Collections.Generic;

namespace Acdparser.Model;

public class DefineBranching : Define
{
    public List<Branch>? Branches { get; set; } = new();
}
