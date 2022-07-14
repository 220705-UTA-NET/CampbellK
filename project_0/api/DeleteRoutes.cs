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

            Console.WriteLine("Expense deleted");
            executeDeleteCommand(command);
        }

        public void ResetExpenses()
        {
            NpgsqlCommand command = new NpgsqlCommand(commandText, dbConn);

            Console.WriteLine("All expenses reset");
            executeDeleteCommand(command);
        }

        private void executeDeleteCommand(NpgsqlCommand command)
        {
            NpgsqlDataReader reader = command.ExecuteReader();
            reader.Close();
            command.Dispose();

            displayInfo.displayInteractionMenu();
        }
    }
}