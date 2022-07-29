using Microsoft.AspNetCore.Mvc;
using Flash.Data;
using System.Text.Json;

namespace Flash.Api.Controllers
{
    public class FlashcardController : Controller
    {
        private readonly ILogger<FlashcardController> logger;

        public FlashcardController(ILogger<FlashcardController> logger)
        {
            this.logger = logger;
        }

        // Viewing all cards as a ledger. Returns a list of all cards to be displayed for user
        [HttpGet("/viewAllCards")]
        // Studying all cards
        [HttpGet("/reviewAll")]
        public ContentResult ViewAllCards()
        {
            ContentResult response = new ContentResult();

            try
            {
                Database dbConn = new Database();
                List<Flashcard> allFlashcards = dbConn.FetchAllFlashcards();

                // not converting the list to json; just returning empty
                string jsonContent = JsonSerializer.Serialize(allFlashcards);
                response = new ContentResult()
                {
                    StatusCode = 200,
                    ContentType = "application/json",
                    Content = jsonContent
                };

                logger.LogInformation(jsonContent);
            }
            catch (Exception ex)
            {
                logger.LogError($"ViewAllCards error: {ex}");
                response.StatusCode = 500;
            }

            return response;
        }

        [HttpGet("/viewReviewStats")]
        public ContentResult ViewReviewStats()
        {
            ContentResult response = new ContentResult();

            Database dbConn = new Database();

            List<WordTracker> wordStats = dbConn.ViewReviewStats();

            string jsonContent = JsonSerializer.Serialize<List<WordTracker>>(wordStats);

            response = new ContentResult()
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = jsonContent
            };

            return response;
        }

        // Creating a new card
        [HttpPost("/addNewCard")]
        public ContentResult CreateNewCard([FromBody] Flashcard newFlashcard)
        {
            ContentResult response = new ContentResult();

            try
            {
                Database dbConn = new Database();

                int status = dbConn.CreateNewCard(newFlashcard);

                string jsonContent = JsonSerializer.Serialize($"New card successfully created: {status}");
                response = new ContentResult()
                {
                    StatusCode = 200,
                    ContentType = "application/json",
                    Content = jsonContent
                };

                logger.LogInformation($"Created new card: {jsonContent}");
            }
            catch (Exception ex)
            {
                logger.LogError($"addNewCard error: {ex}");
                response.StatusCode = 500;
            }

            return response;
        }

        [HttpPost("/updateReview")]
        public ContentResult UpdateReview([FromBody] List<WordTracker> reviewUpdates)
        {
            ContentResult response = new ContentResult();

            Database dbConn = new Database();

            dbConn.UpdateReview(reviewUpdates);

            string jsonContent = JsonSerializer.Serialize("Reviews have been updated");
            response = new ContentResult()
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = jsonContent
            };

            return response;
        }

        // Editing an existing card
        [HttpPut("/editCard/{cardId}")]
        public ContentResult EditCard([FromBody] Flashcard updatedCard, int cardId)
        {
            ContentResult response = new ContentResult();

            try
            {
                Database dbConn = new Database();
                int status = dbConn.EditCard(updatedCard, cardId);

                string jsonContent = JsonSerializer.Serialize($"Card {updatedCard.Word} has been updated: {status}");
                response = new ContentResult()
                {
                    StatusCode = 200,
                    ContentType = "application/json",
                    Content = jsonContent
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"editCard error: {ex}");
                response.StatusCode = 500;
            }

            return response;
        }

        // Deleting a single card
        [HttpDelete("/deleteCard/{cardId}")]
        public ContentResult DeleteCard(int cardId)
        {
            ContentResult response = new ContentResult();

            try
            {
                Database dbConn = new Database();
                int status = dbConn.DeleteCard(cardId);

                string jsonContent = JsonSerializer.Serialize($"Card {cardId} has been deleted: {status}");
                response = new ContentResult()
                {
                    StatusCode = 200,
                    ContentType = "application/json",
                    Content = jsonContent
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"deleteCard error: {ex}");
                response.StatusCode = 500;
            }

            return response;
        }

        // Deleting all cards
        [HttpDelete("/deleteAllCards")]
        public ContentResult DeleteAllCards()
        {
            ContentResult response = new ContentResult();

            try
            {
                Database dbConn = new Database();
                int status = dbConn.DeleteAllCards();

                string jsonContent = JsonSerializer.Serialize($"All cards have been successfully deleted: {status}");
                response = new ContentResult()
                {
                    StatusCode = 200,
                    ContentType = "application/json",
                    Content = jsonContent
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"deleteAllCards error: {ex}");
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
