using System;
using Npgsql;

namespace Api
{
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

        public string AddExpense(NpgsqlConnection dbConn, Expense newExpense)
        {

            NpgsqlCommand command = new NpgsqlCommand("INSERT INTO budget (Description, Amount, Category, Date) VALUES (@Description, @Amount, @Category, @Date)", dbConn);

            NpgsqlParameter description = new NpgsqlParameter("Description", newExpense.Description);
            NpgsqlParameter amount = new NpgsqlParameter("Amount", newExpense.Amount);
            NpgsqlParameter category = new NpgsqlParameter("Category", newExpense.Category);
            NpgsqlParameter date = new NpgsqlParameter("Date", newExpense.Date);

            command.Parameters.Add(description);
            command.Parameters.Add(amount);
            command.Parameters.Add(category);
            command.Parameters.Add(date);

            NpgsqlDataReader reader = command.ExecuteReader();

            reader.Close();
            command.Dispose();

            return "Entry successfully added";
        }

        public string UpdateExpense(NpgsqlConnection dbConn, int id, Expense updatedExpense)
        {
            NpgsqlCommand command = new NpgsqlCommand("UPDATE budget SET (Description, Amount, Category, Date) = (@Description, @Amount, @Category, @Date) WHERE id = @id", dbConn);

            NpgsqlParameter description = new NpgsqlParameter("Description", updatedExpense.Description);
            NpgsqlParameter amount = new NpgsqlParameter("Amount", updatedExpense.Amount);
            NpgsqlParameter category = new NpgsqlParameter("Category", updatedExpense.Category);
            NpgsqlParameter date = new NpgsqlParameter("Date", updatedExpense.Date);
            NpgsqlParameter updatedExpenseId = new NpgsqlParameter("Id", id);

            command.Parameters.Add(description);
            command.Parameters.Add(amount);
            command.Parameters.Add(category);
            command.Parameters.Add(date);
            command.Parameters.Add(updatedExpenseId);

            NpgsqlDataReader reader = command.ExecuteReader();

            reader.Close();
            command.Dispose();

            return "Expense updated";
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

    public class Expense
    {
        public int? Id {get; set;}
        public string? Description {get; set;}
        public double Amount {get; set;}
        public string? Category {get; set;}
        public string? Date {get; set;}
    }
}