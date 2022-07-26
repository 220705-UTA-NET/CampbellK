namespace Flash.Data
{
    public class Flashcard
    {
        public int? Id { get; set; }
        public string? Word { get; set; }
        public string? Definition { get; set; }
        public string? Example { get; set; }
        public string? Difficulty { get; set; }
        public string? Notes { get; set; }
        public DateTime lastReviewed { get; set; }
        public DateTime nextReview { get; set; }
    }
}