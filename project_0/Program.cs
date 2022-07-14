using Npgsql;
using Budget.Database;
using Budget.UserInteraction;
using Budget.Tracking;

namespace Budget
{
    class Program
    {
        static void Main(string[] args)
        {
            // establish connection to database; needs to be passed in routing/api
            DbConnection connection = new DbConnection();
            NpgsqlConnection dbConn = connection.DbConnect();

            // display users current totalExpense & budgetGoal
            BudgetTracking budgetConstruction = new BudgetTracking(); budgetConstruction.fetchUserBudgetInfo();

            // ask for user commands and respond accordingly
            UserInput userInput = new UserInput(dbConn, args);
            userInput.askUserInput();

            // close the db connection @ program finish
            dbConn.Close();
        }
    }
}