using System;
using Npgsql;

namespace database
{
    public class DbConnection
    {
            // static are scoped to class rather than object instance, therefore cannot declare them within DbConnect()
            private static string? Host = System.Environment.GetEnvironmentVariable("postgres_host");
            private static string? Username = System.Environment.GetEnvironmentVariable("postgres_username");
            private static string? Database = System.Environment.GetEnvironmentVariable("postgres_database");
            private static string? Port = System.Environment.GetEnvironmentVariable("postgres_port");
            private static string? Password = System.Environment.GetEnvironmentVariable("postgres_password");

        public void DbConnect()
        {
            if (Host == null || Username == null || Database == null || Port == null || Password == null) {
                Console.WriteLine("Error retrieving environmental variables");
                return;
            } else {
        
                Console.WriteLine("Connecting to postgresql...");

                string connectionString = String.Format(
                    "server={0};Username={1};Database={2};Port={3};Password={4}",
                    Host,
                    Username,
                    Database,
                    Port,
                    Password
                );

                // establishing connection to postgresql
                NpgsqlConnection conn = new NpgsqlConnection(connectionString);

                // open connection to db, enabling execution of queries
                conn.Open();

                // check what the user wants to do
                // ** how to allow the user to continuously revisit? **
                // constantly utilize Console.ReadLine() at the end of each endpoint
                string? userCommand = Console.ReadLine();

                // will want to set up a prepared statement
                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM budget", conn);

                // execute query and save results 
                NpgsqlDataReader reader = command.ExecuteReader();

                // list where table data will be saved
                List<Dictionary<string, string>> listOfEntries = new List<Dictionary<string, string>>();

                while (reader.Read())
                {
                    string? id = reader["id"].ToString();
                    string? description = reader["description"].ToString();
                    string? amount = reader["amount"].ToString();
                    string? category = reader["category"].ToString();
                    string? date = reader["date"].ToString();

                    // combine the row data into a single struct to save to listOfEntries;
                    Dictionary<string, string> budgetEntry = new Dictionary<string, string>()
                    {
                        {"id", id},
                        {"description", description},
                        {"amount", amount},
                        {"category", category},
                        {"date", date}
                    };

                    listOfEntries.Add(budgetEntry);
                }

                Console.WriteLine(listOfEntries[0]["id"]);

                // end the reader
                reader.Close();
                // discard the command
                command.Dispose();
                // close the db connection
                conn.Close();
            }
        }
    }
}