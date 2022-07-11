// probably does not need child classes and such
// really just asking for user input
using Microsoft.AspNetCore.Builder;
using System.Threading;
using System.Net.Http;
using System.Text.Json;
using Api;

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
                    Console.WriteLine("Viewing expenes total...");

                    thread.Start();
                    client.GetAsync($"http://localhost:3000/viewExpenseTotal");
                    break;

                case "2":
                    Console.WriteLine("Viewing all expenses...");
                    
                    fetchAllExpenseDetails();
                    break;

                case "3":
                    // combine the above into an object & serialize it for post request
                    Expense newExpense = new Expense();

                    Console.WriteLine("Creating a new expenditure. Type the relevant information...");

                    Console.WriteLine("Description:");
                    newExpense.Description = Console.ReadLine();

                    Console.WriteLine("Amount:");
                    newExpense.Amount = Convert.ToDouble(Console.ReadLine()); 
                    
                    Console.WriteLine("Category:");
                    newExpense.Category = Console.ReadLine();
                
                    Console.WriteLine("Date:");
                    newExpense.Date = Console.ReadLine();

                    // serializing newExpense
                    var content = JsonSerializer.Serialize(newExpense);
                    // wrapping JSON to enable adding it to body of request
                    StringContent stringContent = new StringContent(content);

                    thread.Start();
                    client.PostAsync($"http://localhost:3000/newExpense", stringContent);
                    break;

                case "4":
                    Console.WriteLine("Which expense would like to edit?");

                    fetchAllExpenseDetails();

                    Console.WriteLine("Type the id of the expense you would like to edit:");
                    string expenseToEdit = Console.ReadLine();
                    
                    try 
                    {
                        thread.Start();
                        client.GetAsync($"http://localhost:3000/editExpense/{expenseToEdit}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("The request failed. Double check the Id you submitted");
                    }

                    break;

                case "5":
                    Console.WriteLine("Which expense would like to delete?");

                    fetchAllExpenseDetails();

                    Console.WriteLine("Type the id of the expense you would like to edit:");
                    string expenseToDelete = Console.ReadLine();
                    
                    try 
                    {
                        thread.Start();
                        client.GetAsync($"http://localhost:3000/deleteExpense/{expenseToDelete}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("The request failed. Double check the Id you submitted");
                    }

                    break;

                case "6":
                    Console.WriteLine("Reseting expenses...");
                    
                    thread.Start();
                    client.GetAsync($"http://localhost:3000/resetExpense");
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

        private void fetchAllExpenseDetails()
        {
            Thread showCurrentExpensesThread = new Thread(() => startWebServer());
            showCurrentExpensesThread.Start();
            client.GetAsync($"http://localhost:3000/viewExpenseTotal");
        }
    } 
}