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
        
        public bool ViewExpenseTotal(bool startupCall)
        {
            try 
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                double expenseTotal = 0;

                while (reader.Read())
                {
                    expenseTotal += reader.GetDouble(0);
                }

                if (!startupCall)
                {
                    Console.WriteLine("\n --------------------------------------- \n");
                    Console.WriteLine($"\n Expense Total: \n {expenseTotal} \n");
                    Console.WriteLine("\n --------------------------------------- \n");
                }

                // returns our current values for currentBudget and totalExpense
                BudgetTracking budgetTracker = new BudgetTracking();
                Dictionary<string, string> previousBudget = budgetTracker.getBudgetAndExpense();

                // update expenseTotal & re-write the budget.json content
                previousBudget["currentExpenseTotal"] = expenseTotal.ToString();

                var serializedUpdatedBudget = JsonSerializer.Serialize(previousBudget);
                File.WriteAllText("./budget.json", serializedUpdatedBudget);

                // end the reader
                reader.Close();
                // discard the command
                command.Dispose();

                if (!startupCall)
                {
                    commandMenu.displayInteractionMenu();
                }
                
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error viewing expense total: {ex}");
            }
        }

        public bool ViewExpenseDetails()
        {
            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                // list where table data will be saved
                List<Dictionary<string, string>> listOfEntries = new List<Dictionary<string, string>>();

                Console.WriteLine($"\n\n {"Id:", 0} {"Description:", 25} {"Amount:", 25} {"Category:", 25} {"Date:", 25}");
                Console.WriteLine("\n --------------------------------------------------------------------------------------------------------------------- \n");

                while (reader.Read())
                {
                    string? id = reader["id"].ToString();
                    string? description = reader["description"].ToString();
                    string? amount = reader["amount"].ToString();
                    string? category = reader["category"].ToString();
                    string? date = reader["date"].ToString();

                    Console.WriteLine($"{id, 0} {description, 25} {amount, 25} {category, 25} {date, 25}");

                }
                Console.WriteLine("\n --------------------------------------------------------------------------------------------------------------------- \n");

                Console.WriteLine(reader);

                reader.Close();
                command.Dispose();

                commandMenu.displayInteractionMenu();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error viewing expense detail: {ex}");
            }
        }
    }
}