using System;
using Npgsql;
using UserInteraction;
using System.Text.Json;

namespace RouteMethods
{
    public class ReadRouteMethods: ApiMethods
    {
        public ReadRouteMethods(NpgsqlConnection dbConn, string commandText) : base(dbConn, commandText)
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
}