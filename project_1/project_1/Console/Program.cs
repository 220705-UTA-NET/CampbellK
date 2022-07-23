using System;
using Flash.Data;

namespace Flash.Console
{
    public class Console
    {
        static void Main()
        {

            // Everything below is just for testing and needs to be deleted after CRUD methods are set up

            Database dbConn = new Database();
            dbConn.DbConnect();

            // delete all
            dbConn.DeleteAllCards();
        }
    }
}