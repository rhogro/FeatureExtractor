using DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Worker
{
    public class WordExtractor
    {
        public List<Article<string, int>> ExtractFeatures(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);
            List<Article<string, int>> articles = new List<Article<string, int>>();
            foreach(string file in files)
            {
                string[] title = ExtractTitle(file);
                Dictionary<string, int> words = ExtractWords(file);
                Article<string, int> article = new Article<string, int>(title, words, file);
                articles.Add(article);
            }
            return articles;
        }

        private Dictionary<string, int> ExtractWords(string file)
        {
            string text = File.ReadAllText(file).Replace("\r", "").Replace("\n", "").Replace("<p>", "").Replace("</p>", "");
            string[] words = text.ExtractWords("<text>", "</text>");
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach(string word in words)
            {
                if (dictionary.ContainsKey(word))
                {
                    dictionary[word] = ++dictionary[word];
                }
                else
                {
                    dictionary.Add(word, 1);
                }
            }
            return dictionary;
        }

        private string[] ExtractTitle(string file)
        {
            string text = File.ReadAllText(file);
            return text.ExtractWords("<title>", "</title>");
        }

    }
}
