using System;
using Npgsql;

// establishes database connection and returns *dbConn*, which is utilized by the API methods in Api namespace
namespace Budget.Database
{
    public class DbConnection
    {
            private static string? Host = System.Environment.GetEnvironmentVariable("postgres_host") ?? throw new ArgumentNullException(nameof(Host));
            private static string? Username = System.Environment.GetEnvironmentVariable("postgres_username") ?? throw new ArgumentNullException(nameof(Username));
            private static string? Database = System.Environment.GetEnvironmentVariable("postgres_database") ?? throw new ArgumentNullException(nameof(Database));
            private static string? Port = System.Environment.GetEnvironmentVariable("postgres_port") ?? throw new ArgumentNullException(nameof(Port));
            private static string? Password = System.Environment.GetEnvironmentVariable("postgres_password") ?? throw new ArgumentNullException(nameof(Password));

        public static NpgsqlConnection DbConnect()
        {
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