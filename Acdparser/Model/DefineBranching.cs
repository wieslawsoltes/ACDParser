using System.Collections.Generic;

namespace ACDParser.Model;

public class DefineBranching : Define
{
    public List<Branch>? Branches { get; set; } = new();
}
