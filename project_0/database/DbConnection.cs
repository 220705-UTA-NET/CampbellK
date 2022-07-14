using System;
using Npgsql;

// establishes database connection and returns *dbConn*, which is utilized by the API methods in Api namespace
namespace Budget.Database
{
    public class DbConnection
    {
            private static string? Host = System.Environment.GetEnvironmentVariable("postgres_host");
            private static string? Username = System.Environment.GetEnvironmentVariable("postgres_username");
            private static string? Database = System.Environment.GetEnvironmentVariable("postgres_database");
            private static string? Port = System.Environment.GetEnvironmentVariable("postgres_port");
            private static string? Password = System.Environment.GetEnvironmentVariable("postgres_password");

        public NpgsqlConnection DbConnect()
        {
            if (Host == null || Username == null || Database == null || Port == null || Password == null) {
                throw new Exception("Failed to fetch all environmental variables");

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
                NpgsqlConnection dbConn = new NpgsqlConnection(connectionString);

                // open connection to db, enabling execution of queries
                dbConn.Open();

                return dbConn;
            }
        }
    }
}