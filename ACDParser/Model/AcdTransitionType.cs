namespace ACDParser.Model;

public enum AcdTransitionType
{
    /// <summary>
    /// Use return animation
    /// </summary>
    Return = 0x00,

    /// <summary>
    /// Use exit branches.
    /// </summary>
    Branching = 0x01,

    /// <summary>
    /// No transition.
    /// </summary>
    None = 0x02
}
