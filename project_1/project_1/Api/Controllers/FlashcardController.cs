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

        // For viewing all cards as a ledger. Returns a list of all cards to be displayed for user
        [HttpGet("/viewAllCards")]
        // For studying all cards
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
    }
}
