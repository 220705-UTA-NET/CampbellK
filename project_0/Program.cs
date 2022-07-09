using System;
using System.IO;
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

            // ask for user input continuously -- perhaps in its own class?
            // how do we get this working?

            // Console.WriteLine("What would you like to do?");
            // Console.WriteLine("1. View current balance  2. View past expenses  3. Add a new expense  4. Reset expenses");
            // string? userCommand = Console.ReadLine();

            // API routes -- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0
            BudgetApi api = new BudgetApi();
            app.MapGet("/viewExpenseTotal", () => api.ViewExpenseTotal(dbConn));
            app.MapGet("/viewExpenseDetails", () => api.viewExpenseDetails(dbConn));
            // app.MapPost("/newExpense", () => api.AddExpense(dbConn));
             app.MapPost("/newExpense", async (HttpRequest httpRequest) => {

                // read request body for JSON
                StreamReader reader = new StreamReader(httpRequest.Body);
                string requestBody = await reader.ReadToEndAsync();
                Console.WriteLine(requestBody);

                // parse request body JSON

                // once body has been parsed, can send to addExpense
                // api.AddExpense(dbConn);
             });
            app.MapDelete("/resetExpenses", () => "Reset");

            // is blocking; how to enable user to keep asking for commands in console?
            app.Run("http://localhost:3000");

            // close the db connection; should this go elsewhere?
            dbConn.Close();
        }
    }
}