using System.Data.SQLite;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

public class DatabaseContext
{
    private readonly string _connectionString;

    public DatabaseContext(string connectionString)
    {
        _connectionString = connectionString;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            var command = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS Todos (Id INTEGER PRIMARY KEY AUTOINCREMENT, Task TEXT NOT NULL,Descriptions TEXT NOT NULL, IsCompleted INTEGER DEFAULT 0)",
                connection
            );
            command.ExecuteNonQuery();
        }
    }

    public List<Todo> GetTodos()
    {
        var todos = new List<Todo>();
        Console.WriteLine("Connecting to database...");

        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            Console.WriteLine("Database connected.");

            var command = new SQLiteCommand("SELECT * FROM Todos ORDER BY IsCompleted ASC", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var Task = reader.GetString(1);
                    var Descriptions = reader.GetString(2);
                    var IsCompleted = reader.GetInt32(3) == 1;

                    Console.WriteLine($"Retrieved: ID={id}, Task={Task},Descriptions={Descriptions}, IsCompleted={IsCompleted}");

                    todos.Add(new Todo
                    {
                        Id = id,
                        Task = Task,
                        Descriptions = Descriptions,
                        IsCompleted = IsCompleted
                    });
                }
            }
        }

        Console.WriteLine($"Total todos retrieved: {todos.Count}");
        return todos;
    }

    public void AddTodo(string Task,string Descriptions)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();

            var command = new SQLiteCommand(
                "INSERT INTO Todos (Task,Descriptions) VALUES (@Task,@Descriptions)",
                connection
            );
            command.Parameters.AddWithValue("@Task", Task );
            command.Parameters.AddWithValue("@Descriptions", Descriptions);
            command.ExecuteNonQuery();
        }
    }

    public void UpdateTodo(int Id, string Task, string Descriptions)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            var command = new SQLiteCommand(
                "UPDATE Todos SET Task = @Task, Descriptions = @Descriptions WHERE Id = @Id",
                connection
            );
            command.Parameters.AddWithValue("@Id", Id);
            command.Parameters.AddWithValue("@Task", Task);
            command.Parameters.AddWithValue("@Descriptions",Descriptions);
            command.ExecuteNonQuery();
        }
    }

    public int UpdateTodoStatus(int Id,string Task,string Descriptions, bool isCompleted)
    {
        try
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand(
                    "UPDATE Todos SET IsCompleted = @IsCompleted WHERE Id = @Id",
                    connection
                );

                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@Task", Task);
                command.Parameters.AddWithValue("@Descriptions", Descriptions);
                command.Parameters.AddWithValue("@IsCompleted", isCompleted);

                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"Rows affected: {rowsAffected}"); // Log the number of rows updated
                return rowsAffected;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating todo status: {ex.Message}");
            throw;
        }
    }

    public void DeleteTodo(int id)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            var command = new SQLiteCommand(
                "DELETE FROM Todos WHERE Id = @Id",
                connection
            );
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
    }


}

public class Todo
{
    public int Id { get; set; }

    [JsonPropertyName("Task")]
    public string Task { get; set; } = string.Empty;

    [JsonPropertyName("Descriptions")]
    public string Descriptions { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}