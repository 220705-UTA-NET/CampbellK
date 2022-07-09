using System;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Npgsql;
using Database;
using Api;

namespace Budget
{
    class Program
    {
        static void Main(string[] args)
        {
            DbConnection connection = new DbConnection();
            NpgsqlConnection dbConn = connection.DbConnect();

            // establish server component
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // API routes -- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0
            BudgetApi api = new BudgetApi();
            app.MapGet("/viewExpenseTotal", () => api.ViewExpenseTotal(dbConn));
            app.MapGet("/viewExpenseDetails", () => api.viewExpenseDetails(dbConn));
            // for routes that need access to the request content, will need to parse the JSON first, then send that data on to the cooresponding method
            app.MapPost("/newExpense", async (HttpRequest httpRequest) => {

                // read request body content
                StreamReader reader = new StreamReader(httpRequest.Body);
                string requestBody = await reader.ReadToEndAsync();
                Console.WriteLine(requestBody);

                // parse request JSON into cooresponding expense class
                Expense newExpense = JsonSerializer.Deserialize<Expense>(requestBody); 

                // once body has been parsed, can send to addExpense
                api.AddExpense(dbConn, newExpense);
             });
            app.MapDelete("/resetExpenses", () => "Reset");

            // is blocking; how to enable user to keep asking for commands in console?
            app.Run("http://localhost:3000");

            // close the db connection; should this go elsewhere?
            dbConn.Close();
        }
    }
}