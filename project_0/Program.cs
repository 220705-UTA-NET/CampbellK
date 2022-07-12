using Npgsql;
using Database;
using UserInteraction;

namespace Budget
{
    class Program
    {
        static void Main(string[] args)
        {
            // establish connection to database; needs to be passed in routing/api
            DbConnection connection = new DbConnection();
            NpgsqlConnection dbConn = connection.DbConnect();

            // ask for user commands and respond accordingly
            UserInput userInput = new UserInput(dbConn, args);
            userInput.askUserInput();

            // close the db connection @ program finish
            dbConn.Close();
        }
    }
}