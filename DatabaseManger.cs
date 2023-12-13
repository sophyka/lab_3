using System.Data.SQLite;
using Newtonsoft.Json;

public class DatabaseManager
{
    private string connectionString;

    public DatabaseManager(string dbFilePath)
    {
        connectionString = $"Data Source={dbFilePath};Version=3;";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS TurtleData (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    X REAL,
                    Y REAL,
                    Angle INTEGER,
                    PenDown INTEGER,
                    PenColor TEXT,
                    Dots TEXT,
                    Lines TEXT,
                    Figures TEXT,
                    CurrentFigure TEXT,
                    Steps TEXT
                );";
            using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public void SaveTurtleData(TurtleStorage turtleData)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Подготовка данных для сохранения в виде строки JSON
            string dotsJson = JsonConvert.SerializeObject(turtleData.Dots);
            string linesJson = JsonConvert.SerializeObject(turtleData.Lines);
            string figuresJson = JsonConvert.SerializeObject(turtleData.Figures);
            string currentFigureJson = JsonConvert.SerializeObject(turtleData.CurrentFigure);
            string stepsJson = JsonConvert.SerializeObject(turtleData.Steps);

            // Вставка данных в таблицу
            string insertQuery = @"
                INSERT INTO TurtleData (X, Y, Angle, PenDown, PenColor, Dots, Lines, Figures, CurrentFigure, Steps)
                VALUES (@X, @Y, @Angle, @PenDown, @PenColor, @Dots, @Lines, @Figures, @CurrentFigure, @Steps);";
            using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@X", turtleData.X);
                command.Parameters.AddWithValue("@Y", turtleData.Y);
                command.Parameters.AddWithValue("@Angle", turtleData.Angle);
                command.Parameters.AddWithValue("@PenDown", turtleData.PenDown ? 1 : 0);
                command.Parameters.AddWithValue("@PenColor", turtleData.PenColor);
                command.Parameters.AddWithValue("@Dots", dotsJson);
                command.Parameters.AddWithValue("@Lines", linesJson);
                command.Parameters.AddWithValue("@Figures", figuresJson);
                command.Parameters.AddWithValue("@CurrentFigure", currentFigureJson);
                command.Parameters.AddWithValue("@Steps", stepsJson);

                command.ExecuteNonQuery();
            }
        }
    }

    public TurtleStorage LoadTurtleData()
    {
        TurtleStorage turtleData = new TurtleStorage();

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string selectQuery = "SELECT * FROM TurtleData ORDER BY Id DESC LIMIT 1;";
            using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        turtleData.X = Convert.ToSingle(reader["X"]);
                        turtleData.Y = Convert.ToSingle(reader["Y"]);
                        turtleData.Angle = Convert.ToInt32(reader["Angle"]);
                        turtleData.PenDown = Convert.ToInt32(reader["PenDown"]) == 1;
                        turtleData.PenColor = (string)reader["PenColor"];

                        turtleData.Dots = JsonConvert.DeserializeObject<List<Dot>>((string)reader["Dots"]);
                        turtleData.Lines = JsonConvert.DeserializeObject<List<Line>>((string)reader["Lines"]);
                        turtleData.Figures = JsonConvert.DeserializeObject<List<Figure>>((string)reader["Figures"]);
                        turtleData.CurrentFigure = JsonConvert.DeserializeObject<Figure>((string)reader["CurrentFigure"]);
                        turtleData.Steps = JsonConvert.DeserializeObject<List<string>>((string)reader["Steps"]);
                    }
                }
            }
        }

        return turtleData;
    }

    public void ClearDatabase()
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = "DELETE FROM TurtleData WHERE Id = 1";
                command.ExecuteNonQuery();
            }
        }
    }
}
