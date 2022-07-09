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
    
    class BudgetApi
    {
        // set up consistent variables that all methods will need
        
        public double ViewExpenseTotal(NpgsqlConnection dbConn)
        {
            // sql command; use prepared statement for any user values
            NpgsqlCommand command = new NpgsqlCommand("SELECT amount FROM budget", dbConn);
            // execute query and save results 
            NpgsqlDataReader reader = command.ExecuteReader();

            double expenseTotal = 0;

            while (reader.Read())
            {
                expenseTotal += reader.GetDouble(0);
            }

            // end the reader
            reader.Close();
            // discard the command
            command.Dispose();

            return expenseTotal;
        }

        public List<Dictionary<string, string>> viewExpenseDetails(NpgsqlConnection dbConn)
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM budget", dbConn);

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

        public string deleteExpense(NpgsqlConnection dbConn, int id)
        {
            NpgsqlCommand command = new NpgsqlCommand("DELETE FROM budget WHERE id = @id", dbConn);
            NpgsqlDataReader reader = command.ExecuteReader();

            reader.Close();
            command.Dispose();

            return "Expense deleted";
        }

        public string resetExpenses(NpgsqlConnection dbConn)
        {
            NpgsqlCommand command = new NpgsqlCommand("TRUNCATE TABLE budget", dbConn);

            NpgsqlDataReader reader = command.ExecuteReader();

            reader.Close();
            command.Dispose();

            return "Expense sheet reset";
        }
    }

    public class PostOrPut
    {
        NpgsqlConnection dbConn;
        Expense expense;
        public int id;

        // if Id is present, then we know it is UPDATE. If not, then we know it is POST
        public PostOrPut(NpgsqlConnection dbConn, Expense expense, int id = -1)
        {
            this.dbConn = dbConn;
            this.expense = expense;
            this.id = id;
        }

        public string changeExpense()
        {
            string commandText;
            // if a new insert rather than update an exisiting expense
            if (id == -1)
            {
                commandText = "INSERT INTO budget (Description, Amount, Category, Date) VALUES (@Description, @Amount, @Category, @Date)";
            }
            else
            {
                commandText = "UPDATE budget SET (Description, Amount, Category, Date) = (@Description, @Amount, @Category, @Date) WHERE id = @id";
            }


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

            // for UPDATE rather than POST
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
}