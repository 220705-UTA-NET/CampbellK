using Microsoft.AspNetCore.Builder;
using System.Text.Json;
using Npgsql;
using Budget.RouteMethods;
using Budget.Routes;
using Budget.Tracking;
using Budget.Helpers;

// contains the code for handling all user interaction with the console and the multi-threading required to both run a web server & interact with it simultaneously
namespace Budget.UserInteraction
{
    public class UserInput
    {
        private bool exitStatus = false;
        private bool firstLoop = true;
        private NpgsqlConnection dbConn;
        // drilled down from main; required fro WebApplication.CreateBuilder()
        private string[] args;
        // other threads may take some time to shut down, so utilizing various ports to avoid conflict
        public int port = 3000;
        // allows us to read & write to budget.json, where totalExpense & budgetGoal are stored
        private BudgetTracking budgetTracker = new BudgetTracking();
        private ApiRoutes apiRoutes;
        private HelperMethods helperMethods;

        public UserInput(NpgsqlConnection dbConn, string[] args)
        {
            this.dbConn = dbConn;
            this.args = args;
            // establish server connection & routes
            apiRoutes = new ApiRoutes(dbConn, args);
            helperMethods = new HelperMethods();
        }

        public void askUserInput()
        {
            try
            {
                // listening for requests on a seperate thread
                Thread webServerThread = new Thread(() => helperMethods.startWebServer(port, dbConn, args));
                webServerThread.Start();

                while (!exitStatus)
                {
                    // app needs to be re-recreated for each loop since it is readonly after creation (and therefore cannot change the http url)
                    WebApplication app = apiRoutes.EstablishRoutes(dbConn, args) ?? throw new ArgumentNullException(nameof(app));

                    HttpClient client = new HttpClient();

                    if (firstLoop)
                    {
                        helperMethods.DisplayInteractionMenu();
                        firstLoop = false;
                    }

                    string? userAction = Console.ReadLine();
                    handleUserInput(userAction, client, app);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed during user input: {ex}");
            }
        }

        async private void handleUserInput(string? userInput, HttpClient client, WebApplication app)
        {
            switch(userInput)
            {
                case "1":
                    await client.GetAsync($"http://localhost:{port}/viewExpenseTotal");

                    break;

                case "2":                    
                    await client.GetAsync($"http://localhost:{port}/viewExpenseDetails");

                    break;

                case "3":
                    Console.WriteLine("\n Creating a new expenditure. Fill in the relevant information... \n");

                    StringContent stringContent = GatherExpenseInfo();

                    await client.PostAsync($"http://localhost:{port}/newExpense", stringContent);

                    break;

                case "4":
                    Console.WriteLine("\n Type the id of the expense you would like to edit: \n");
                    string? expenseToEditId = Console.ReadLine();

                    Console.WriteLine("\n Editing expenditure: type the relevant information. \n");
                    StringContent editedStringContent = GatherExpenseInfo();
                    
                    try 
                    {
                        await client.PutAsync($"http://localhost:{port}/editExpense/{expenseToEditId}", editedStringContent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n The edit request failed: {ex} \n");
                    }

                    break;

                case "5":
                    Console.WriteLine("\n Type the id of the expense you would like to delete: \n");
                    string? expenseToDelete = Console.ReadLine();
                    
                    try 
                    {
                        await client.DeleteAsync($"http://localhost:{port}/deleteExpense/{expenseToDelete}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n The delete request failed: {ex} \n");
                    }

                    break;

                case "6":
                    Console.WriteLine("\n Reseting expenses... \n");
                    
                    await client.DeleteAsync($"http://localhost:{port}/resetExpenses");
                    break;
                
                case "7":
                    Dictionary<string, string> priorBudget = budgetTracker.getBudgetAndExpense();

                    Console.WriteLine("\n --------------------------------------- \n");
                    Console.WriteLine("Current budget goal:");
                    Console.WriteLine(priorBudget["currentBudget"]);
                    Console.WriteLine("\n --------------------------------------- \n");

                    helperMethods.DisplayInteractionMenu();

                    break;
                
                case "8":
                    Console.WriteLine("\n Type your new budget goal: \n");

                    string budgetGoal = "";
                    bool gotBudgetNumber = false;

                    while (!gotBudgetNumber)
                    {
                        try
                        {
                            budgetGoal = Console.ReadLine() ?? throw new ArgumentNullException(nameof(budgetGoal));

                            double testIfNumber = Convert.ToDouble(budgetGoal);

                            gotBudgetNumber = true;
                        }
                        catch
                        {
                            Console.WriteLine("\n Invalid input. Please submit a number \n");
                        }
                    }

                    Dictionary<string, string> previousBudget = budgetTracker.getBudgetAndExpense();

                    // update expenseTotal & re-write the budget.json content
                    previousBudget["currentBudget"] = budgetGoal;

                    var serializedUpdatedBudget = JsonSerializer.Serialize(previousBudget);
                    File.WriteAllText("./budget.json", serializedUpdatedBudget);

                    Console.WriteLine("\n --------------------------------------- \n");
                    Console.WriteLine("Budget goal set");
                    Console.WriteLine("\n --------------------------------------- \n");

                    helperMethods.DisplayInteractionMenu();

                    break;

                case "0":
                    Console.WriteLine("\n --------------------------------------- \n");
                    Console.WriteLine("\n Terminating program... \n");
                    Console.WriteLine("\n --------------------------------------- \n");
                    exitStatus = true;
                    app?.StopAsync();

                    // setting exit, app.stopasync & breaking is not reliably ending the program, so including the below
                    Environment.Exit(0);

                    break;

                default:
                    Console.WriteLine("\n --------------------------------------- \n");
                    Console.WriteLine("\n Command not recognized, please try again. \n");
                    Console.WriteLine("\n --------------------------------------- \n");

                    helperMethods.DisplayInteractionMenu();

                    break;
            }
        }

        private StringContent GatherExpenseInfo()
        {
            // combine the below responses into an object & serialize it for post request
            Expense editedInformation = new Expense();

            Console.WriteLine("Description:");
            editedInformation.Description = Console.ReadLine();

            Console.WriteLine("Amount:");
            bool gotNumber = false;
            // continue to loop until user gives a number
            while (!gotNumber)
            {
                try
                {
                    editedInformation.Amount = Convert.ToDouble(Console.ReadLine());
                    gotNumber = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("\n Invalid input. Please enter a number \n");
                }
                
            } 
            
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