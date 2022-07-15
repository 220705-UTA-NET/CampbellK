using System;
using Xunit;
using Budget.RouteMethods;

namespace Budget.ApiTests
{
    // returns true if successful, or an Exception if fail for read routes
    public class TestReadRoutes
    {
        [Fact]
        public void TestSuccessViewExpense()
        {


            Assert.Equal("expected", "actual");
        }

        [Fact]
        public void TestFailViewExpense()
        {
            Assert.Equal("true", "true");
        }
    }
}