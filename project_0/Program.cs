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

            app.MapPost("/newExpense", async (HttpRequest httpRequest) => {

                // read request body content
                StreamReader reader = new StreamReader(httpRequest.Body);
                string requestBody = await reader.ReadToEndAsync();

                // parse request JSON into cooresponding expense class
                Expense newExpense = JsonSerializer.Deserialize<Expense>(requestBody); 

                // once body has been parsed, can send to addExpense
                api.AddExpense(dbConn, newExpense);
             });
            
            app.MapPut("/editExpense/{id}", async (HttpRequest httpRequest, int id) => {
                StreamReader reader = new StreamReader(httpRequest.Body);
                string requestBody = await reader.ReadToEndAsync();

                Expense updatedExpense = JsonSerializer.Deserialize<Expense>(requestBody);

                api.UpdateExpense(dbConn, id, updatedExpense);
            });

            app.MapDelete("/deleteExpense/{id}", () => "Delete expense");

            app.MapDelete("/resetExpenses", () => api.resetExpenses(dbConn));

            // start web server: is blocking
            app.Run("http://localhost:3000");

            // close the db connection; should this go elsewhere?
            dbConn.Close();
        }
    }
}