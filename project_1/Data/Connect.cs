﻿using System;
using System.Data.SqlClient;

namespace Flash.Data
{
    public class Database
    {
        // establish azure connection
        private static string? connectionString = System.Environment.GetEnvironmentVariable("Azure_Connection_String") ?? throw new ArgumentNullException(nameof(connectionString));

        public SqlConnection DbConnect()
        {
            SqlConnection dbConn = new SqlConnection(connectionString);

            dbConn.Open();
            System.Console.WriteLine("Sql Connection Established");

            return dbConn;
        }

        // Fetch all flashcards
        // will be used to either study or view all in a ledger
        public List<Flashcard> FetchAllFlashcards()
        {
            SqlConnection dbConn = DbConnect();

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
                flashcard.Reading = reader.GetString(4);
                flashcard.Difficulty = reader.GetString(5);

                allFlashcards.Add(flashcard);
            }

            dbConn.Close();
            return allFlashcards;
        }
        public int CreateNewCard(Flashcard newFlashcard)
        {
            SqlConnection dbConn = DbConnect();

            using SqlCommand reviewCommand = new SqlCommand("INSERT INTO review (Word, SuccessfulReviews, FailedReviews) VALUES (@Word, 0, 0)", dbConn);
            reviewCommand.Parameters.AddWithValue("@Word", newFlashcard.Word);

            reviewCommand.ExecuteNonQuery();

            // insert into flashcards AFTER it is in review due to FK
            using SqlCommand command = new SqlCommand("INSERT INTO flashcards (Word, Definition, Example, Reading, Difficulty) VALUES (@Word, @Definition, @Example, @Reading, @Difficulty)", dbConn);

            SetQueryParameters(command, newFlashcard);

            int commandStatus = command.ExecuteNonQuery();

            dbConn.Close();
            return commandStatus;
        }

        public int EditCard(Flashcard updatedFlashcard, int cardId)
        {
            SqlConnection dbConn = DbConnect();

            using SqlCommand reviewCommand = new SqlCommand("UPDATE review SET Word = @Word WHERE Id = @Id", dbConn);
            reviewCommand.Parameters.AddWithValue("@Word", updatedFlashcard.Word);
            reviewCommand.Parameters.AddWithValue("@Id", cardId);

            reviewCommand.ExecuteNonQuery();

            using SqlCommand command = new SqlCommand("UPDATE flashcards SET Word = @Word, Definition = @Definition, Example = @Example, Reading = @Reading, Difficulty = @Difficulty WHERE Id = @Id", dbConn);

            SetQueryParameters(command, updatedFlashcard);
            command.Parameters.AddWithValue("@Id", cardId);

            int commandStatus = command.ExecuteNonQuery();

            dbConn.Close();
            return commandStatus;
        }

        public int DeleteCard(int cardId)
        {
            SqlConnection dbConn = DbConnect();

            using SqlCommand reviewCommand = new SqlCommand("DELETE FROM review WHERE Id = @Id", dbConn);
            reviewCommand.Parameters.AddWithValue("@Id", cardId);

            reviewCommand.ExecuteNonQuery();

            using SqlCommand command = new SqlCommand("DELETE FROM flashcards WHERE Id = @Id", dbConn);
            command.Parameters.AddWithValue("@Id", cardId);

            int commandStatus = command.ExecuteNonQuery();

            dbConn.Close();
            return commandStatus;
        }

        public int DeleteAllCards()
        {
            SqlConnection dbConn = DbConnect();

            using SqlCommand reviewCommand = new SqlCommand("DELETE FROM review", dbConn);
            reviewCommand.ExecuteNonQuery();

            using SqlCommand command = new SqlCommand("DELETE FROM flashcards", dbConn);

            int commandStatus = command.ExecuteNonQuery();

            dbConn.Close();
            return commandStatus;
        }

        private void SetQueryParameters(SqlCommand command, Flashcard flashcard)
        {
            // currently appears to be accepting Japanese characters
            // if it returns ??, add N in front of the value
            command.Parameters.AddWithValue("@Word", $"{flashcard.Word}");
            command.Parameters.AddWithValue("@Definition", flashcard.Definition);
            command.Parameters.AddWithValue("@Example", $"{flashcard.Example}");
            command.Parameters.AddWithValue("@Reading", $"{flashcard.Reading}");
            command.Parameters.AddWithValue("@Difficulty", flashcard.Difficulty);
        }
    }
}