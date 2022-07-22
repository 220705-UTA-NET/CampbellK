using System;
using System.Data.SqlClient;

namespace Flash.Data
{
    public class Database
    {
        // establish azure connection
        private static string? connectionString = System.Environment.GetEnvironmentVariable("Azure_Connection_String") ?? throw new ArgumentNullException(nameof(connectionString));
        public SqlConnection? dbConn;

        public SqlConnection DbConnect()
        {
            dbConn = new SqlConnection(connectionString);

            dbConn.Open();
            System.Console.WriteLine("Connection Established");

            FetchAllFlashcards();

            return dbConn;
        }

        // Fetch all flashcards
        // will be used to either study or view all in a ledger
        public List<Flashcard> FetchAllFlashcards()
        {
            List<Flashcard> allFlashcards = new List<Flashcard>();

            SqlCommand command = new SqlCommand("SELECT * FROM flashcards", dbConn);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Flashcard flashcard = new Flashcard();

                flashcard.Id = reader.GetInt32(0);
                flashcard.Word = reader.GetString(1);
                flashcard.Definition = reader.GetString(2);
                flashcard.Example = reader.GetString(3);
                flashcard.Difficulty = reader.GetString(4);

                allFlashcards.Add(flashcard);
            }

            return allFlashcards;
        }

        public void CreateNewCard()
        {
            SqlCommand command = new SqlCommand("INSERT INTO flashcards (Word, Definition, Example, Difficulty), (@Word, @Definition, @Example, @Difficulty)");
        }

        public void EditCard() { }


        public void DeleteCard() { }


        public void DeleteAllCards() { }
    }
}