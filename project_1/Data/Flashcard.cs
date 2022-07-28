namespace Flash.Data
{
    public class Flashcard
    {
        public int? Id { get; set; }
        public string? Word { get; set; }
        public string? Definition { get; set; }
        public string? Example { get; set; }
        public string? Difficulty { get; set; }
        public string? Reading { get; set; }
    }

    // for reviews
    public class WordTracker
    {
        public string? Word {get; set;}
        public int Correct {get; set;}
        public int Incorrect {get; set;}
    }
}