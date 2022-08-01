using System;
using System.Text;
using System.Text.Json;
using Flash.Data;

namespace Flash.Console.UserInterface
{
    public class UserInput
    {
        private readonly string uri = "https://projectonektc.azurewebsites.net";
        
        // for auto-generating example sentences
        SentenceScrapper scrapper = new SentenceScrapper();
            
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
                    ViewReviewStatsAsync(client).Wait();
                    break;
                case "3":
                    ViewAllCardsAsync(client).Wait();
                    break;
                case "4":
                    CreateNewCardAsync(client).Wait();
                    break;
                case "5":
                    EditCardAsync(client).Wait();
                    break;
                case "6":
                    DeleteCardAsync(client).Wait();
                    break;
                case "7":
                    DeleteAllCardsAsync(client).Wait();
                    break;
                case "0":
                    System.Console.WriteLine("\n\t Terminating program...");
                    break;
                default:
                    System.Console.WriteLine("\n\t Unrecognized input, please try again. \n");
                    HandleUserInput();
                    break;
            }
        }

        async private Task ReviewCardsAsync(HttpClient client)
        {
            var response = await client.GetAsync($"{uri}/reviewAll");
            string responseContent = await response.Content.ReadAsStringAsync();

            List<Flashcard> contents = JsonSerializer.Deserialize<List<Flashcard>>(responseContent) ?? throw new NullReferenceException(nameof(contents));

            System.Console.WriteLine("\n\t For each word shown, type the defintion. \n");

            // CreateReviewSession loops through all vocabulary, testing for definition
            List<Flashcard> reviewResults = await CreateReviewSession(contents);

            System.Console.WriteLine($"\n\t Number of incorrect responses: {reviewResults.Count} \n");

            if (reviewResults.Count != 0)
            {
                System.Console.WriteLine("\n\t Incorrect responses:");

                foreach (Flashcard card in reviewResults)
                {
                    System.Console.Write($"{card.Word}\t\t");
                };
            }

            // give opportunity to re-try missed vocabulary
            if (reviewResults.Count > 0)
            {
                // as the user gets more correct, toReview shrinks until user  quits or no more words are missed
                List<Flashcard> toReview = reviewResults;
                
                while (toReview.Count > 0)
                {
                    System.Console.WriteLine("\n\n\t Would you like to re-try your failed words? Y/N \n");

                    string retryResponse = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(retryResponse));

                    if (retryResponse.ToLower() == "y")
                    {
                        toReview = await CreateReviewSession(reviewResults);
                    }
                    else
                    {
                        toReview = new List<Flashcard> { };
                    }
                }
            }

            HandleUserInput();
        }

        async private Task ViewReviewStatsAsync(HttpClient client)
        {
            var response = await client.GetAsync($"{uri}/viewReviewStats");
            string responseContent = await response.Content.ReadAsStringAsync();

            List<WordTracker> contents = JsonSerializer.Deserialize<List<WordTracker>>(responseContent) ?? throw new NullReferenceException(nameof(contents));

            System.Console.WriteLine($"\n {"Word", 3} {"|", 10} {"Correct Reviews", 20} {"|",10} {"Failed Reviews", 30}");

            CreateLineBreak();

            foreach(WordTracker reviewStat in contents)
            {
                System.Console.WriteLine($"\n {reviewStat.Word, 3} {"|", 10} {reviewStat.Correct, 20} {"|",10} {reviewStat.Incorrect, 30}");  
            }

            HandleUserInput();
        }

        async private Task ViewAllCardsAsync(HttpClient client)
        {
            var response = await client.GetAsync($"{uri}/reviewAll");
            string responseContent = await response.Content.ReadAsStringAsync();

            List<Flashcard> contents = JsonSerializer.Deserialize<List<Flashcard>>(responseContent) ?? throw new NullReferenceException(nameof(contents));

            System.Console.WriteLine($"\n {"Id", 3} {"|", 10} {"Word", 10} {"|",10} {"Definition", 25} {"|",10} {"Example", 10} {"|",10} {"Reading", 10} {"|",10} {"Difficulty", 10} \n");

            CreateLineBreak();

            foreach (Flashcard card in contents)
            {
                System.Console.WriteLine($"\n {card.Id, 3} {"|",10} {card.Word, 10} {"|",10} {card.Definition, 25} {"|",10} {card.Example, 10} {"|",10} {card.Reading, 10} {"|",10} {card.Difficulty, 10} \n");
            }

            HandleUserInput();
        }

        async private Task CreateNewCardAsync(HttpClient client)
        {
            Flashcard newCard = new Flashcard();

            System.Console.WriteLine("\n\t Creating a new flashcard... \n");

            System.Console.WriteLine("\n\t Would you like to auto-fill a card? Y/N \n");
            string autoFillCard = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(autoFillCard));

            if (autoFillCard.ToLower() == "y")
            {
                try
                {
                    newCard = await AutoFillCardAsync();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"\n\t Autocomplete failed, please enter your new card manually: {ex}");
                }
            }
            else
            {
                System.Console.WriteLine("\n\t Continuing to manual card creation. \n");

                newCard = await FillOutFlashcard();
            }

            string serializedContent = JsonSerializer.Serialize(newCard);
            // Required to include the data type in StringContent, or else 415 error

            System.Console.WriteLine(serializedContent);

            StringContent stringContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{uri}/addNewCard", stringContent);

            await ParseResponseAsync(response);
        }

        async private Task EditCardAsync(HttpClient client)
        {
            System.Console.WriteLine("\n\t Editing card... \n");
            System.Console.WriteLine("\t Type the Id of the card you would like to edit:\n");

            string cardId = GetCardId();
            
            Flashcard updatedCard = await FillOutFlashcard();

            string serializedContent = JsonSerializer.Serialize(updatedCard);

            StringContent stringContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{uri}/editCard/{cardId}", stringContent);

            await ParseResponseAsync(response);
        }

        async private Task DeleteCardAsync(HttpClient client)
        {
            System.Console.WriteLine("\n\t Deleting card... \n");

            System.Console.WriteLine("\t Type the Id of the card you would like to delete:\n");

            string cardId = GetCardId();

            HttpResponseMessage response = await client.DeleteAsync($"{uri}/deleteCard/{cardId}");

            await ParseResponseAsync(response);
        }

        async private Task DeleteAllCardsAsync(HttpClient client)
        {
            bool validInput = false;

            while (!validInput)
            {
                System.Console.WriteLine("\n\t Deleting all cards. Would you like to continue? Y/N \n");

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

                    System.Console.WriteLine("\n\t Returning to main menu... \n");
                    HandleUserInput();
                }
                else
                {
                    System.Console.WriteLine("\n\t Command not recognized, please try again.\n");
                }
            }
        }

        private static void DisplayMenu()
        {
            System.Console.WriteLine("\n[1] Review Session");
            System.Console.WriteLine("[2] View review stats");
            System.Console.WriteLine("[3] View all cards");
            System.Console.WriteLine("[4] Create a new card");
            System.Console.WriteLine("[5] Edit a card");
            System.Console.WriteLine("[6] Delete a card");
            System.Console.WriteLine("[7] Delete all cards");
            System.Console.WriteLine("[0] Exit\n");
        }

        async private Task<List<Flashcard>> CreateReviewSession(List<Flashcard> contents)
        {
            // for tracking which words are wrong & being used to re-review
            List<Flashcard> failedWords = new List<Flashcard> { };
            // for updating the review table
            List<WordTracker> reviewedWords = new List<WordTracker> {};

            foreach (Flashcard card in contents)
            {
                System.Console.WriteLine($"\n{card.Word}\n");
                string userAnswer = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(userAnswer));

                if (userAnswer.ToLower() == card.Definition?.ToLower())
                {
                    System.Console.WriteLine("\n\t CORRECT \n");

                    WordTracker wordTrack = new WordTracker();

                    wordTrack.Word = card.Word;
                    wordTrack.Correct = 1;

                    reviewedWords.Add(wordTrack);
                }
                else
                {
                    System.Console.WriteLine("\n\t INCORRECT... \n");
                    failedWords.Add(card);

                    WordTracker wordTrack = new WordTracker();

                    wordTrack.Word = card.Word;
                    wordTrack.Incorrect = 1;

                    reviewedWords.Add(wordTrack);
                }

                System.Console.WriteLine($"\n {"Id",0} {"|",10} {"Word",10} {"|",10} {"Definition",25} {"|",10} {"Example",10} {"|",10} {"Reading",10} {"|",10} {"Difficulty",10} \n");

                CreateLineBreak();

                System.Console.WriteLine($"\n {card.Id,0} {"|",10} {card.Word,10} {"|",10} {card.Definition,25} {"|",10} {card.Example,10} {"|",10} {card.Reading,10} {"|",10} {card.Difficulty,10} \n");

                CreateLineBreak();
            };

            // send request update review table endpoint
            HttpClient reviewUpdateClient = new HttpClient();

            // not serializing things correctly?
            string updatedReviews = JsonSerializer.Serialize<List<WordTracker>>(reviewedWords);

            StringContent reviewContent = new StringContent(updatedReviews, Encoding.UTF8, "application/json");

            await reviewUpdateClient.PostAsync($"{uri}/updateReview", reviewContent);

            return failedWords;
        }

        async private Task<Flashcard> FillOutFlashcard()
        {
            Flashcard card = new Flashcard();

            bool gotWord = false;

            while (!gotWord)
            {
                System.Console.WriteLine("\n\t Word in Japanese:");
                card.Word = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(card.Word));

                if (card.Word.Length != 0)
                {
                    gotWord = true;
                }
                else
                {
                    System.Console.WriteLine("\n\t Please enter a word\n");
                    HandleUserInput();
                }
            }

            bool gotDefinition = false;
            while (!gotDefinition)
            {
                System.Console.WriteLine("\n\t Definition:");

                card.Definition = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(card.Definition));

                if (card.Definition.Length != 0)
                {
                    gotDefinition = true;
                }
                else
                {
                    System.Console.WriteLine("\n\t Please enter a definition\n");
                }
            }

            System.Console.WriteLine("\n\t Example:");

            System.Console.WriteLine("\n\t Would you like to auto-generate your example sentence? Y/N");

            string? autoGenSentenceResponse = System.Console.ReadLine();

            if (autoGenSentenceResponse?.ToLower() == "y")
            {
                card.Example = await scrapper.ScrapSentencesAsync(card.Word);
            }
            else
            {
                System.Console.WriteLine("\n\t Please type your example sentence, or leave blank");

                card.Example = System.Console.ReadLine();
            }
            
            System.Console.WriteLine("\n\t Reading:");
            card.Reading = System.Console.ReadLine();

            System.Console.WriteLine("\n\t Difficulty:");
            card.Difficulty = System.Console.ReadLine();

            return card;
        }

        async private Task<Flashcard> AutoFillCardAsync()
        {
            Flashcard newCard = new Flashcard();

            System.Console.WriteLine("\n\t Which word would you like to autofill? \n");
            
            string desiredWord = System.Console.ReadLine() ?? throw new NullReferenceException(nameof(desiredWord));

            HttpClient jishoClient = new HttpClient();

            HttpResponseMessage rawWordData = await jishoClient.GetAsync($"https://jisho.org/api/v1/search/words?keyword={desiredWord}");

            string autofillResponse = await rawWordData.Content.ReadAsStringAsync();

            AutoFillFlashcard autoFilledData = JsonSerializer.Deserialize<AutoFillFlashcard>(autofillResponse) ?? throw new NullReferenceException(nameof(autoFilledData));

            try
            {
                newCard.Word = autoFilledData?.data?[0].slug ?? throw new NullReferenceException(nameof(newCard.Word));

                newCard.Definition = autoFilledData?.data?[0]?.senses?[0]?.english_definitions?[0] ?? throw new NullReferenceException(nameof(newCard.Definition));

                newCard.Example = await scrapper.ScrapSentencesAsync(newCard.Definition);

                newCard.Reading = autoFilledData?.data?[0]?.japanese?[0].reading;

                newCard.Difficulty = autoFilledData?.data[0].jlpt?[0];
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"\n\t Autocomplete failed, please enter your new card manually: {ex}");

                newCard = await FillOutFlashcard();
            }

            return newCard;    
        }

        async private Task ParseResponseAsync(HttpResponseMessage response)
        {
            try
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                string contents = JsonSerializer.Deserialize<string>(responseContent) ?? throw new NullReferenceException(nameof(contents));
                
                System.Console.WriteLine($"\n{contents}\n");
                HandleUserInput();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Failed to parse the http response, please try again: {ex}");
                HandleUserInput();
            }
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
                    System.Console.WriteLine("\n\t Please type a valid number \n");
                }
            }

            return cardId;
        }

        private static void CreateLineBreak()
        {
            System.Console.WriteLine("\n -------------------------------------------------------------------------------------------------------------------------------------------------------------\n");
        }
    }
}
