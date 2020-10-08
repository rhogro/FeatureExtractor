using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worker
{
    public static class StringHelpers
    {
        public static string[] ExtractWords(this string text, string from, string to)
        {
            int pFrom = text.IndexOf(from) + from.Length;
            int pTo = text.LastIndexOf(to);
            string[] uncleanWords = text.Substring(pFrom, pTo - pFrom).Split(' ');
            List<string> cleanedWords = new List<string>();
            foreach(string word in uncleanWords)
            {
                cleanedWords.Add(word.Trim().Trim(',', '.').ToLowerInvariant());
            }
            return cleanedWords.ToArray();
        }
    }
}
