using System;
using Xunit;
using Npgsql;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Budget.Database;
using Budget.Tracking;
using Budget.UserInteraction;
using Budget.RouteMethods;
using Budget.Helpers;
using Budget.Routes;

namespace Budget.ApiTests
{
    public class TestReadRoutes
    {
        [Fact]
        public void TestSuccessViewExpenseDetail()
        {
            // need to create a new connection for each test since each api ends by closing the connect
            NpgsqlConnection dbConn = DbConnection.DbConnect();

            ReadRouteMethods readRoute = new ReadRouteMethods(dbConn, "Select * FROM budget");
            bool result = readRoute.ViewExpenseDetails();

            // Assert.Equal("expected", "actual");
            // returns true if successful, or an Exception if fail for read routes
            Assert.True(result);
        }

        [Fact]
        public void TestFailViewExpenseDetail()
        {
            NpgsqlConnection dbConn = DbConnection.DbConnect();
            ReadRouteMethods readRoute = new ReadRouteMethods(dbConn, "Select abc FROM budget");

            Assert.Throws<Exception>(() => readRoute.ViewExpenseDetails());
        }
    }

    public class TestPostAndPutRoutes
    {
        NpgsqlConnection dbConn;
        Expense testData = new Expense();
        PostAndPutRouteMethods route;
        int insertStatus;

        public TestPostAndPutRoutes()
        {
            testData.Description = "Testing description";
            testData.Amount = 50;
            testData.Category = "Testing";
            testData.Date = "07/17/2022";
        }

        [Fact]
        public void TestSuccessCreateNewExpense()
        {
            dbConn = DbConnection.DbConnect();

            route = new PostAndPutRouteMethods(dbConn, "INSERT INTO budget (description, amount, category, date) VALUES (@Description, @Amount, @Category, @Date)", testData, -1);

            insertStatus = route.CreateNewExpense();

            // returns the number of rows inserted upon success
            Assert.Equal(1, insertStatus);
        }

        [Fact]
        public void TestSuccessUpdateExpense()
        {
            NpgsqlConnection dbConn = DbConnection.DbConnect();

            // will need a valid id in order to test the put route
            route = new PostAndPutRouteMethods(dbConn, "UPDATE budget SET (description, amount, category, date) = (@Description, @Amount, @Category, @Date) WHERE id = @Id", testData, 40);

            insertStatus = route.UpdateOldExpense();

            // returns the number of rows inserted upon success
            Assert.Equal(1, insertStatus);
        }
    }    
}