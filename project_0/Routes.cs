using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Npgsql;
using Api;

// contains all routes to be used by the web server
// returns *app*, needed to start up the web server in the additional threads created in UserInput namespace
namespace Routes
{
    public class ApiRoutes
    {
        public WebApplication EstablishRoutes(NpgsqlConnection dbConn, string[] args)
        {
            // establish server component
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // API routes

            // show sum of all expense costs
            app.MapGet("/viewExpenseTotal", () => {
                ReadRoutes api = new ReadRoutes(dbConn, "SELECT amount FROM budget");
                api.ViewExpenseTotal();
            });

            // show details for all expenses
            app.MapGet("/viewExpenseDetails", () => {
                ReadRoutes api = new ReadRoutes(dbConn, "SELECT * FROM budget");
                api.ViewExpenseDetails();
            });

            // add a new expense
            app.MapPost("/newExpense", async (HttpRequest httpRequest) => {
                // read request body content
                StreamReader reader = new StreamReader(httpRequest.Body);
                string requestBody = await reader.ReadToEndAsync();

                // parse request JSON into cooresponding expense class
                Expense? newExpense = JsonSerializer.Deserialize<Expense>(requestBody); 

                // once body has been parsed, can send to addExpense
                PostAndPutRoutes postOrPutRoutes = new PostAndPutRoutes(dbConn, "INSERT INTO budget (Description, Amount, Category, Date) VALUES (@Description, @Amount, @Category, @Date)", newExpense, -1);
                postOrPutRoutes.changeExpense();
                });
            
            // edit an existing expense
            app.MapPut("/editExpense/{id}", async (HttpRequest httpRequest, int id) => {
                StreamReader reader = new StreamReader(httpRequest.Body);
                string requestBody = await reader.ReadToEndAsync();

                Expense? updatedExpense = JsonSerializer.Deserialize<Expense>(requestBody);

                PostAndPutRoutes postOrPutRoutes = new PostAndPutRoutes(dbConn, "UPDATE budget SET (Description, Amount, Category, Date) = (@Description, @Amount, @Category, @Date) WHERE id = @id", updatedExpense, id);
                postOrPutRoutes.changeExpense();
            });

            // delete a particular expense
            app.MapDelete("/deleteExpense/{id}", (int id) => {
                DeleteRoutes deleteSingleExpense = new DeleteRoutes(dbConn, "DELETE FROM budget WHERE id = @id", id);
                deleteSingleExpense.deleteExpenses();
            });

            // delete all expenses
            app.MapDelete("/resetExpenses", () => {
                DeleteRoutes deleteAllExpenses = new DeleteRoutes(dbConn, "TRUNCATE TABLE budget", -1);
                deleteAllExpenses.deleteExpenses();
            });

            // set a budget goal
            app.MapPost("/setBudget", async (HttpRequest httpRequest) => {
                StreamReader reader = new StreamReader(httpRequest.Body);
                string requestBody = await reader.ReadToEndAsync();

                string? newBudget = JsonSerializer.Deserialize<string>(requestBody);

               Users setBudget = new Users(dbConn, $"INSERT INTO budgetGoal (budget) VALUES ({newBudget})"); 
               setBudget.setBudgetGoal();
            });

            // view current budget
            app.MapGet("/viewBudget", () => {
               Users viewCurrentBudget = new Users(dbConn, "SELECT budget FROM budgetGoal ORDER BY id DESC LIMIT 1");
               viewCurrentBudget.ViewBudget();
            });

            return app;
        }
    }
}