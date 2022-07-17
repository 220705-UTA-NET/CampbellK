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

        [Fact]
        public void TestSuccessCreateNewExpense()
        {
            NpgsqlConnection dbConn = DbConnection.DbConnect();

            Expense testData = new Expense();
            testData.Description = "Testing description";
            testData.Amount = 50;
            testData.Category = "Testing";
            testData.Date = "07/17/2022";

            PostAndPutRouteMethods route = new PostAndPutRouteMethods(dbConn, "INSERT INTO budget (description, amount, category, date) VALUES (@1, @2, @3, @4)", testData, -1);

            int insertStatus = route.CreateNewExpense();

            // returns the number of rows inserted upon success
            Assert.Equal(1, insertStatus);
        }
    }
}