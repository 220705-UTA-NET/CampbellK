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
            NpgsqlConnection dbConn = DbConnection.DbConnect();

            // display users current totalExpense & budgetGoal
            BudgetTracking budgetConstruction = new BudgetTracking(); budgetConstruction.fetchUserBudgetInfo(dbConn);

            // ask for user commands and respond accordingly
            UserInput userInput = new UserInput(dbConn, args);
            userInput.askUserInput();

            // close the db connection @ program finish
            dbConn.Close();
        }
    }
}