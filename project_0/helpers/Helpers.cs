using System;
using Npgsql;
using Microsoft.AspNetCore.Builder;

namespace Budget.Helpers
{
    public class HelperMethods
    {
        // making port a parameter despite having a public port variable in UserInput.cs due to needing to offer different ports for the multiple threads that may be running outside of the above while loop
        // referencing the namespace variable is not always as up-to-date as it should be; passing it as a param has shown more consistent results
        public void startWebServer(int port, WebApplication app)
        {
            Console.WriteLine($"Listening on port {port}");
            app.Run($"http://localhost:{port}");
        }

        public void displayInteractionMenu()
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