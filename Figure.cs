[Serializable]
public class Figure
{
    public List<Line> Lines { get; set;}

    public Figure()
    {
        Lines = new List<Line>();
    }

    public Figure(Figure obj)
    {
        Lines = new List<Line>(obj.Lines);
    }

    public void AddLine(Line line)
    {   
        Lines.Add(line);
    }

    public void Clear()
    {
        Lines.Clear();
    }

    public List<Line> GetLines()
    {
        return Lines;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Figure otherFigure = (Figure)obj;

        // Проверяем, что все линии у фигур совпадают
        foreach (var line in Lines)
        {
            if (!otherFigure.Lines.Contains(line))
                return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        int hashCode = 17;

        // Используем XOR для комбинирования хэш-кодов линий
        foreach (var line in Lines)
        {
            hashCode = hashCode * 23 + line.GetHashCode();
        }

        return hashCode;
    }

    public void Display()
    {
        foreach (var line in Lines)
        {
            Console.WriteLine(line);
        }
    }
}