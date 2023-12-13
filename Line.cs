[Serializable]
public class Line
{
    public Dot StartDot { get; set; }
    public Dot EndDot { get; set; }
    public string Color { get; set; }

    public Line(Dot startDot, Dot endDot, string color)
    {
        StartDot = startDot;
        EndDot = endDot;
        Color = color;
    }
    public Line()
    {}
    public Line(Line obj)
    {
        StartDot = new Dot(obj.StartDot);
        EndDot = new Dot(obj.EndDot);
        Color = obj.Color;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Line otherLine = (Line)obj;
        return ((StartDot.Equals(otherLine.StartDot) && EndDot.Equals(otherLine.EndDot)) || (StartDot.Equals(otherLine.EndDot) && EndDot.Equals(otherLine.StartDot)));
    }

    public override int GetHashCode()
    {
        return Tuple.Create(StartDot, EndDot, Color).GetHashCode();
    }

    public override string ToString(){
        return $"({StartDot.X.ToString().PadLeft(3)}:{StartDot.Y.ToString().PadLeft(3)})--->({EndDot.X.ToString().PadLeft(3)}:{EndDot.Y.ToString().PadLeft(3)}) | Color: {Color}";
    }
}