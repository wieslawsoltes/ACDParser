using System.Collections.Generic;

namespace Acdparser;

public class DefineBranching : Define
{
    public List<Branch>? Branches { get; set; } = new();
}
