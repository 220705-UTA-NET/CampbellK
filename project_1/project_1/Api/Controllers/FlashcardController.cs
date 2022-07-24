using Microsoft.AspNetCore.Mvc;
using Flash.Data;
using System.Text.Json;
using System.Data.SqlClient;

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
            Database dbConn = new Database();
            List<Flashcard> allFlashcards = dbConn.FetchAllFlashcards();

            // not converting the list to json; just returning empty
            string jsonContent = JsonSerializer.Serialize(allFlashcards);
            ContentResult response = new ContentResult()
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
            Database dbConn = new Database();
            // parse out data from body
            int status = dbConn.CreateNewCard(newFlashcard);

            string jsonContent = JsonSerializer.Serialize($"New card successfully created: {status}");
            ContentResult response = new ContentResult()
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
            Database dbConn = new Database();
            int status = dbConn.EditCard(updatedCard, cardId);

            string jsonContent = JsonSerializer.Serialize($"Card {updatedCard.Word} has been updated: {status}");
            ContentResult response = new ContentResult()
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = jsonContent
            };

            return response;
        }

        // Deleting a single card
        [HttpDelete("/deleteCard/{cardId}")]
        public ContentResult DeleteCard(int cardId)
        {
            Database dbConn = new Database();
            int status = dbConn.DeleteCard(cardId);

            string jsonContent = JsonSerializer.Serialize($"Card {cardId} has been deleted: {status}");
            ContentResult response = new ContentResult()
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = jsonContent
            };

            return response;
        }

        // Deleting all cards
        [HttpDelete("/deleteAllCards")]
        public ContentResult DeleteAllCards()
        {
            Database dbConn = new Database();
            int status = dbConn.DeleteAllCards();

            string jsonContent = JsonSerializer.Serialize($"All cards have been successfully deleted: {status}");
            ContentResult response = new ContentResult()
            {
                StatusCode = 200,
                ContentType = "application/json",
                Content = jsonContent
            };

            return response;
        }
    }
}
