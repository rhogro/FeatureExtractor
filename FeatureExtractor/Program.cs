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
        private const string Reuters34Path = @"..\..\Books\Reuters_34\Training";
        private const string TestPath = @"..\..\Books\Test";
        static void Main(string[] args)
        {
            WordExtractor wordExtractor = new WordExtractor(TestPath);
            Console.WriteLine("Entropy = " + wordExtractor.Entropy);
            Console.ReadKey();
        }
    }
}
