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
            // establish connection to database; needs to be passed in routing/api
            DbConnection connection = new DbConnection();
            NpgsqlConnection dbConn = connection.DbConnect();

            // establish server component
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // API routes

            // app.MapGet("/viewExpenseTotal", () => api.ViewExpenseTotal(dbConn));
            app.MapGet("/viewExpenseTotal", () => {
                ReadRoutes api = new ReadRoutes(dbConn, "SELECT amount FROM budget");
                api.ViewExpenseTotal();
            });

            app.MapGet("/viewExpenseDetails", () => {
                ReadRoutes api = new ReadRoutes(dbConn, "SELECT * FROM budget");
                api.ViewExpenseDetails();
            });

            app.MapPost("/newExpense", async (HttpRequest httpRequest) => {

                // read request body content
                StreamReader reader = new StreamReader(httpRequest.Body);
                string requestBody = await reader.ReadToEndAsync();

                // parse request JSON into cooresponding expense class
                Expense newExpense = JsonSerializer.Deserialize<Expense>(requestBody); 

                // once body has been parsed, can send to addExpense
                PostAndPutRoutes postOrPutRoutes = new PostAndPutRoutes(dbConn, newExpense, -1);
                postOrPutRoutes.changeExpense();
             });
            
            app.MapPut("/editExpense/{id}", async (HttpRequest httpRequest, int id) => {
                StreamReader reader = new StreamReader(httpRequest.Body);
                string requestBody = await reader.ReadToEndAsync();

                Expense updatedExpense = JsonSerializer.Deserialize<Expense>(requestBody);

                PostAndPutRoutes postOrPutRoutes = new PostAndPutRoutes(dbConn, updatedExpense, id);
                postOrPutRoutes.changeExpense();
            });

            app.MapDelete("/deleteExpense/{id}", (int id) => {
                DeleteRoutes deleteSingleExpense = new DeleteRoutes(dbConn, id);
                deleteSingleExpense.deleteExpenses();
            });

            app.MapDelete("/resetExpenses", () => {
                DeleteRoutes deleteAllExpenses = new DeleteRoutes(dbConn, -1);
                deleteAllExpenses.deleteExpenses();
            });

            // start web server: is blocking
            app.Run("http://localhost:3000");

            // close the db connection
            dbConn.Close();
        }
    }
}