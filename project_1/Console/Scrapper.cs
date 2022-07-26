using System;

namespace Flash.Console.UserInterface
{
    public class SentenceScrapper
    {
        string baseUri = "https://jisho.org/search/";
        // https://jisho.org/search/%E3%81%82%E3%81%97%E3%81%9F%20%23sentences
        async public void ScrapSentencesAsync(string word)
        {
            HttpClient client = new HttpClient();

            // grab the html of the sentence page of the given word
            var pageInformation = await client.GetAsync($"{baseUri}/{word}%23sentences");
            string pageContent = await pageInformation.Content.ReadAsStringAsync();

            // between first japanese sentence & first english sentence will contain within it the sentence I need
            int firstJapaneseSentence = pageContent.IndexOf("japanese_sentence");
            int firstEnglishSentence = pageContent.IndexOf("english_sentence");
            int length = firstEnglishSentence - firstJapaneseSentence;

            string subStringContent = pageContent.Substring(firstJapaneseSentence, length);
            System.Console.WriteLine(subStringContent);

            // substring again to extrapolate each sentence. avoid the furigana by looking only for <span class="unlinked">, where the kanji/kana appears right after

            // split substring on closing tag
            string[] splitContent = subStringContent.Split(">");

            // can search each part, is there content between the start index & the next <
            string sentence = "";
            foreach(string part in splitContent)
            {
                System.Console.WriteLine(part);

                // string += part (IF IT CONTAINS WHAT I WANT, needs to be parsed)
            }


        }
    }
}
