using System;
using Npgsql;
using UserInteraction;

namespace RouteMethods
{
    public class PostAndPutRouteMethods : ApiMethods
    {
        private Expense expense;
        private int? id;
        private NpgsqlParameter? description;
        private NpgsqlParameter? amount;
        private NpgsqlParameter? category;
        private NpgsqlParameter? date;
        private NpgsqlParameter? updatedExpenseId;
        private DisplayInformation displayInfo = new DisplayInformation();

        public PostAndPutRouteMethods(NpgsqlConnection dbConn, string commandText, Expense expense, int id = -1) : base(dbConn, commandText)
        {
            this.expense = expense;
            this.id = id;
        }

        private NpgsqlCommand setSqlParameters()
        {
            // creating params for prepared statement
            description = new NpgsqlParameter("Description", expense.Description);
            amount = new NpgsqlParameter("Amount", expense.Amount);
            category = new NpgsqlParameter("Category", expense.Category);
            date = new NpgsqlParameter("Date", expense.Date);

            NpgsqlCommand command = new NpgsqlCommand(commandText, dbConn);

            command.Parameters.Add(description);
            command.Parameters.Add(amount);
            command.Parameters.Add(category);
            command.Parameters.Add(date);

            return command;

        }

        public void createNewExpense()
        {
            NpgsqlCommand command = setSqlParameters();

            NpgsqlDataReader reader = command.ExecuteReader();

            reader.Close();
            command.Dispose();

            Console.WriteLine("\n Entry successfully added \n");

            displayInfo.displayInteractionMenu();
        }

        public void updateOldExpense()
        {
            NpgsqlCommand command = setSqlParameters();

            updatedExpenseId = new NpgsqlParameter("Id", id);
            command.Parameters.Add(updatedExpenseId);

            NpgsqlDataReader reader = command.ExecuteReader();

            reader.Close();
            command.Dispose();

            Console.WriteLine("\n Entry successfully updated \n");

            displayInfo.displayInteractionMenu();
        }
    }
}