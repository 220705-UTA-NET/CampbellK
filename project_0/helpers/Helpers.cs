using System;
using Npgsql;
using Microsoft.AspNetCore.Builder;
using Budget.UserInteraction;

namespace Budget.Helpers
{
    class HelperMethods
    {
        // UserInput userInput;
        // public HelperMethods(NpgsqlConnection dbConn, string[] args)
        // {
        //     userInput = new UserInput(dbConn, args);
        // }

        // making port a parameter despite having a public port variable due to needing to offer different ports for the multiple threads that may be running outside of the above while loop
        public void startWebServer(int port, WebApplication app)
        {
            Console.WriteLine($"Listening on port {port}");
            app.Run($"http://localhost:{port}");
        }
    }
}