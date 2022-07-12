using Microsoft.AspNetCore.Builder;
using System.Text.Json;
using Api;
using Routes;
using Npgsql;

// contains the code for handling all user interaction with the console and the multi-threading required to both run a web server & interact with it simultaneously
namespace UserInteraction
{
    public class UserInput
    {
        bool exit = false;
        bool firstLoop = true;
        public WebApplication? app;
        NpgsqlConnection dbConn;
        // drilled down from main; required fro WebApplication.CreateBuilder()
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
                // app needs to be re-recreated for each loop since it is readonly after creation (and therefore cannot change the url)
                app = apiRoutes.EstablishRoutes(dbConn, args);

                HttpClient client = new HttpClient();

                if (firstLoop)
                {
                    DisplayInformation displayInfo = new DisplayInformation();
                    displayInfo.displayInteractionMenu();
                    firstLoop = false;
                }

                string? userAction = Console.ReadLine();
                handleUserInput(userAction, client);

                port++;
            }
        }

        private void handleUserInput(string? userInput, HttpClient client)
        {
            // create additional thread to run the web server (since it is blocking)
            Thread thread = new Thread(() => startWebServer());

            switch(userInput)
            {
                case "1":
                    Console.WriteLine("\n Viewing expenes total... \n");

                    thread.Start();
                    client.GetAsync($"http://localhost:{port}/viewExpenseTotal");
                    break;

                case "2":
                    Console.WriteLine("\n Viewing all expenses...\n");
                    
                    fetchAllExpenseDetails(client);
                    break;

                case "3":
                    Console.WriteLine("\n Creating a new expenditure. Type the relevant information... \n");

                    StringContent stringContent = gatherExpenseInfo();

                    thread.Start();
                    client.PostAsync($"http://localhost:{port}/newExpense", stringContent);
                    break;

                case "4":
                    Console.WriteLine("\n Which expense would like to edit? \n");

                    Console.WriteLine("\n Type the id of the expense you would like to edit: \n");
                    string? expenseToEditId = Console.ReadLine();

                    Console.WriteLine("\n Editing expenditure. Type the relevant information. \n");

                    StringContent editedStringContent = gatherExpenseInfo();
                    
                    try 
                    {
                        thread.Start();
                        client.PutAsync($"http://localhost:{port}/editExpense/{expenseToEditId}", editedStringContent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n The request failed: {ex} \n");
                    }

                    break;

                case "5":
                    Console.WriteLine("\n Which expense would like to delete? \n");

                    Console.WriteLine("\n Type the id of the expense you would like to delete: \n");
                    string? expenseToDelete = Console.ReadLine();
                    
                    try 
                    {
                        thread.Start();
                        client.DeleteAsync($"http://localhost:{port}/deleteExpense/{expenseToDelete}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n The request failed: {ex} \n");
                    }

                    break;

                case "6":
                    Console.WriteLine("\n Reseting expenses... \n");
                    
                    thread.Start();
                    client.DeleteAsync($"http://localhost:{port}/resetExpenses");
                    break;

                case "0":
                    Console.WriteLine("\n Terminating program... \n");
                    exit = true;
                    app?.StopAsync();
                    break;

                default:
                    Console.WriteLine("\n Command not recognized, please try again. \n");
                    break;
            }
        }

        private void startWebServer()
        {
            // open server connection; blocking
            app?.Run($"http://localhost:{port}");
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

    class DisplayInformation
    {
        public void displayInteractionMenu()
        {
            Console.WriteLine("\n Please type the number that cooresponds to your desired action: \n");
            Console.WriteLine("1. View total expenditure");
            Console.WriteLine("2. View all expense details");
            Console.WriteLine("3. Create a new expenditure");
            Console.WriteLine("4. Edit an existing expenditure");
            Console.WriteLine("5. Delete an expenditure");
            Console.WriteLine("6. Reset expenses");
            Console.WriteLine("0. End \n");
        }
    }
}