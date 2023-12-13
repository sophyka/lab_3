using System.Collections.Generic;

[Serializable]
public class TurtleStorage
{
    public float X { get; set; }
    public float Y { get; set; }
    public int Angle { get; set; }
    public bool PenDown { get; set; }
    public string PenColor { get; set; }
    public List<Dot> Dots { get; set; }
    public List<Line> Lines { get; set; }
    public List<Figure> Figures { get; set; }
    public Figure CurrentFigure { get; set; }
    public List<string> Steps { get; set; }
}
