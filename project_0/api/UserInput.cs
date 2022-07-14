using Microsoft.AspNetCore.Builder;
using System.Text.Json;
using RouteMethods;
using Routes;
using Npgsql;
using Tracking;

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
        public int port = 3000;
        // allows us to read & write to budget.json, where totalExpense & budgetGoal are stored
        public BudgetTracking budgetTracker = new BudgetTracking();
        // for displaying the console menu to user
        public DisplayInformation displayInfo = new DisplayInformation();

        public UserInput(NpgsqlConnection dbConn, string[] args)
        {
            this.dbConn = dbConn;
            this.args = args;
        }

        public void askUserInput()
        {
            try
            {
                while (!exit)
                {
                    port++;
                    // establish server connection & routes
                    ApiRoutes apiRoutes = new ApiRoutes();
                    // app needs to be re-recreated for each loop since it is readonly after creation (and therefore cannot change the http url)
                    app = apiRoutes.EstablishRoutes(dbConn, args);

                    HttpClient client = new HttpClient();

                    if (firstLoop)
                    {
                        // for first iteration, display interaction menu
                        displayInfo.displayInteractionMenu();
                        firstLoop = false;
                    }

                    string? userAction = Console.ReadLine();
                    handleUserInput(userAction, client);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed during user input: {ex}");
            }
        }

        async private void handleUserInput(string? userInput, HttpClient client)
        {
            // create additional thread to run the web server (since it is blocking)
            Thread thread = new Thread(() => startWebServer());

            switch(userInput)
            {
                case "1":
                    thread.Start();
                    await client.GetAsync($"http://localhost:{port}/viewExpenseTotal");

                    break;

                case "2":                    
                    thread.Start();
                    await client.GetAsync($"http://localhost:{port}/viewExpenseDetails");

                    break;

                case "3":
                    Console.WriteLine("\n Creating a new expenditure. Type the relevant information... \n");

                    StringContent stringContent = gatherExpenseInfo();

                    thread.Start();
                    await client.PostAsync($"http://localhost:{port}/newExpense", stringContent);

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
                        await client.PutAsync($"http://localhost:{port}/editExpense/{expenseToEditId}", editedStringContent);
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
                        await client.DeleteAsync($"http://localhost:{port}/deleteExpense/{expenseToDelete}");
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
                    await client.DeleteAsync($"http://localhost:{port}/resetExpenses");
                    break;
                
                case "7":
                    Dictionary<string, string> priorBudget = budgetTracker.getBudgetAndExpense();

                    Console.WriteLine("\n --------------------------------------- \n");
                    Console.WriteLine("Current budget:");
                    Console.WriteLine(priorBudget["currentBudget"]);
                    Console.WriteLine("\n --------------------------------------- \n");

                    displayInfo.displayInteractionMenu();

                    break;
                
                case "8":
                    Console.WriteLine("\n Type your new budget goal: \n");
                    string? budgetGoal = Console.ReadLine();

                    Dictionary<string, string> previousBudget = budgetTracker.getBudgetAndExpense();

                    // update expenseTotal & re-write the budget.json content
                    previousBudget["currentBudget"] = budgetGoal;

                    var serializedUpdatedBudget = JsonSerializer.Serialize(previousBudget);
                    File.WriteAllText("./budget.json", serializedUpdatedBudget);

                    Console.WriteLine("\n --------------------------------------- \n");
                    Console.WriteLine("Budget goal set");
                    Console.WriteLine("\n --------------------------------------- \n");

                    displayInfo.displayInteractionMenu();
                    break;

                case "0":
                    Console.WriteLine("\n --------------------------------------- \n");
                    Console.WriteLine("\n Terminating program... \n");
                    Console.WriteLine("\n --------------------------------------- \n");
                    exit = true;
                    app?.StopAsync();
                    break;

                default:
                    Console.WriteLine("\n --------------------------------------- \n");
                    Console.WriteLine("\n Command not recognized, please try again. \n");
                    Console.WriteLine("\n --------------------------------------- \n");
                    break;
            }
        }

        // making port a parameter despite having a public port variable due to needing to offer different ports for the multiple threads that may be running outside of the above while loop
        public void startWebServer()
        {
            Console.WriteLine($"Listening on port {port}");
            app?.Run($"http://localhost:{port}");
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

    public class DisplayInformation
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
            Console.WriteLine("7. View budget goal");
            Console.WriteLine("8. Set budget goal");
            Console.WriteLine("0. End \n");
        }
    }
}