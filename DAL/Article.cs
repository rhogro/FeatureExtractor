using System;
using System.Collections.Generic;
using System.Text;

namespace DAL
{
    public class Article
    {
        public string FileName { get; set; }
        public string[] Title { get; set; }
        public Dictionary<string, int> Words { get; set; }
        public Dictionary<string, int> AllWords { get; set; }
        public List<string> Topics { get; set; }

        public Article()
        {
            Words = new Dictionary<string, int>();
        }

        public Article(string[] title, Dictionary<string, int> words, string fileName, List<string> topics)
        {
            Title = title;
            Words = words;
            FileName = fileName;
            Topics = topics;
            
            AllWords = words;

            for(int i=0;i < title.Length; i++)
            {
                if (AllWords.ContainsKey(title[i]))
                {
                    AllWords[title[i]] += 2;
                }
                else
                {
                    AllWords.Add(title[i], 2);
                }
            }

        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Title");
            foreach (string word in Title)
            {
                builder.AppendLine(string.Format("{0}", word));
            }
            builder.AppendLine("\n");
            builder.AppendLine(string.Format("Number of words: {0}", Title.Length + Words.Count));
            builder.Append("Topics:");
            foreach(string topic in Topics)
            {
                builder.Append(string.Format("{0}, ", topic));
            }
            builder.AppendLine("\n");
            foreach (KeyValuePair<string, int> kvp in Words)
            {
                builder.AppendLine(string.Format("Word: \"{0}\" - Count: {1}", kvp.Key, kvp.Value));
            }
            return builder.ToString();
        }
    }
}
