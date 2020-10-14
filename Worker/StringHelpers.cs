using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Worker
{
    public static class StringHelpers
    {
        public static string[] ExtractWords(this string text, string from, string to)
        {
            int pFrom = text.IndexOf(from) + from.Length;
            int pTo = text.LastIndexOf(to);
            string[] uncleanWords = text.Substring(pFrom, pTo - pFrom).Replace("-", " ").Split(' ');
            List<string> cleanedWords = new List<string>();
            foreach(string word in uncleanWords)
            {
                string cleanWord = word.Trim().Trim(',', '.').ToLowerInvariant();
                cleanWord = StripPossesion(cleanWord);
                cleanWord = Regex.Replace(cleanWord,@"[^a-zA-Z -]", "");
                cleanWord = Regex.Replace(cleanWord, @"[\d-]", "");
                if (!string.IsNullOrWhiteSpace(cleanWord))
                {
                    cleanedWords.Add(cleanWord);
                }
                
            }
            return cleanedWords.ToArray();
        }

        private static string StripPossesion(string word)
        {
            if (word.Contains('\''))
            {
                int pos = word.IndexOf('\'');
                word = word.Substring(0, pos);
            }

            return word;
        }
    }
}
