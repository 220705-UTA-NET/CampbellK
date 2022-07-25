using System;

namespace Flash.Console.UserInterface
{
    public class AutoFillFlashcard
    {
        public Dictionary<string, int> meta { get; set; }
        public List<JishoEntry> data { get; set; }
    }

    public class JishoEntry
    {
        public string slug { get; set; }
        public bool is_common { get; set; }
        public List<string> tags { get; set; }
        public List<string> jlpt { get; set; }
        public List<WordAndReading> japanese { get; set; }
        public List<Sense> senses { get; set; }
        public Attribution attribution { get; set; }
    }

    public class WordAndReading
    {
        public string word { get; set; }
        public string reading { get; set; }
    }

    public class Sense
    {
        public List<string> english_definitions { get; set; }
        public List<string> parts_of_speech { get; set; }
        public List<Dictionary<string, string>> links { get; set; }
        public List<string> tags { get; set; }
        public List<string> restrictions { get; set; }
        public List<string> see_also { get; set; }
        public List<string> antonyms { get; set; }
        public List<string> source { get; set; }
        public List<string> info { get; set; }
    }

    public class Attribution
    {
        public bool jmdict { get; set; }
        public bool jmnedict { get; set; }
        // if it exists, it is a string. if it doesn't, returns false bool. thus must use dynamic
        public dynamic dbpedia { get; set; }
    }
}
