using System;
using System.Text.Json;

namespace Tracking
{
    public class BudgetTracking
    {
        public int currentBudget;
        public double currentExpenseTotal;

        public void fetchUserBudgetInfo()
        {
            if (!File.Exists("./budget.json"))
            {
                File.Create("./budget.json");

                Dictionary<string, string> defaultTracker = new Dictionary<string, string>()
                {
                    {"currentBudget", "0"},
                    {"currentExpenseTotal", "0"}
                };

                var serializedDefault = JsonSerializer.Serialize(defaultTracker);

                File.AppendAllText("./budget.json", serializedDefault);
            }
            else
            {
                string budgetJson = File.ReadAllText("./budget.json");

                Dictionary<string, string>? previousBudgetInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(budgetJson);


                // now need to utilize the object to write to global variables for expense total and budget goal

                currentBudget = Int32.Parse(previousBudgetInfo["currentBudget"]);
                currentExpenseTotal = Convert.ToDouble(previousBudgetInfo["currentExpenseTotal"]);

            }

            // logic for displaying current budget and current total expenses
            Console.WriteLine($"\n Current budget goal: \n {currentBudget}");
            Console.WriteLine($"\n Current expense total:\n {currentExpenseTotal}");
            Console.WriteLine($"\n You have ${currentBudget - currentExpenseTotal} remaining \n");
        }
    }
}