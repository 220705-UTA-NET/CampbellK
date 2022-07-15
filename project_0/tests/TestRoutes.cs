using System;
using Xunit;
using Npgsql;
using Budget.Database;
using Budget.Tracking;
using Budget.UserInteraction;
using Budget.RouteMethods;

namespace Budget.ApiTests
{
    public class TestRoutes
    {
        protected NpgsqlConnection dbConn = DbConnection.DbConnect();
    }

    // returns true if successful, or an Exception if fail for read routes
    public class TestReadRoutes : TestRoutes
    {
        [Fact]
        public void TestSuccessViewExpense()
        {
            ReadRouteMethods readRoute = new ReadRouteMethods(dbConn, "Select amount FROM budget");
            bool result = readRoute.ViewExpenseTotal();

            // Assert.Equal("expected", "actual");
            Assert.True(result);
        }

        [Fact]
        public void TestFailViewExpense()
        {
            Assert.Equal("true", "123");
        }
    }
}