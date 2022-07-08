using System;
using Microsoft.AspNetCore.Builder;
using database;

namespace project_0
{
    class Program
    {
        static void Main(string[] args)
        {
            DbConnection conn = new DbConnection();
            conn.DbConnect();

            // establish server component
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // API routes
            app.MapGet("/", () => "Hello world!");

            app.Run("http://localhost:3000");
        }
    }
}