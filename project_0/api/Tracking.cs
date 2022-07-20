using System;
using System.Text.Json;
using Npgsql;
using Budget.RouteMethods;

namespace Budget.Tracking
{
    public class BudgetTracking
    {
        private int currentBudget;
        private double currentExpenseTotal;

        public void fetchUserBudgetInfo(NpgsqlConnection dbConn)
        {
            if (!File.Exists("./budget.json"))
            {
                Dictionary<string, string> defaultTracker = new Dictionary<string, string>()
                {
                    {"currentBudget", "0"},
                    {"currentExpenseTotal", "0"}
                };

                string serializedDefault = JsonSerializer.Serialize(defaultTracker);
                File.WriteAllText("./budget.json", serializedDefault);
            }
            else
            {    
                // update local tracking with fresh expense data
                ReadRouteMethods readRoutes = new ReadRouteMethods(dbConn, "SELECT amount FROM budget");
                readRoutes.ViewExpenseTotal(true);

                // get budget and expense values from budget.json & setting them in the current scope
                getBudgetAndExpense();

                Console.WriteLine($"\n Current budget goal: \n {currentBudget}");
                Console.WriteLine($"\n Current expense total:\n {currentExpenseTotal}");
                Console.WriteLine($"\n You have ${currentBudget - currentExpenseTotal} remaining \n");
                Console.WriteLine("\n --------------------------------------- \n");
            }
        }

        public Dictionary<string, string> getBudgetAndExpense()
        {
            string budgetJson = File.ReadAllText("./budget.json");

            Dictionary<string, string>? previousBudgetInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(budgetJson) ?? throw new ArgumentNullException(nameof(previousBudgetInfo));

            try 
            {
                currentBudget = Int32.Parse(previousBudgetInfo["currentBudget"]);
                currentExpenseTotal = Convert.ToDouble(previousBudgetInfo["currentExpenseTotal"]);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error parsing budget info: {ex}");
            }

            return previousBudgetInfo;
        }
    }
}