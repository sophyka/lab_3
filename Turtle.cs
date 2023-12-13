using System.Globalization;
using System.Transactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using Newtonsoft.Json;

public class Turtle
{
    private float x;
    private float y;
    private int angle;
    private bool penDown;
    private string penColor;
    private List<Dot> dots;
    private List<Line> lines;
    private List<Figure> figures;
    private Figure currentFigure;
    private List<String> steps;

    private DatabaseManager databaseManager;

    public Turtle()
    {
        x = 0;
        y = 0;
        angle = 0;
        penDown = false;
        penColor = "black";
        dots = new List<Dot>();
        lines = new List<Line>();
        figures = new List<Figure>();
        currentFigure = new Figure();
        steps = new List<String>();
        databaseManager = new DatabaseManager("turtle_storage.sqlite");
    }
        
    private void AddDot(Dot dot)
    {
        if (!dots.Contains(dot))
        {
            dots.Add(dot);
        }
    }

    private void AddLine(Line line)
    {
        AddDot(line.StartDot);
        AddDot(line.EndDot);

        if (!line.StartDot.Equals(line.EndDot))
        {
            if (!lines.Contains(line))
            {
                lines.Add(line);
            }
        }
    }
    public void ProcessCommand(string command)
    {
        steps.Add($"[{command}] ");
        string[] parts = command.Split(' ');
        string action = parts[0].ToLower();

        switch (action)
        {
            case "pu":
                PenUp();
                break;
            case "pd":
                PenDown();
                break;
            case "angle":
                if (parts.Length == 2 && int.TryParse(parts[1], out int newAngle))
                {
                    ChangeAngle(newAngle);
                }
                else
                {
                    Console.WriteLine("‼ Invalid angle command. Usage: angle N ");
                }
                break;
            case "move":
                if (parts.Length == 2 && float.TryParse(parts[1], out float distance))
                {
                    Move(distance);
                }
                else
                {
                    Console.WriteLine("‼ Invalid move command. Usage: move N ");
                }
                break;
            case "color":
                if (parts.Length == 2)
                {
                    ChangePenColor(parts[1]);
                }
                else
                {
                    Console.WriteLine("‼ Invalid color command. Usage: color {colorName} ‼");
                }
                break;
            case "list":
                if (parts.Length ==2)
                {
                    switch(parts[1])
                    {
                        case "lines":
                            DisplayLines();
                            break;
                        case "figures":
                            DisplayFigures();
                            break;
                        case "steps":
                            int i = 0;
                            String message = "";
                            Console.WriteLine("►Your steps:");
                            foreach(var step in steps)
                            {
                                if (i<5)
                                {
                                    message+=step+" ";
                                    i++;
                                } else
                                {
                                    i=0;
                                    Console.WriteLine(message);
                                    message = "";
                                }
                            }
                            if (message != "")
                                Console.WriteLine(message);
                            break;
                        default:
                            Console.WriteLine("‼ Invalid param for list. Usage: list {lines | figures} ‼");
                            break;
                    }
                }
                break;
            case "save":
                if (parts.Length == 2)
                {
                    SaveToFile(parts[1]);
                }else
                {
                    Console.WriteLine("‼ Invalid param for save. Usage: save {xml | json | db} ‼");
                    break;
                }
                break;
            case "load":
                if (parts.Length == 2)
                {
                   LoadFromFile(parts[1]);
                }else
                {
                    Console.WriteLine("‼ Invalid param for load. Usage: load {xml | json | db} ‼");
                    break;
                }
                break;
            case "clear":
                if (parts.Length == 2)
                {
                    ClearFiles(parts[1]);
                }else
                {
                    Console.WriteLine("‼ Invalid clear command. Usage: clear files {xml | json | db} ‼");
                }
                break;
            default:
                Console.WriteLine("‼ Unknown command. Use commands from list ‼");
                break;
        }
    }

    public void PrintMenu(){
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║               ░░░░▒▒▒▓▓▌Turtle Graphics Simulator▐▓▓▒▒▒░░░░               ║");
        Console.WriteLine("╠═══════════════════════════════════════════════════════════════════════════╣");
        Console.WriteLine("║   ╟▌ Aviable commands ▐╢                                                  ║");
        Console.WriteLine("╠── move N ▼                                                                ║");
        Console.WriteLine("║    command to change turtle’s position on N steps.                        ║");
        Console.WriteLine("╠── angle N ▼                                                               ║");
        Console.WriteLine("║    command to change turtle’s angle of direction to N degrees.            ║");
        Console.WriteLine("╠── pd ▼                                                                    ║");
        Console.WriteLine("║    command to put down the pen.                                           ║");
        Console.WriteLine("╠── pu ▼                                                                    ║");
        Console.WriteLine("║    command to put up the pen.                                             ║");
        Console.WriteLine("╠── color {colorName} ▼                                                     ║");
        Console.WriteLine("║    command to change color of the pen to {colorName}. {black | green}     ║");
        Console.WriteLine("╠── list  {lines | steps} ▼                                                 ║");
        Console.WriteLine("║    command to list lines or steps.                                        ║");
        Console.WriteLine("╠── save  {xml | json | db} ▼                                               ║");
        Console.WriteLine("║    command for save to choosen format.                                    ║");
        Console.WriteLine("╠── load  {xml | json | db} ▼                                               ║");
        Console.WriteLine("║    command for load from choosen format.                                  ║");
        Console.WriteLine("╠── clear {xml | json | db} ▼                                               ║");
        Console.WriteLine("║    command for clear choosen storage.                                     ║");
        Console.WriteLine("╠── exit ▼                                                                  ║");
        Console.WriteLine("║    command to exit the program.                                           ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════╝\n");
    }
    private void PenUp()
    {
        penDown = false;
    }

    private void PenDown()
    {
        penDown = true;
    }

    private void ChangeAngle(int newAngle)
    {
        angle += newAngle;
        angle = angle % 360;
    }

    private void Move(float distance)
    {
        if (penDown)
        {
            float newX = (float)Math.Round(x + distance * (float)Math.Cos(angle * Math.PI / 180),2);
            float newY = (float)Math.Round(y + distance * (float)Math.Sin(angle * Math.PI / 180),2);

            Dot startDot = new Dot(x, y);
            Dot endDot = new Dot(newX, newY);
            Line line = new Line(startDot,endDot,penColor);

            currentFigure.AddLine(line);
            bool isIntersect = FindAllIntersections(line);
            if (isIntersect)
            {
                Figure newFigure = new Figure(currentFigure);
                figures.Add(newFigure);
                Console.WriteLine("Figure created!");
                currentFigure.Clear();
            }
            AddLine(line);
            
        }

        x += distance * (float)Math.Cos(angle * Math.PI / 180);
        y += distance * (float)Math.Sin(angle * Math.PI / 180);

        x = (float)Math.Round(x, 2);
        y = (float)Math.Round(y, 2);
    }

    private void ChangePenColor(string newColor)
    {
        penColor = newColor;
    }
    
    private void DisplayLines()
    {
        Console.WriteLine("► Lines:");
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }

    private void DisplayFigures()
    {
        Console.WriteLine("► Figures:");
        for(int i=0;i<figures.Count;i++)
        {
            Console.WriteLine($"[Figure {i+1}]");
            figures[i].Display();
        }
    }
    
    private bool IsIntersect(Line line1, Line line2)
    {

        float x1 = line1.StartDot.X;
        float y1 = line1.StartDot.Y;
        float x2 = line1.EndDot.X;
        float y2 = line1.EndDot.Y;
        float x3 = line2.StartDot.X;
        float y3 = line2.StartDot.Y;
        float x4 = line2.EndDot.X;
        float y4 = line2.EndDot.Y;

        float denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
        if (denominator == 0)
            return false;
        

        float x = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / denominator;
        float y = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / denominator;

        Dot intersection = new Dot(x, y);

        bool onLine1 = IsPointOnLine(intersection, line1.StartDot, line1.EndDot);
        bool onLine2 = IsPointOnLine(intersection, line2.StartDot, line2.EndDot);

        return onLine1 && onLine2;
    }

    private bool FindAllIntersections(Line currLine)
    {
        bool foundIntersection = false;
        for(int i=0;i<lines.Count-1;i++)
        {
            foundIntersection = IsIntersect(currLine,lines[i]);
            if (foundIntersection)
                break;
        }
        return foundIntersection;
    }
    private bool IsPointOnLine(Dot point, Dot lineStart, Dot lineEnd)
    {
        float minX = Math.Min(lineStart.X, lineEnd.X);
        float maxX = Math.Max(lineStart.X, lineEnd.X);
        float minY = Math.Min(lineStart.Y, lineEnd.Y);
        float maxY = Math.Max(lineStart.Y, lineEnd.Y);

        return point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY;
    }

    public void SaveToFile(string fileType)
    {
        TurtleStorage storage = new TurtleStorage
        {
            X = x,
            Y = y,
            Angle = angle,
            PenDown = penDown,
            PenColor = penColor,
            Dots = dots,
            Lines = lines,
            Figures = figures,
            CurrentFigure = currentFigure,
            Steps = steps
        };

        switch (fileType.ToLower())
        {
            case "xml":
                SaveToXml("storage.xml", storage);
                break;
            case "json":
                SaveToJson("storage.json", storage);
                break;
            case "db":
                SaveToDatabase(storage);
                break;
            default:
                Console.WriteLine($"Invalid storage type: {fileType}. Supported types: xml, json");
                break;
        }
    }

    public void LoadFromFile(string fileType)
    {
        TurtleStorage storage = new TurtleStorage();

        switch (fileType.ToLower())
        {
            case "xml":
                storage = LoadFromXml("storage.xml");
                break;
            case "json":
                storage = LoadFromJson("storage.json");
                break;
            case "db":
                storage = LoadFromDatabase();
                break;
            default:
                Console.WriteLine($"Invalid storage type: {fileType}. Supported types: xml, json");
                break;
        }

        if (storage != null)
        {
            x = storage.X;
            y = storage.Y;
            angle = storage.Angle;
            penDown = storage.PenDown;
            penColor = storage.PenColor;
            dots = storage.Dots;
            lines = storage.Lines;
            figures = storage.Figures;
            currentFigure = storage.CurrentFigure;
            steps = storage.Steps;
        }
    }

    private void SaveToXml(string fileName, TurtleStorage storage)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(TurtleStorage));
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            serializer.Serialize(writer, storage);
        }
        Console.WriteLine($"Data saved to {fileName} (XML).");
    }

    private TurtleStorage LoadFromXml(string fileName)
    {
        if (File.Exists(fileName))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TurtleStorage));
            using (StreamReader reader = new StreamReader(fileName))
            {
                return (TurtleStorage)serializer.Deserialize(reader);
            }
        }
        else
        {
            Console.WriteLine($"File {fileName} not found.");
            return null;
        }
    }

    private void SaveToJson(string fileName, TurtleStorage storage)
    {
        string json = JsonConvert.SerializeObject(storage, Formatting.Indented);
        File.WriteAllText(fileName, json);
        Console.WriteLine($"Data saved to {fileName} (JSON).");
    }

    private TurtleStorage LoadFromJson(string fileName)
    {
        if (File.Exists(fileName))
        {
            string json = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<TurtleStorage>(json);
        }
        else
        {
            Console.WriteLine($"File {fileName} not found.");
            return null;
        }
    }
    private void ClearFiles(string fileType)
    {
        switch (fileType.ToLower())
        {
            case "xml":
                ClearFile("storage.xml");
                break;
            case "json":
                ClearFile("storage.json");
                break;
            case "db":
                databaseManager.ClearDatabase();
                break;
            default:
                Console.WriteLine($"Invalid storage type: {fileType}. Supported types: xml, json");
                break;
        }
    }

    private void ClearFile(string fileName)
    {
        try
        {
            File.Delete(fileName);
            Console.WriteLine($"File {fileName} has been deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting storage {fileName}: {ex.Message}");
        }
    }

    public void SaveToDatabase(TurtleStorage storage)
    {
        databaseManager.SaveTurtleData(storage);
    }

    public TurtleStorage LoadFromDatabase()
    {
        TurtleStorage storage = databaseManager.LoadTurtleData();
        return storage;
    }

    public override string ToString()
    {
        string penState = penDown ? "put down" : "put up";
        return $"■ Current color: {penColor}, pen state: {penState}, location ({x}; {y}), angle: {angle} degrees. ■";
    }
}