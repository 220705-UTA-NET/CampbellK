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

        public int DeleteSingleExpense()
        {
            NpgsqlCommand command = new NpgsqlCommand(commandText, dbConn);

            NpgsqlParameter expenseId = new NpgsqlParameter("Id", id);
            command.Parameters.Add(expenseId);

            Console.WriteLine("\n --------------------------------------- \n");
            Console.WriteLine("Expense deleted");
            Console.WriteLine("\n --------------------------------------- \n");

            int affectedRows = executeDeleteCommand(command);

            commandMenu.DisplayInteractionMenu();

            return affectedRows;
        }

        public int ResetExpenses()
        {
            NpgsqlCommand command = new NpgsqlCommand(commandText, dbConn);
            Console.WriteLine("\n --------------------------------------- \n");
            Console.WriteLine("All expenses reset");
            Console.WriteLine("\n --------------------------------------- \n");
            
            int affectedRows = executeDeleteCommand(command);

            commandMenu.DisplayInteractionMenu();

            return affectedRows;
        }

        private int executeDeleteCommand(NpgsqlCommand command)
        {
            try
            {
                int reader = command.ExecuteNonQuery();
                command.Dispose();

                return reader;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to execute delete command: {ex}");
            }

        }
    }
}