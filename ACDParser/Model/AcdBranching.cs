using System.Collections.Generic;

namespace ACDParser.Model;

public class AcdBranching : AcdBase
{
    public List<AcdBranch>? Branches { get; set; } = new();
}
