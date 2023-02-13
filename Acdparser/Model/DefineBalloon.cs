namespace Acdparser.Model;

public class DefineBalloon : Define
{
    public int NumLines { get; set; }
    public int CharsPerLine { get; set; }
    public string? FontName { get; set; }
    public int FontHeight { get; set; }
    public string? ForeColor { get; set; }
    public string? BackColor { get; set; }
    public string? BorderColor { get; set; }
}
