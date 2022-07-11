// probably does not need child classes and such
// really just asking for user input
using Microsoft.AspNetCore.Builder;
using System.Threading;
using System.Net.Http;

namespace UserInteraction
{
    public class UserInput
    {
        bool exit = false;
        WebApplication app;

        // http request
        // https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/console-webapiclient
        // https://stackoverflow.com/questions/4015324/send-http-post-request-in-net

        private static readonly HttpClient client = new HttpClient();
        // var streamTask = client.GetStreamAsync("");
        // var response = await Json.DeserializeAsync<string>();

        public UserInput(WebApplication app)
        {
            this.app = app;
        }

        public void askUserInput()
        {
            while (!exit)
            {
                Console.WriteLine("Please type the number that cooresponds to your desired action:");
                Console.WriteLine("1. View all expense details");
                Console.WriteLine("2. View total expenditure");
                Console.WriteLine("3. Create a new expenditure");
                Console.WriteLine("4. Edit an existing expenditure");
                Console.WriteLine("5. Delete an expenditure");
                Console.WriteLine("6. Reset expenses");
                Console.WriteLine("0. End");

                string? userAction = Console.ReadLine();
                handleUserInput(userAction);
            }
        }

        private void handleUserInput(string userInput)
        {
            // create additional thread to run the web server (since it is blocking)
            Thread thread = new Thread(() => startWebServer());

            switch(userInput)
            {
                case "1":
                    Console.WriteLine("Viewing all expenses...");

                    thread.Start();
                    client.GetAsync($"http://localhost:3000/viewExpenseTotal");

                    break;
                case "2":
                    Console.WriteLine("Viewing total expenditure...");
                    
                    thread.Start();
                    client.GetAsync($"http://localhost:3000/viewExpenseDetails");

                    break;
                case "3":
                    Console.WriteLine("Creating a new expenditure...");
                    
                    thread.Start();
                    client.GetAsync($"http://localhost:3000/newExpense");

                    break;
                case "4":
                    Console.WriteLine("Editing an expenditure...");

                    // need to accept id as a param
                    string id = "";
                    
                    thread.Start();
                    client.GetAsync($"http://localhost:3000/editExpense/{id}");

                    break;
                case "5":
                    Console.WriteLine("Deleting an expenditure...");
                    // http request


                    break;

                case "6":
                    Console.WriteLine("Reseting expenses...");
                    // http request


                    break;

                case "0":
                    Console.WriteLine("Terminating program...");
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Command not recognized, please try again.");
                    break;
            }
        }

        private void startWebServer()
        {
            // open server connection
            app.Run("http://localhost:3000");
        }
    } 
}