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
            List<Article<string, int>> articles = wordExtractor.ExtractFeatures(@"..\..\Books\Reuters_34");
            foreach(Article<string, int> article in articles)
            {
                Console.WriteLine(string.Format("ARTICLE: {0}\n", article.FileName));
                Console.WriteLine(article);
                Console.WriteLine("\n------------------------------------------------------\n");
            }
            Console.ReadKey();
        }
    }
}
