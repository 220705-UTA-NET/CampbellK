using System;

namespace Flash.Console.UserInterface
{
    public class SentenceScrapper
    {
        string baseUri = "https://jisho.org/search/";

        async public Task<string> ScrapSentencesAsync(string? word)
        {
            string finishedExampleSentence = "";

            HttpClient client = new HttpClient();

            // grab the html of the sentence page of the given word
            var pageInformation = await client.GetAsync($"{baseUri}/{word}%23sentences");
            string pageContent = await pageInformation.Content.ReadAsStringAsync();

            try
            {
                // between first japanese sentence & first english sentence will contain within it the sentence I need
                int firstJapaneseSentence = pageContent.IndexOf("japanese_sentence");
                int firstEnglishSentence = pageContent.IndexOf("english_sentence");
                int length = firstEnglishSentence - firstJapaneseSentence;

                string subStringContent = pageContent.Substring(firstJapaneseSentence, length);

                // substring again to extrapolate each sentence. avoid the furigana by looking only for <span class="unlinked">, where the kanji/kana appears right after
                // split substring on closing tag
                string[] splitContent = subStringContent.Split(">");

                // can search each part, is there content between the start index & the next <
                string sentence = "";

                for (int i = 0; i < splitContent.Length; i++)
                {
                    //System.Console.WriteLine(splitContent[i]);
                    // if part contains "unlinked", then grab beginning of NEXT part (up until the next <) which contains the kanji/kana
                    // avoid grabbing the furigana
                    if (splitContent[i].Contains("unlinked"))
                    {
                        sentence += splitContent[i + 1];
                    }
                }

                // substring has difficulties with parsing through kana/kanji, so removing the unecessary content like this instead
                finishedExampleSentence = sentence.Replace("</span", "");
                System.Console.WriteLine(finishedExampleSentence);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Failed to parse auto-generated sentence, setting empty default: {ex}");
            }

            return finishedExampleSentence;
        }
    }
}
