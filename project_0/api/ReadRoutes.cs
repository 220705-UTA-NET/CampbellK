using System;
using Npgsql;
using System.Text.Json;
using Budget.Tracking;

namespace Budget.RouteMethods
{
    public class ReadRouteMethods: ApiMethods
    {
        public ReadRouteMethods(NpgsqlConnection dbConn, string commandText) : base(dbConn, commandText)
        {}
        
        public void ViewExpenseTotal()
        {
            try 
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                double expenseTotal = 0;

                while (reader.Read())
                {
                    expenseTotal += reader.GetDouble(0);
                }

                Console.WriteLine("\n --------------------------------------- \n");
                Console.WriteLine($"\n Expense Total: \n {expenseTotal} \n");
                Console.WriteLine("\n --------------------------------------- \n");

                // end the reader
                reader.Close();
                // discard the command
                command.Dispose();

                // returns our current values for currentBudget and totalExpense
                BudgetTracking budgetTracker = new BudgetTracking();
                Dictionary<string, string> previousBudget = budgetTracker.getBudgetAndExpense();

                // update expenseTotal & re-write the budget.json content
                previousBudget["currentExpenseTotal"] = expenseTotal.ToString();

                var serializedUpdatedBudget = JsonSerializer.Serialize(previousBudget);
                File.WriteAllText("./budget.json", serializedUpdatedBudget);

                commandMenu.displayInteractionMenu();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error viewing expense total: {ex}");
            }
        }

        public void ViewExpenseDetails()
        {
            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                // list where table data will be saved
                List<Dictionary<string, string>> listOfEntries = new List<Dictionary<string, string>>();

                Console.WriteLine("\n --------------------------------------- \n");
                Console.WriteLine($"\n Id:\t\t Description:\t\t Amount:\t\t Category:\t\t Date:\t\t");

                while (reader.Read())
                {
                    string? id = reader["id"].ToString();
                    string? description = reader["description"].ToString();
                    string? amount = reader["amount"].ToString();
                    string? category = reader["category"].ToString();
                    string? date = reader["date"].ToString();

                    Console.WriteLine($"{id}\t\t {description}\t\t\t {amount}\t\t\t {category}\t\t {date}");
                }
                Console.WriteLine("\n --------------------------------------- \n");

                reader.Close();
                command.Dispose();

                commandMenu.displayInteractionMenu();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error viewing expense detail: {ex}");
            }
        }
    }
}