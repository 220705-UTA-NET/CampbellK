using System;
using Npgsql;
using Database;

namespace Api
{
    class BudgetApi
    {
        // set up consistent variables that all methods will need
        

        //methods for read, write, edit, delete
        public void ReadBudgetEntries(NpgsqlConnection dbConn)
        {
            // will want to set up a prepared statement
                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM budget", dbConn);

                // execute query and save results 
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

                    // combine the row data into a single struct to save to listOfEntries;
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

                Console.WriteLine(listOfEntries[0]["id"]);

                // end the reader
                reader.Close();
                // discard the command
                command.Dispose();
        }
    }
}