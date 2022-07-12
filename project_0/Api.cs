using System;
using Npgsql;
using UserInteraction;

// contains all API functionality used by the Routes namespace
// parent class: ApiMethods. Accepts and establishes the database connection and the sql command
// chidlren: ReadRoutes, PostAndPutRoutes, and DeleteRoutes
namespace Api
{
    public class Expense
    {
        public int? Id {get; set;}
        public string? Description {get; set;}
        public double Amount {get; set;}
        public string? Category {get; set;}
        public string? Date {get; set;}
    }

    public class ApiMethods
    {
        public NpgsqlConnection dbConn;
        public string commandText = "";
        public NpgsqlCommand command;

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
            // re-print the interaction menu
            DisplayInformation displayInfo = new DisplayInformation();
            displayInfo.displayInteractionMenu();
        }

        public void ViewExpenseDetails()
        {

            NpgsqlDataReader reader = command.ExecuteReader();

            // list where table data will be saved
            List<Dictionary<string, string>> listOfEntries = new List<Dictionary<string, string>>();

            Console.WriteLine($"Id:\t\t Description:\t\t Amount:\t\t Category:\t\t Date:\t\t");

            while (reader.Read())
            {
                string? id = reader["id"].ToString();
                string? description = reader["description"].ToString();
                string? amount = reader["amount"].ToString();
                string? category = reader["category"].ToString();
                string? date = reader["date"].ToString();

                // combine the row data into a single dictionary to save to listOfEntries;
                Dictionary<string, string> budgetEntry = new Dictionary<string, string>()
                {
                    {"id", id},
                    {"description", description},
                    {"amount", amount},
                    {"category", category},
                    {"date", date}
                };

                listOfEntries.Add(budgetEntry);

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
        Expense expense;
        private int id;

        public PostAndPutRoutes(NpgsqlConnection dbConn, string commandText, Expense expense, int id = -1) : base(dbConn, commandText)
        {
            this.expense = expense;
            this.id = id;
        }

        public void changeExpense()
        {
            NpgsqlCommand command = new NpgsqlCommand(commandText, dbConn);

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

            // add id parameter for editExpense
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

                DisplayInformation displayInfo = new DisplayInformation();
                displayInfo.displayInteractionMenu();
            }
            else 
            {
                Console.WriteLine("Entry successfully updated");

                DisplayInformation displayInfo = new DisplayInformation();
                displayInfo.displayInteractionMenu();
            }
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

                DisplayInformation displayInfo = new DisplayInformation();
                displayInfo.displayInteractionMenu();
            }
            else
            {
                Console.WriteLine("All expenses reset");
                DisplayInformation displayInfo = new DisplayInformation();
                displayInfo.displayInteractionMenu();
            }
        }  
    }
}