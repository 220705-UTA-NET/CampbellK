using System;
using Xunit;
using Npgsql;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using System.Threading;
using Budget.Database;
using Budget.Tracking;
using Budget.UserInteraction;
using Budget.RouteMethods;
using Budget.Helpers;
using Budget.Routes;

namespace Budget.ApiTests
{
    public class TestApiRoutes
    {
        NpgsqlConnection dbConn;
        private Expense testData = new Expense();

        public TestApiRoutes()
        {
            testData.Description = "Testing description";
            testData.Amount = 50;
            testData.Category = "Testing";
            testData.Date = "07/17/2022";
        }

        [Fact]
        public void TestSuccessViewExpenseDetail()
        {
            // need to create a new connection for each test since each api ends by closing the connect
            dbConn = DbConnection.DbConnect();

            ReadRouteMethods readRoute = new ReadRouteMethods(dbConn, "Select * FROM budget");
            bool result = readRoute.ViewExpenseDetails();

            // Assert.Equal("expected", "actual");
            // returns true if successful, or an Exception if fail for read routes
            Assert.True(result);
        }

        [Fact]
        public void TestFailViewExpenseDetail()
        {
            dbConn = DbConnection.DbConnect();
            ReadRouteMethods readRoute = new ReadRouteMethods(dbConn, "Select abc FROM budget");

            Assert.Throws<Exception>(() => readRoute.ViewExpenseDetails());
        }

        [Fact]
        public void TestSuccessCreateNewExpense()
        {
            dbConn = DbConnection.DbConnect();

            PostAndPutRouteMethods route = new PostAndPutRouteMethods(dbConn, "INSERT INTO budget (description, amount, category, date) VALUES (@Description, @Amount, @Category, @Date)", testData, -1);

            int insertStatus = route.CreateNewExpense();

            // returns the number of rows inserted upon success
            Assert.Equal(1, insertStatus);
        }

        [Fact]
        public void TestSuccessUpdateExpense()
        {
            dbConn = DbConnection.DbConnect();

            // will need a valid id in order to test the put route
            PostAndPutRouteMethods route = new PostAndPutRouteMethods(dbConn, "UPDATE budget SET (description, amount, category, date) = (@Description, @Amount, @Category, @Date) WHERE id = @Id", testData, 73);

            int insertStatus = route.UpdateOldExpense();

            // returns the number of rows inserted upon success
            Assert.Equal(1, insertStatus);
        }

        [Fact]
        public void TestSuccessDeleteOne()
        {
            dbConn = DbConnection.DbConnect();

            // will need a valid id to be successful
            DeleteRouteMethods route = new DeleteRouteMethods(dbConn, "DELETE FROM budget WHERE id = @Id", 73);
            int deleteStatus = route.DeleteSingleExpense();
            
            Assert.Equal(1, deleteStatus);
        }

        [Fact]
        public void TestSuccessResetExpenses()
        {
            dbConn = DbConnection.DbConnect();

            DeleteRouteMethods route = new DeleteRouteMethods(dbConn, "DELETE FROM budget", -1);
            
            int deleteStatus = route.ResetExpenses();

            Assert.True(deleteStatus > 0);
        }
    }
}