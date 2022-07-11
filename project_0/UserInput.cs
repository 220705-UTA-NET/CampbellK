// probably does not need child classes and such
// really just asking for user input
using Microsoft.AspNetCore.Builder;
using System.Threading;
using System.Net.Http;
using System.Text.Json;
using Api;
using Routes;
using Npgsql;

namespace UserInteraction
{
    public class UserInput
    {
        bool exit = false;
        public WebApplication app;
        NpgsqlConnection dbConn;
        string[] args;
        // other threads may take some time to shut down, so utilizing various ports to avoid conflict
        int port = 3000;

        public UserInput(NpgsqlConnection dbConn, string[] args)
        {
            this.dbConn = dbConn;
            this.args = args;
        }

        public void askUserInput()
        {
            while (!exit)
            {
                // establish server connection & routes
                ApiRoutes apiRoutes = new ApiRoutes();

                app = apiRoutes.EstablishRoutes(dbConn, args);

                // client to fire http requests
                HttpClient client = new HttpClient();

                Console.WriteLine("Please type the number that cooresponds to your desired action:");
                Console.WriteLine("1. View total expenditure");
                Console.WriteLine("2. View all expense details");
                Console.WriteLine("3. Create a new expenditure");
                Console.WriteLine("4. Edit an existing expenditure");
                Console.WriteLine("5. Delete an expenditure");
                Console.WriteLine("6. Reset expenses");
                Console.WriteLine("0. End");

                string? userAction = Console.ReadLine();
                handleUserInput(userAction, client);

                port++;
            }
        }

        private void handleUserInput(string userInput, HttpClient client)
        {
            // create additional thread to run the web server (since it is blocking)
            Thread thread = new Thread(() => startWebServer());

            switch(userInput)
            {
                case "1":
                    Console.WriteLine("Viewing expenes total...");

                    thread.Start();
                    client.GetAsync($"http://localhost:{port}/viewExpenseTotal");
                    break;

                case "2":
                    Console.WriteLine("Viewing all expenses...");
                    
                    fetchAllExpenseDetails(client);
                    break;

                case "3":
                    Console.WriteLine("Creating a new expenditure. Type the relevant information...");

                    StringContent stringContent = gatherExpenseInfo();

                    thread.Start();
                    client.PostAsync($"http://localhost:{port}/newExpense", stringContent);
                    break;

                case "4":
                    Console.WriteLine("Which expense would like to edit?");

                    fetchAllExpenseDetails(client);

                    Console.WriteLine("Type the id of the expense you would like to edit:");
                    string expenseToEditId = Console.ReadLine();

                    Console.WriteLine("Editing expenditure. Type the relevant information. If you wish to keep something the same, give a blank response");

                    StringContent editedStringContent = gatherExpenseInfo();
                    
                    try 
                    {
                        thread.Start();
                        client.PutAsync($"http://localhost:{port}/editExpense/{expenseToEditId}", editedStringContent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("The request failed. Double check the Id you submitted");
                    }

                    break;

                case "5":
                    Console.WriteLine("Which expense would like to delete?");

                    fetchAllExpenseDetails(client);

                    Console.WriteLine("Type the id of the expense you would like to edit:");
                    string expenseToDelete = Console.ReadLine();
                    
                    try 
                    {
                        thread.Start();
                        client.DeleteAsync($"http://localhost:{port}/deleteExpense/{expenseToDelete}");
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
                    client.GetAsync($"http://localhost:{port}/resetExpense");
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
            app.Run($"http://localhost:{port}");
        }

        private void fetchAllExpenseDetails(HttpClient client)
        {
            Thread showCurrentExpensesThread = new Thread(() => startWebServer());
            showCurrentExpensesThread.Start();
            client.GetAsync($"http://localhost:{port}/viewExpenseDetails");
        }

        private StringContent gatherExpenseInfo()
        {
            // combine the below responses into an object & serialize it for post request
            Expense editedInformation = new Expense();

            Console.WriteLine("Description:");
            editedInformation.Description = Console.ReadLine();

            Console.WriteLine("Amount:");
            editedInformation.Amount = Convert.ToDouble(Console.ReadLine()); 
            
            Console.WriteLine("Category:");
            editedInformation.Category = Console.ReadLine();
        
            Console.WriteLine("Date:");
            editedInformation.Date = Console.ReadLine();

            // serializing expense
            var editedContent = JsonSerializer.Serialize(editedInformation);
            // wrapping JSON to enable adding it to body of request
            StringContent editedStringContent = new StringContent(editedContent);

            return editedStringContent;
        }
    } 
}