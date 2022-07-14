using System;
using Npgsql;
using Budget.UserInteraction;

// contains all API functionality used by the Routes namespace
// parent class: ApiMethods. Accepts and establishes the database connection and the sql command
// chidlren: ReadRoutes, PostAndPutRoutes, and DeleteRoutes
namespace Budget.RouteMethods
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
        protected DisplayInformation commandMenu = new DisplayInformation();

        public ApiMethods(NpgsqlConnection dbConn, string commandText)
        {
            this.dbConn = dbConn;
            this.commandText = commandText;
            // sql command; use prepared statement for any user values
            this.command = new NpgsqlCommand(commandText, dbConn);
        }
    }
}