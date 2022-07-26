using Flash.Data;
using System;
using System.Text;
using System.Text.Json;

namespace Flash.Console.UserInterface
{
    public class UserInput
    {
        private readonly string uri = "https://localhost:7106";
            
        private static void DisplayMenu()
        {
            System.Console.WriteLine("\n[1] Review Session");
            System.Console.WriteLine("[2] View all cards");
            System.Console.WriteLine("[3] Create a new card");
            System.Console.WriteLine("[4] Edit a card");
            System.Console.WriteLine("[5] Delete a card");
            System.Console.WriteLine("[6] Delete all cards");
            System.Console.WriteLine("[0] Exit\n");
        }

       public void HandleUserInput()
        {
            CreateLineBreak();

            HttpClient client = new HttpClient();

            DisplayMenu();

            string userRequest = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(userRequest));
            FireUserRequest(client, userRequest);
            
        }

        private void FireUserRequest(HttpClient client, string userRequest)
        {
            switch(userRequest)
            {
                case "1":
                    ReviewCardsAsync(client).Wait();
                    break;
                case "2":
                    ViewAllCardsAsync(client).Wait();
                    break;
                case "3":
                    CreateNewCardAsync(client).Wait();
                    break;
                case "4":
                    EditCardAsync(client).Wait();
                    break;
                case "5":
                    DeleteCardAsync(client).Wait();
                    break;
                case "6":
                    DeleteAllCardsAsync(client).Wait();
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

        async private Task ReviewCardsAsync(HttpClient client)
        {
            var response = await client.GetAsync($"{uri}/reviewAll");
            string responseContent = await response.Content.ReadAsStringAsync();

            List<Flashcard> contents = JsonSerializer.Deserialize<List<Flashcard>>(responseContent) ?? throw new NullReferenceException(nameof(contents));

            System.Console.WriteLine("\n For each word shown, type the defintion. \n");

            // CreateReviewSession loops through all vocabulary, testing for definition
            List<Flashcard> reviewResults = CreateReviewSession(contents);

            System.Console.WriteLine($"\n Number of incorrect responses: {reviewResults.Count} \n");
            if (reviewResults.Count != 0)
            {
                System.Console.WriteLine("\n Incorrect responses:");
                foreach (Flashcard card in reviewResults)
                {
                    System.Console.Write($"{card.Word}\t\t");
                };
            }

            // give opportunity to re-try missed vocabulary
            if (reviewResults.Count > 0)
            {
                // as the user gets more correct, toReview should shrink until user is ready to quit or no more words are missed
                List<Flashcard> toReview = reviewResults;
                
                while (toReview.Count > 0)
                {
                    System.Console.WriteLine("\n Would you like to re-try your failed words? Y/N \n");

                    string retryResponse = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(retryResponse));

                    if (retryResponse.ToLower() == "y")
                    {
                        toReview = CreateReviewSession(reviewResults);
                    }
                    else
                    {
                        toReview = new List<Flashcard> { };
                    }
                }
            }

            HandleUserInput();
        }

        async private Task ViewAllCardsAsync(HttpClient client)
        {
            var response = await client.GetAsync($"{uri}/reviewAll");
            string responseContent = await response.Content.ReadAsStringAsync();

            List<Flashcard> contents = JsonSerializer.Deserialize<List<Flashcard>>(responseContent) ?? throw new NullReferenceException(nameof(contents));
            System.Console.WriteLine($"\n {"Id", 3} {"|", 10} {"Word", 10} {"|",10} {"Definition", 50} {"|",10} {"Example", 10} {"|",10} {"Notes", 10} {"|",10} {"Difficulty", 10} \n");

            CreateLineBreak();

            foreach (Flashcard card in contents)
            {
                System.Console.WriteLine($"\n {card.Id, 3} {"|",10} {card.Word, 10} {"|",10} {card.Definition, 50} {"|",10} {card.Example, 10} {"|",10} {card.Notes, 10} {"|",10} {card.Difficulty, 10} \n");
            }

            HandleUserInput();
        }

        async private Task CreateNewCardAsync(HttpClient client)
        {
            Flashcard newCard = new Flashcard();

            System.Console.WriteLine("\n Creating a new flashcard... \n");

            System.Console.WriteLine("\n Would you like to auto-fill a card? Y/N \n");
            string autoFillCard = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(autoFillCard));

            if (autoFillCard.ToLower() == "y")
            {
                try
                {
                    newCard = await AutoFillCardAsync();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Autocomplete failed, please enter your new card manually: {ex}");
                }
            }
            else
            {
                System.Console.WriteLine("\n Continuing to manual card creation. \n");
                newCard = FillOutFlashcard();
            }

            string serializedContent = JsonSerializer.Serialize(newCard);
            // Required to include the data type in StringContent, or else get a 415 error
            StringContent stringContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{uri}/addNewCard", stringContent);
            await ParseResponseAsync(response);
        }

        async private Task EditCardAsync(HttpClient client)
        {
            System.Console.WriteLine("\n Editing card... \n");
            System.Console.WriteLine("Type the Id of the card you would like to edit:\n");

            string cardId = GetCardId();
            
            Flashcard updatedCard = FillOutFlashcard();
            string serializedContent = JsonSerializer.Serialize(updatedCard);
            StringContent stringContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{uri}/editCard/{cardId}", stringContent);
            await ParseResponseAsync(response);
        }

        async private Task DeleteCardAsync(HttpClient client)
        {
            System.Console.WriteLine("\n Deleting card... \n");
            System.Console.WriteLine("Type the Id of the card you would like to delete:\n");

            string cardId = GetCardId();

            HttpResponseMessage response = await client.DeleteAsync($"{uri}/deleteCard/{cardId}");
            await ParseResponseAsync(response);
        }

        async private Task DeleteAllCardsAsync(HttpClient client)
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
                    await ParseResponseAsync(response);
                }
                else if (userDeleteResponse.ToLower() == "n")
                {
                    validInput = true;

                    System.Console.WriteLine("\n Returning to main menu... \n");
                    HandleUserInput();
                }
                else
                {
                    System.Console.WriteLine("\n Command not recognized, please try again.\n");
                }
            }
        }

        private List<Flashcard> CreateReviewSession(List<Flashcard> contents)
        {
            List<Flashcard> failedWords = new List<Flashcard> { };

            foreach (Flashcard card in contents)
            {
                System.Console.WriteLine($"\n{card.Word}\n");
                string userAnswer = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(userAnswer));

                if (userAnswer.ToLower() == card.Definition?.ToLower())
                {
                    System.Console.WriteLine("\n CORRECT \n");
                }
                else
                {
                    System.Console.WriteLine("\n INCORRECT... \n");
                    failedWords.Add(card);
                }

                System.Console.WriteLine($"\n {"Id",0} {"|",10} {"Word",10} {"|",10} {"Definition",50} {"|",10} {"Example",10} {"|",10} {"Notes",10} {"|",10} {"Difficulty",10} \n");

                CreateLineBreak();

                System.Console.WriteLine($"\n {card.Id,0} {"|",10} {card.Word,10} {"|",10} {card.Definition,50} {"|",10} {card.Example,10} {"|",10} {card.Notes,10} {"|",10} {card.Difficulty,10} \n");

                CreateLineBreak();
            };

            return failedWords;
        }

        private Flashcard FillOutFlashcard()
        {
            Flashcard card = new Flashcard();

            bool gotWord = false;
            while (!gotWord)
            {
                System.Console.WriteLine("\nWord:");
                card.Word = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(card.Word));

                if (card.Word.Length != 0)
                {
                    gotWord = true;
                }
                else
                {
                    System.Console.WriteLine("\nPlease enter a word\n");
                }
            }

            bool gotDefinition = false;
            while (!gotDefinition)
            {
                System.Console.WriteLine("\nDefinition:");
                card.Definition = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(card.Definition));

                if (card.Definition.Length != 0)
                {
                    gotDefinition = true;
                }
                else
                {
                    System.Console.WriteLine("\nPlease enter a definition\n");
                }
            }

            System.Console.WriteLine("\nExample:");
            card.Example = System.Console.ReadLine();

            System.Console.WriteLine("\nNotes:");
            card.Notes = System.Console.ReadLine();

            System.Console.WriteLine("\nDifficulty:");
            card.Difficulty = System.Console.ReadLine();

            return card;
        }

        async private Task<Flashcard> AutoFillCardAsync()
        {
            Flashcard newCard = new Flashcard();

            System.Console.WriteLine("\n Which word would you like to autofill? \n");
            string desiredWord = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(desiredWord));

            HttpClient jishoClient = new HttpClient();

            HttpResponseMessage wordData = await jishoClient.GetAsync($"https://jisho.org/api/v1/search/words?keyword={desiredWord}");

            string autofillResponse = await wordData.Content.ReadAsStringAsync();

            AutoFillFlashcard autoFilledData = JsonSerializer.Deserialize<AutoFillFlashcard>(autofillResponse) ?? throw new NullReferenceException(nameof(autoFilledData));

            newCard.Word = autoFilledData.data[0].slug ?? throw new NullReferenceException(nameof(newCard.Word));
            newCard.Definition = autoFilledData.data[0].senses[0].english_definitions[0] ?? throw new NullReferenceException(nameof(newCard.Definition));
            newCard.Example = "";
            newCard.Notes = autoFilledData.data[0].japanese[0].reading;
            newCard.Difficulty = autoFilledData.data[0].jlpt[0];

            newCard.lastReviewed = DateTime.Now;
            newCard.nextReview = DateTime.Today.AddDays(1);

            return newCard;    
        }

        async private Task ParseResponseAsync(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            string contents = JsonSerializer.Deserialize<string>(responseContent) ?? throw new NullReferenceException(nameof(contents));

            System.Console.WriteLine($"\n{contents}\n");
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
                    System.Console.WriteLine("\n Please type a valid number \n");
                }
            }

            return cardId;
        }

        private static void CreateLineBreak()
        {
            System.Console.WriteLine("\n ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ \n");
        }
    }
}
