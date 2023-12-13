[Serializable]
public class Dot
{
    public float X { get; set; }
    public float Y { get; set; }

    public Dot(float x, float y)
    {
        X = x;
        Y = y;
    }
    public Dot()
    {}
    public Dot(Dot obj)
    {
        X=obj.X;
        Y=obj.Y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Dot otherDot = (Dot)obj;
        return X == otherDot.X && Y == otherDot.Y;
    }

    public override int GetHashCode()
    {
        return Tuple.Create(X, Y).GetHashCode();
    }

    public override string ToString(){
        return $"({X};{Y})";
    }
}