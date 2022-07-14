using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Npgsql;
using RouteMethods;

// contains all routes to be used by the web server
// returns *app*, needed to start up the web server in the additional threads created in UserInput namespace
namespace Routes
{
    public class ApiRoutes
    {  
        public WebApplication EstablishRoutes(NpgsqlConnection dbConn, string[] args)
        {   
            // establish server component
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            WebApplication app = builder.Build();

            // show sum of all expense costs
            app.MapGet("/viewExpenseTotal", () => {
                ReadRouteMethods api = new ReadRouteMethods(dbConn, "SELECT amount FROM budget");
                api.ViewExpenseTotal();
            });

            // show details for all expenses
            app.MapGet("/viewExpenseDetails", () => {
                ReadRouteMethods api = new ReadRouteMethods(dbConn, "SELECT * FROM budget");
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
                PostAndPutRouteMethods postOrPutRoutes = new PostAndPutRouteMethods(dbConn, "INSERT INTO budget (Description, Amount, Category, Date) VALUES (@Description, @Amount, @Category, @Date)", newExpense);
                postOrPutRoutes.createNewExpense();
            });
            
            // edit existing expense
            app.MapPut("/editExpense/{id}", async (HttpRequest httpRequest, int id) => {
                StreamReader reader = new StreamReader(httpRequest.Body);
                string requestBody = await reader.ReadToEndAsync();

                Expense? updatedExpense = JsonSerializer.Deserialize<Expense>(requestBody);

                PostAndPutRouteMethods postOrPutRoutes = new PostAndPutRouteMethods(dbConn, "UPDATE budget SET (Description, Amount, Category, Date) = (@Description, @Amount, @Category, @Date) WHERE id = @id", updatedExpense, id);
                postOrPutRoutes.updateOldExpense();
            });

            // delete a particular expense
            app.MapDelete("/deleteExpense/{id}", (int id) => {
                DeleteRouteMethods deleteSingleExpense = new DeleteRouteMethods(dbConn, "DELETE FROM budget WHERE id = @id", id);
                deleteSingleExpense.deleteExpenses();
            });

            // delete all expenses
            app.MapDelete("/resetExpenses", () => {
                DeleteRouteMethods deleteAllExpenses = new DeleteRouteMethods(dbConn, "TRUNCATE TABLE budget", -1);
                deleteAllExpenses.deleteExpenses();
            });

            return app;
        }
    }
}