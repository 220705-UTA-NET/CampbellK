using System;
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
            app.MapGet("/viewBalance", () => api.ReadBudgetEntries(dbConn));
            app.MapGet("/expenses", () => "Expenses");
            app.MapPost("/newExpense", () => "Add expense");
            app.MapDelete("/resetExpenses", () => "Reset");

            // is blocking; how to enable user to keep asking for commands in console?
            app.Run("http://localhost:3000");

            // close the db connection; should this go elsewhere?
            dbConn.Close();
        }
    }
}