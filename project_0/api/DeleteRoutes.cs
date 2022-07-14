using System;
using Npgsql;
using UserInteraction;

namespace RouteMethods
{
    public class DeleteRouteMethods : ApiMethods
    {
        private int id;

        public DeleteRouteMethods(NpgsqlConnection dbConn, string commandText, int id = -1) : base(dbConn, commandText)
        {
            this.id = id;
        }

        public void deleteExpenses()
        {
            NpgsqlCommand command = new NpgsqlCommand(commandText, dbConn);

            // if deleting a particular expense item RATHER than clearing the table
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
                Console.WriteLine("Expense deleted");
            }
            else
            {
                Console.WriteLine("All expenses reset");
            }

            DisplayInformation displayInfo = new DisplayInformation();
            displayInfo.displayInteractionMenu();
        }  
    }
}