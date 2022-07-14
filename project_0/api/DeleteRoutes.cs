using System;
using Npgsql;
using UserInteraction;

namespace RouteMethods
{
    public class DeleteRouteMethods : ApiMethods
    {
        private int id;
        private DisplayInformation displayInfo = new DisplayInformation();

        public DeleteRouteMethods(NpgsqlConnection dbConn, string commandText, int id = -1) : base(dbConn, commandText)
        {
            this.id = id;
        }

        public void DeleteSingleExpense()
        {
            NpgsqlCommand command = new NpgsqlCommand(commandText, dbConn);

            NpgsqlParameter expenseId = new NpgsqlParameter("Id", id);
            command.Parameters.Add(expenseId);

            Console.WriteLine("\n --------------------------------------- \n");
            Console.WriteLine("Expense deleted");
            Console.WriteLine("\n --------------------------------------- \n");

            executeDeleteCommand(command);
        }

        public void ResetExpenses()
        {
            NpgsqlCommand command = new NpgsqlCommand(commandText, dbConn);
            Console.WriteLine("\n --------------------------------------- \n");
            Console.WriteLine("All expenses reset");
            Console.WriteLine("\n --------------------------------------- \n");
            
            executeDeleteCommand(command);
        }

        private void executeDeleteCommand(NpgsqlCommand command)
        {
            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                reader.Close();
                command.Dispose();

                displayInfo.displayInteractionMenu();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to execute delete command: {ex}");
            }

        }
    }
}