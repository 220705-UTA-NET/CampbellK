using System;
using Npgsql;
using UserInteraction;
using System.Text.Json;
using Tracking;

// contains all API functionality used by the Routes namespace
// parent class: ApiMethods. Accepts and establishes the database connection and the sql command
// chidlren: ReadRoutes, PostAndPutRoutes, and DeleteRoutes
namespace RouteMethods
{
    public class Expense
    {
        public int? Id {get; set;}
        public string? Description {get; set;}
        public double Amount {get; set;}
        public string? Category {get; set;}
        public string? Date {get; set;}
    }

    public abstract class ApiMethods
    {
        public NpgsqlConnection dbConn;
        public string commandText = "";
        public NpgsqlCommand command;
        // for pulling prior values of totalExpense & budgetGoal
        public BudgetTracking budgetTracker = new BudgetTracking();

        public ApiMethods(NpgsqlConnection dbConn, string commandText)
        {
            this.dbConn = dbConn;
            this.commandText = commandText;
            // sql command; use prepared statement for any user values
            this.command = new NpgsqlCommand(commandText, dbConn);
        }
    }
}