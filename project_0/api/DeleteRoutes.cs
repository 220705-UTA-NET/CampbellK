using System;
using Npgsql;
using Budget.UserInteraction;

namespace Budget.RouteMethods
{
    public class DeleteRouteMethods : ApiMethods
    {
        private int id;

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

            commandMenu.displayInteractionMenu();
        }

        public void ResetExpenses()
        {
            NpgsqlCommand command = new NpgsqlCommand(commandText, dbConn);
            Console.WriteLine("\n --------------------------------------- \n");
            Console.WriteLine("All expenses reset");
            Console.WriteLine("\n --------------------------------------- \n");
            
            executeDeleteCommand(command);

            commandMenu.displayInteractionMenu();
        }

        private void executeDeleteCommand(NpgsqlCommand command)
        {
            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();
                reader.Close();
                command.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to execute delete command: {ex}");
            }

        }
    }
}