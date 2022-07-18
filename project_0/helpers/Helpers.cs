using System;
using Microsoft.AspNetCore.Builder;
using Npgsql;
using Budget.Routes;

namespace Budget.Helpers
{
    public class HelperMethods
    {
        WebApplication serverApp;
        public void startWebServer(int port, NpgsqlConnection dbConn, string[] args)
        {
            try
            {
                ApiRoutes apiRoutes = new ApiRoutes(dbConn, args);
                WebApplication serverApp = apiRoutes.EstablishRoutes(dbConn, args) ?? throw new ArgumentNullException(nameof(serverApp));

                Console.WriteLine($"Listening on port {port}");
                serverApp.Run($"http://localhost:{port}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Threaded web server error: {ex}");
            }
        }

        public void DisplayInteractionMenu()
        {
            Console.WriteLine("\n Please type the number that cooresponds to your desired action: \n");
            Console.WriteLine("1. View total expenditure");
            Console.WriteLine("2. View all expense details");
            Console.WriteLine("3. Create a new expenditure");
            Console.WriteLine("4. Edit an existing expenditure");
            Console.WriteLine("5. Delete an expenditure");
            Console.WriteLine("6. Reset expenses");
            Console.WriteLine("7. View budget goal");
            Console.WriteLine("8. Set budget goal");
            Console.WriteLine("0. End \n");
        }
    }
}