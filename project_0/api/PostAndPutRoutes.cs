using System;
using Npgsql;
using System.Text.Json;
using Budget.UserInteraction;
using Budget.Tracking;

namespace Budget.RouteMethods
{
    public class PostAndPutRouteMethods : ApiMethods
    {
        private Expense expense;
        private int? id;
        private NpgsqlParameter? description;
        private NpgsqlParameter? amount;
        private NpgsqlParameter? category;
        private NpgsqlParameter? date;
        private NpgsqlParameter? updatedExpenseId;

        public PostAndPutRouteMethods(NpgsqlConnection dbConn, string commandText, Expense expense, int id = -1) : base(dbConn, commandText)
        {
            this.expense = expense ?? throw new ArgumentNullException(nameof(expense));
            this.id = id;
        }

        private NpgsqlCommand SetSqlParameters()
        {
            // creating params for prepared statement
            description = new NpgsqlParameter("Description", expense.Description);
            amount = new NpgsqlParameter("Amount", expense.Amount);
            category = new NpgsqlParameter("Category", expense.Category);
            date = new NpgsqlParameter("Date", expense.Date);

            NpgsqlCommand command = new NpgsqlCommand(commandText, dbConn);

            command.Parameters.Add(description);
            command.Parameters.Add(amount);
            command.Parameters.Add(category);
            command.Parameters.Add(date);

            return command;
        }

        public int CreateNewExpense()
        {
            try
            {
                NpgsqlCommand command = SetSqlParameters();

                int reader = command.ExecuteNonQuery();
                command.Dispose();

                Dictionary<string, string> updatedExpenseAndReminader = GetUpdatedExpenseAndRemainder();

                Console.WriteLine("\n --------------------------------------- \n");
                Console.WriteLine("\nEntry successfully added.");
                Console.WriteLine($"New expense total: ${updatedExpenseAndReminader["updatedTotalExpense"]}");
                Console.WriteLine($"You have ${updatedExpenseAndReminader["remainder"]} remaining\n");
                Console.WriteLine("\n --------------------------------------- \n");

                commandMenu.DisplayInteractionMenu();

                return reader;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create new expense: {ex}");
            }
        }

        public int UpdateOldExpense()
        {
            try
            {
                NpgsqlCommand command = SetSqlParameters();

                updatedExpenseId = new NpgsqlParameter("Id", id);
                command.Parameters.Add(updatedExpenseId);

                int reader = command.ExecuteNonQuery();
                command.Dispose();

                Console.WriteLine("\n --------------------------------------- \n");
                Console.WriteLine("\n Entry successfully updated \n");
                Console.WriteLine("\n --------------------------------------- \n");

                commandMenu.DisplayInteractionMenu();

                return reader;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating previous expense: {ex}");
            }
        }

        private Dictionary<string, string> GetUpdatedExpenseAndRemainder()
        {
            // incorporate into a seperate function when finished
            BudgetTracking tracker = new BudgetTracking();
            Dictionary<string, string> previousBudget = tracker.getBudgetAndExpense();

            double updatedTotalExpense = expense.Amount + Convert.ToDouble(previousBudget["currentExpenseTotal"]);
            double remainder = Int32.Parse(previousBudget["currentBudget"]) - updatedTotalExpense;

            Dictionary<string, string> updatedExpenseAndBudget = new Dictionary<string, string>()
            {
                {"updatedTotalExpense", updatedTotalExpense.ToString()},
                {"remainder", remainder.ToString()}
            };

            return updatedExpenseAndBudget;
        }
    }
}