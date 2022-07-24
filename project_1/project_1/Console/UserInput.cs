using Flash.Data;
using System;
using System.Text.Json;

namespace Flash.Console.UserInterface
{
    public class UserInput
    {
        private readonly string uri = "https://localhost:7106";
        bool exit = false;

        public static void DisplayMenu()
        {
            System.Console.WriteLine("[1] Review Session");
            System.Console.WriteLine("[2] View all cards");
            System.Console.WriteLine("[3] Create a new card");
            System.Console.WriteLine("[4] Edit a card");
            System.Console.WriteLine("[5] Delete a card");
            System.Console.WriteLine("[6] Delete all cards");
            System.Console.WriteLine("[0] Exit");
        }

       public void HandleUserInput()
        {
            if (!exit)
            {
                HttpClient client = new HttpClient();

                DisplayMenu();

                string? userRequest = System.Console.ReadLine();
                FireUserRequest(client, userRequest);
            }
        }

        private void FireUserRequest(HttpClient client, string userRequest)
        {
            switch(userRequest)
            {
                case "1":
                    ReviewCards(client, userRequest).Wait();
                    break;
            }
        }

        async private Task ReviewCards(HttpClient client, string userRequest)
        {
            var response = await client.GetAsync($"{uri}/reviewAll");
            string responseContent = await response.Content.ReadAsStringAsync();

            List<Flashcard> contents = JsonSerializer.Deserialize<List<Flashcard>>(responseContent);

            System.Console.WriteLine("\n For each word shown, type the defintion.");
            // foreach through each word in contents, ask for user input, then give definition, example, notes
            foreach(Flashcard card in contents)
            {
                System.Console.WriteLine($"\n{card.Word}\n");
                string userAnswer = System.Console.ReadLine();

                if (userAnswer.ToLower() == card.Definition.ToLower())
                {
                    System.Console.WriteLine("\n Correct! \n");
                }
                else
                {
                    System.Console.WriteLine("\n Incorrect... \n");
                }

                System.Console.WriteLine($"\n {"Word", 0} {"Definition", 20} {"Example", 20} {"Notes", 20} {"Difficulty", 20} \n");
                System.Console.WriteLine($"\n {card.Word, 0} {card.Definition, 20} {card.Example, 20} {card.Notes, 20} {card.Difficulty, 20} \n");
            };

            HandleUserInput();
        }
    }
}
