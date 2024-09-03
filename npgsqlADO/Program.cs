using System;
using System.Data;
using Npgsql;

class Program
{
    // Connection string for your PostgreSQL container
    static string connectionString = "Host=localhost;Port=55432;Username=postgres;Password=mysecretpassword;Database=postgres";

    static void Main(string[] args)
    {
        try
        {
            // Create a connection
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Connected to PostgreSQL!");

                // Drop all tables and clear the database
                DropAllTables(connection);

                // Create a table
                CreateTable(connection);

                // Insert some data
                InsertData(connection);

                // Query and display data
                QueryData(connection);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static void DropAllTables(NpgsqlConnection connection)
    {
        Console.WriteLine("Dropping all tables and clearing the database...");

        // Get all table names
        string getTablesQuery = @"
            SELECT tablename FROM pg_tables 
            WHERE schemaname = 'public'";

        using (var command = new NpgsqlCommand(getTablesQuery, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                var tableNames = new List<string>();
                while (reader.Read())
                {
                    tableNames.Add(reader.GetString(0));
                }

                reader.Close();

                // Drop each table
                foreach (var tableName in tableNames)
                {
                    string dropTableQuery = $"DROP TABLE IF EXISTS {tableName} CASCADE";
                    using (var dropCommand = new NpgsqlCommand(dropTableQuery, connection))
                    {
                        dropCommand.ExecuteNonQuery();
                        Console.WriteLine($"Dropped table: {tableName}");
                    }
                }
            }
        }

        Console.WriteLine("All tables dropped and database cleared.");
    }

    static void CreateTable(NpgsqlConnection connection)
    {
        string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS users (
                id SERIAL PRIMARY KEY,
                name VARCHAR(100) NOT NULL,
                email VARCHAR(100) UNIQUE NOT NULL
            )";

        using (var command = new NpgsqlCommand(createTableQuery, connection))
        {
            command.ExecuteNonQuery();
            Console.WriteLine("Table 'users' created.");
        }
    }

    static void InsertData(NpgsqlConnection connection)
    {
        string insertDataQuery = @"
            INSERT INTO users (name, email) VALUES
            (@name1, @email1),
            (@name2, @email2)
            ON CONFLICT (email) DO NOTHING";

        using (var command = new NpgsqlCommand(insertDataQuery, connection))
        {
            command.Parameters.AddWithValue("name1", "John Doe");
            command.Parameters.AddWithValue("email1", "john@example.com");
            command.Parameters.AddWithValue("name2", "Jane Smith");
            command.Parameters.AddWithValue("email2", "jane@example.com");

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine($"{rowsAffected} row(s) inserted.");
        }
    }

    static void QueryData(NpgsqlConnection connection)
    {
        string queryDataQuery = "SELECT * FROM users";

        using (var command = new NpgsqlCommand(queryDataQuery, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                Console.WriteLine("\nUsers in the database:");
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string email = reader.GetString(2);
                    Console.WriteLine($"ID: {id}, Name: {name}, Email: {email}");
                }
            }
        }
    }
}
