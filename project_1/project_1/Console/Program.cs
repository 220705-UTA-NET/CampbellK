using System;
using Flash.Data;

namespace Flash.Console
{
    public class Console
    {
        static void Main()
        {
            Database dbConn = new Database();
            dbConn.DbConnect();
        }
    }
}