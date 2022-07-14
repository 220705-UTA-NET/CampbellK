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

    public class ReadRoutes: ApiMethods
    {
        public ReadRoutes(NpgsqlConnection dbConn, string commandText) : base(dbConn, commandText)
        {}
        
        public void ViewExpenseTotal()
        {
            NpgsqlDataReader reader = command.ExecuteReader();

            double expenseTotal = 0;

            while (reader.Read())
            {
                expenseTotal += reader.GetDouble(0);
            }

            Console.WriteLine($"\n Expense Total: \n {expenseTotal} \n");

            // end the reader
            reader.Close();
            // discard the command
            command.Dispose();

            // returns our current values for currentBudget and totalExpense
            Dictionary<string, string> previousBudget = budgetTracker.getBudgetAndExpense();

            // update expenseTotal & re-write the budget.json content
            previousBudget["currentExpenseTotal"] = expenseTotal.ToString();

            var serializedUpdatedBudget = JsonSerializer.Serialize(previousBudget);
            File.WriteAllText("./budget.json", serializedUpdatedBudget);

            // re-print the interaction menu
            DisplayInformation displayInfo = new DisplayInformation();
            displayInfo.displayInteractionMenu();
        }

        public void ViewExpenseDetails()
        {
            NpgsqlDataReader reader = command.ExecuteReader();

            // list where table data will be saved
            List<Dictionary<string, string>> listOfEntries = new List<Dictionary<string, string>>();

            Console.WriteLine($"\n Id:\t\t Description:\t\t Amount:\t\t Category:\t\t Date:\t\t");

            while (reader.Read())
            {
                string? id = reader["id"].ToString();
                string? description = reader["description"].ToString();
                string? amount = reader["amount"].ToString();
                string? category = reader["category"].ToString();
                string? date = reader["date"].ToString();

                Console.WriteLine($"{id}\t\t\t {description}\t\t\t {amount}\t\t\t {category}\t\t {date}");
            }

            reader.Close();
            command.Dispose();

            DisplayInformation displayInfo = new DisplayInformation();
            displayInfo.displayInteractionMenu();
        }
    }

    public class PostAndPutRoutes : ApiMethods
    {
        private Expense expense;
        private int id;

        public PostAndPutRoutes(NpgsqlConnection dbConn, string commandText, Expense expense, int id = -1) : base(dbConn, commandText)
        {
            this.expense = expense;
            this.id = id;
        }

        public void changeExpense()
        {
            NpgsqlCommand command = new NpgsqlCommand(commandText, dbConn);

            // creating params for prepared statement
            NpgsqlParameter description = new NpgsqlParameter("Description", expense.Description);
            NpgsqlParameter amount = new NpgsqlParameter("Amount", expense.Amount);
            NpgsqlParameter category = new NpgsqlParameter("Category", expense.Category);
            NpgsqlParameter date = new NpgsqlParameter("Date", expense.Date);

            // for UPDATE rather than POST
            NpgsqlParameter updatedExpenseId = new NpgsqlParameter("Id", -1);
            if (id != -1)
            {
                updatedExpenseId = new NpgsqlParameter("Id", id);
            }

            command.Parameters.Add(description);
            command.Parameters.Add(amount);
            command.Parameters.Add(category);
            command.Parameters.Add(date);

            // Add id parameter for editExpense
            if (id != -1)
            {
                command.Parameters.Add(updatedExpenseId);
            }

            NpgsqlDataReader reader = command.ExecuteReader();

            reader.Close();
            command.Dispose();

            if (id == -1)
            {
                Console.WriteLine("Entry successfully added");
            }
            else 
            {
                Console.WriteLine("Entry successfully updated");
            }

            DisplayInformation displayInfo = new DisplayInformation();
            displayInfo.displayInteractionMenu();
        }
    }

    public class DeleteRoutes : ApiMethods
    {
        private int id;

        public DeleteRoutes(NpgsqlConnection dbConn, string commandText, int id = -1) : base(dbConn, commandText)
        {
            this.id = id;
        }

        public void deleteExpenses()
        {
            NpgsqlCommand command = new NpgsqlCommand(commandText, dbConn);

            // if deleting a particular expense item RATHER than clearing the table
            if (id != -1)
            {
                NpgsqlParameter expenseId = new NpgsqlParameter("Id", id);
                command.Parameters.Add(expenseId);
            }

            NpgsqlDataReader reader = command.ExecuteReader();

            reader.Close();
            command.Dispose();

            if (id != -1)
            {
                Console.WriteLine("Expense deleted");
            }
            else
            {
                Console.WriteLine("All expenses reset");
            }

            DisplayInformation displayInfo = new DisplayInformation();
            displayInfo.displayInteractionMenu();
        }  
    }
}