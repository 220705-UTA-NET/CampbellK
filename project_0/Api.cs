using System;
using Npgsql;

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
        
        public double ViewExpenseTotal()
        {
            NpgsqlDataReader reader = command.ExecuteReader();

            double expenseTotal = 0;

            while (reader.Read())
            {
                expenseTotal += reader.GetDouble(0);
            }

            Console.WriteLine("");
            Console.WriteLine(expenseTotal);
            Console.WriteLine("");

            // end the reader
            reader.Close();
            // discard the command
            command.Dispose();

            return expenseTotal;
        }

        public List<Dictionary<string, string>> ViewExpenseDetails()
        {

            NpgsqlDataReader reader = command.ExecuteReader();

            // list where table data will be saved
            List<Dictionary<string, string>> listOfEntries = new List<Dictionary<string, string>>();

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
            }

            reader.Close();
            command.Dispose();

            return listOfEntries;
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

        public string changeExpense()
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
                return "Entry successfully added";
            }
            else 
            {
                return "Entry successfully updated";
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

        public string deleteExpenses()
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
                return "Expense deleted";
            }
            else
            {
                return "All expenses reset";
            }
        }  
    }
}