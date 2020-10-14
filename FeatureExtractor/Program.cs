using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worker;

namespace FeatureExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            WordExtractor wordExtractor = new WordExtractor();
            List<Article<string, int>> articles = wordExtractor.ExtractFeatures(@"..\..\Books\Reuters_34\Training");
            List<string> documentsDictionary = new List<string>();
            foreach(Article<string, int> article in articles)
            {
                documentsDictionary.AddRange(article.Words.Keys);
                documentsDictionary.AddRange(article.Title);
                Console.WriteLine(string.Format("ARTICLE: {0}\n", article.FileName));
                Console.WriteLine(article);
                Console.WriteLine("\n------------------------------------------------------\n");
            }
            documentsDictionary = documentsDictionary.Distinct().ToList();
            Console.WriteLine("Documents dictionary count: " + documentsDictionary.Count);
            Console.ReadKey();
        }
    }
}
