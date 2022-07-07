using System;
using database;

namespace project_0
{
    class Program
    {
        static void Main(string[] args)
        {
            DbConnection conn = new DbConnection();
            conn.DbConnect();
        }
    }
}