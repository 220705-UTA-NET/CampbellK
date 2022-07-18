using System;
using Npgsql;
using Budget.Helpers;

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
        protected NpgsqlConnection dbConn;
        protected string commandText = "";
        protected NpgsqlCommand command;
        protected HelperMethods commandMenu = new HelperMethods();

        public ApiMethods(NpgsqlConnection dbConn, string commandText)
        {
            this.dbConn = dbConn;
            this.commandText = commandText;
            this.command = new NpgsqlCommand(commandText, dbConn);
        }
    }
}