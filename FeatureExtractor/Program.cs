using DAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worker;

namespace FeatureExtractor
{
    class Program
    {
        private const string ReutersSmall = @"..\..\Books\Reuters_34";
        private const string ReutersBig = @"..\..\Books\Reuters_7083";
        private const string TestPath = @"..\..\Books\Test";
        static void Main(string[] args)
        {
            WordExtractor wordExtractor = new WordExtractor(ReutersBig);
            wordExtractor.Start();
            Console.ReadKey();
        }
    }
}
