using System;
using System.Collections.Generic;
using System.Text;

namespace DAL
{
    public class Article<T, Y>
    {
        public string FileName { get; set; }
        public string[] Title { get; set; }
        public Dictionary<T,Y> Words { get; set; }

        public Article()
        {
            Words = new Dictionary<T, Y>();
        }

        public Article(string[] title, Dictionary<T,Y> words, string fileName)
        {
            Title = title;
            Words = words;
            FileName = fileName;
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
            foreach (KeyValuePair<T,Y> kvp in Words)
            {
                builder.AppendLine(string.Format("Word: {0} - Count: {1}", kvp.Key, kvp.Value));
            }
            return builder.ToString();
        }
    }
}
