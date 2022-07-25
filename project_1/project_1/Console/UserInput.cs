using Flash.Data;
using System;
using System.Text;
using System.Text.Json;

namespace Flash.Console.UserInterface
{
    public class UserInput
    {
        private readonly string uri = "https://localhost:7106";
        bool exit = false;

        public static void DisplayMenu()
        {
            System.Console.WriteLine("\n [1] Review Session");
            System.Console.WriteLine("[2] View all cards");
            System.Console.WriteLine("[3] Create a new card");
            System.Console.WriteLine("[4] Edit a card");
            System.Console.WriteLine("[5] Delete a card");
            System.Console.WriteLine("[6] Delete all cards");
            System.Console.WriteLine("\n [0] Exit");
        }

       public void HandleUserInput()
        {
            if (!exit)
            {
                HttpClient client = new HttpClient();

                DisplayMenu();

                string userRequest = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(userRequest));
                FireUserRequest(client, userRequest);
            }
        }

        private void FireUserRequest(HttpClient client, string userRequest)
        {
            switch(userRequest)
            {
                case "1":
                    ReviewCards(client).Wait();
                    break;
                case "2":
                    ViewAllCards(client).Wait();
                    break;
                case "3":
                    CreateNewCard(client).Wait();
                    break;
                case "4":
                    EditCard(client).Wait();
                    break;
                case "5":
                    DeleteCard(client).Wait();
                    break;
                case "6":
                    DeleteAllCards(client).Wait();
                    break;
                case "0":
                    System.Console.WriteLine("\n Terminating program...");
                    break;
                default:
                    System.Console.WriteLine("\n Unrecognized input, please try again. \n");
                    HandleUserInput();
                    break;
            }
        }

        async private Task ReviewCards(HttpClient client)
        {
            var response = await client.GetAsync($"{uri}/reviewAll");
            string responseContent = await response.Content.ReadAsStringAsync();

            List<Flashcard> contents = JsonSerializer.Deserialize<List<Flashcard>>(responseContent) ?? throw new NullReferenceException(nameof(contents));

            System.Console.WriteLine("\n For each word shown, type the defintion. \n");
            // foreach through each word in contents, ask for user input, then give definition, example, notes
            foreach(Flashcard card in contents)
            {
                System.Console.WriteLine($"\n{card.Word}\n");
                string userAnswer = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(userAnswer));

                if (userAnswer.ToLower() == card.Definition.ToLower())
                {
                    System.Console.WriteLine("\n Correct! \n");
                }
                else
                {
                    System.Console.WriteLine("\n Incorrect... \n");
                }

                System.Console.WriteLine($"\n {"Id", 0 }{"Word", 20} {"Definition", 20} {"Example", 20} {"Notes", 20} {"Difficulty", 20} \n");
                System.Console.WriteLine($"\n {card.Id, 0} {card.Word, 20} {card.Definition, 20} {card.Example, 20} {card.Notes, 20} {card.Difficulty, 20} \n");
            };

            HandleUserInput();
        }

        async private Task ViewAllCards(HttpClient client)
        {
            var response = await client.GetAsync($"{uri}/reviewAll");
            string responseContent = await response.Content.ReadAsStringAsync();

            List<Flashcard> contents = JsonSerializer.Deserialize<List<Flashcard>>(responseContent) ?? throw new NullReferenceException(nameof(contents));
            System.Console.WriteLine($"\n {"Id", 0} {"Word", 20} {"Definition", 20} {"Example", 20} {"Notes", 20} {"Difficulty", 20} \n");
            foreach (Flashcard card in contents)
            {
                System.Console.WriteLine($"\n {card.Id, 0} {card.Word, 20} {card.Definition, 20} {card.Example, 20} {card.Notes, 20} {card.Difficulty, 20} \n");
            }

            HandleUserInput();
        }

        async private Task CreateNewCard(HttpClient client)
        {
            System.Console.WriteLine("\n Creating a new flashcard... \n");

            Flashcard newCard = FillOutFlashcard();
            string serializedContent = JsonSerializer.Serialize(newCard);
            // Required to include the data type in StringContent, or else get a 415 error
            StringContent stringContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{uri}/addNewCard", stringContent);
            await ParseResponse(response);
        }

        async private Task EditCard(HttpClient client)
        {
            System.Console.WriteLine("\n Editing card... \n");
            System.Console.WriteLine("Type the Id of the card you would like to edit:");

            string cardId = GetCardId();
            
            Flashcard updatedCard = FillOutFlashcard();
            string serializedContent = JsonSerializer.Serialize(updatedCard);
            StringContent stringContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{uri}/editCard/{cardId}", stringContent);
            await ParseResponse(response);
        }

        async private Task DeleteCard(HttpClient client)
        {
            System.Console.WriteLine("\n Deleting card... \n");
            System.Console.WriteLine("Type the Id of the card you would like to delete:");

            string cardId = GetCardId();

            HttpResponseMessage response = await client.DeleteAsync($"{uri}/deleteCard/{cardId}");
            await ParseResponse(response);
        }

        async private Task DeleteAllCards(HttpClient client)
        {
            bool validInput = false;

            while (!validInput)
            {
                System.Console.WriteLine("\n Deleting all cards. Would you like to continue? Y/N \n");
                string userDeleteResponse = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(userDeleteResponse));

                if (userDeleteResponse.ToLower() == "y")
                {
                    validInput = true;

                    var response = await client.DeleteAsync($"{uri}/deleteAllCards");
                    await ParseResponse(response);
                }
                else if (userDeleteResponse.ToLower() == "n")
                {
                    validInput = true;

                    System.Console.WriteLine("\n Returning to main menu... \n");
                    HandleUserInput();
                }
                else
                {
                    System.Console.WriteLine("\n Command not recognized, please try again.");
                }
            }
        }

        private Flashcard FillOutFlashcard()
        {
            Flashcard card = new Flashcard();

            System.Console.WriteLine("Word:");
            card.Word = System.Console.ReadLine();

            System.Console.WriteLine("Definition:");
            card.Definition = System.Console.ReadLine();

            System.Console.WriteLine("Example:");
            card.Example = System.Console.ReadLine();

            System.Console.WriteLine("Notes:");
            card.Notes = System.Console.ReadLine();

            System.Console.WriteLine("Difficulty:");
            card.Difficulty = System.Console.ReadLine();

            return card;
        }

        async private Task ParseResponse(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            string contents = JsonSerializer.Deserialize<string>(responseContent) ?? throw new NullReferenceException(nameof(contents));

            System.Console.WriteLine(contents);
            HandleUserInput();
        }

        private static string GetCardId()
        {
            string cardId = "";
            bool retrievedCardId = false;
            while (!retrievedCardId)
            {
                try
                {
                    string userInput = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(userInput));
                    int userInputCardId = Int32.Parse(userInput);
                    cardId = userInputCardId.ToString();
                    retrievedCardId = true;
                }
                catch (Exception)
                {
                    System.Console.WriteLine("Please type a valid number");
                }
            }

            return cardId;
        }
    }
}
