using DAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Worker
{
    public class WordExtractor
    {
        private string FolderPath;
        public SortedDictionary<string, double> AllUniqueWords;
        public Dictionary<string, int> AllTopicsAndCounts;
        public Dictionary<string, int> FirstTopicOfEachArticleAndCounts;
        List<Article> Articles;
        public int[,] Matrix { get; set; }
        public List<string>[] ArticleTopics { get; set; }
        public double Entropy { get; set; }
        private Stopwatch _stopwatch;

        public WordExtractor(string folderPath)
        {
            FolderPath = folderPath;
            AllUniqueWords = new SortedDictionary<string, double>();
            AllTopicsAndCounts = new Dictionary<string, int>();
            FirstTopicOfEachArticleAndCounts = new Dictionary<string, int>();
            Articles = new List<Article>();
        }

        public void Start()
        {
            long elapsedMilliseconds;
            Console.WriteLine("Execution started at {0}", DateTime.Now);
            _stopwatch = Stopwatch.StartNew();
            Console.WriteLine("\nInitializing Dataset");
            InitializeArticles();
            elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Dataset initalized in {0} milliseconds", WriteTime(elapsedMilliseconds));
            Console.WriteLine("Number of articles in dataset = {0}", Articles.Count);
            Console.WriteLine("Unique words = {0}", AllUniqueWords.Count);
            Console.WriteLine("Unique topics = {0}", AllTopicsAndCounts.Count);
            Entropy = ComputeEntropy(FirstTopicOfEachArticleAndCounts);
            Console.WriteLine("\nDataset entropy = {0}", Entropy);
            Console.WriteLine("\nComputing information gain");
            AllUniqueWords = ComputeGain();
            FilterArticlesByGain();
            elapsedMilliseconds = _stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
            Console.WriteLine("Information gain computed in {0} milliseconds", WriteTime(elapsedMilliseconds));
            Console.WriteLine("Dataset filtered by information gain");
            Console.WriteLine("Number of articles in dataset = {0}", Articles.Count);
            Console.WriteLine("Unique words = {0}", AllUniqueWords.Count);
            Console.WriteLine("Unique topics = {0}", AllTopicsAndCounts.Count);
            Console.WriteLine("\nComputing dataset matrix");
            ArticleTopics = new List<string>[Articles.Count];
            Matrix = GetMatrix();
            elapsedMilliseconds = _stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
            Console.WriteLine("Dataset matrix computed in {0} milliseconds", WriteTime(elapsedMilliseconds));

            Console.WriteLine("\nWriting .arff file");
            FileWriter fileWriter = new FileWriter();
            fileWriter.WriteToFile(FolderPath, AllUniqueWords, AllTopicsAndCounts, Matrix, ArticleTopics);
            elapsedMilliseconds = _stopwatch.ElapsedMilliseconds - elapsedMilliseconds;
            Console.WriteLine("File written in {0} milliseconds", WriteTime(elapsedMilliseconds));
            Console.WriteLine("\n Execution finished at {0}", DateTime.Now);
            _stopwatch.Stop();
            elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Total execution time: {0}", WriteTime(elapsedMilliseconds));
        }

        private string WriteTime(long milliseconds)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(milliseconds);
            return string.Format("{0:D2}m:{1:D2}s:{2:D3}ms",
                                    t.Minutes,
                                    t.Seconds,
                                    t.Milliseconds);
        }

        public void InitializeArticles()
        {
            string[] files = Directory.GetFiles(FolderPath);
            foreach(var file in files)
            {
               string text = File.ReadAllText(file);
               string[] title = ExtractTitle(text);
               Dictionary<string, int> words = ExtractWords(text);
               List<string> topics = ExtractArticleTopics(file);
               Article article = new Article(title, words, file, topics);
               Articles.Add(article);
            }

            RemoveMostAndLessUsedTopics();
        }

        private void RemoveMostAndLessUsedTopics()
        {
            List<string> mostUsedTopics = AllTopicsAndCounts.Where(t => t.Value >= Articles.Count * 0.95).Select(t => t.Key).ToList();
            List<string> lessUsedTopics = AllTopicsAndCounts.Where(t => t.Value <= Articles.Count * 0.05).Select(t => t.Key).ToList();
            if (mostUsedTopics.Count > 0)
            {
                mostUsedTopics.ForEach(mostUsedTopic =>
                {
                    AllTopicsAndCounts.Remove(mostUsedTopic);
                    FirstTopicOfEachArticleAndCounts.Remove(mostUsedTopic);
                    Articles.ForEach(a => a.Topics.Remove(mostUsedTopic));
                });
            }

            if (lessUsedTopics.Count > 0)
            {
                lessUsedTopics.ForEach(lessUsedTopic =>
                {
                    AllTopicsAndCounts.Remove(lessUsedTopic);
                    FirstTopicOfEachArticleAndCounts.Remove(lessUsedTopic);
                    Articles.ForEach(a => a.Topics.Remove(lessUsedTopic));
                });
            }

            Articles.RemoveAll(a => a.Topics.Count == 0);
        }

        public double ComputeEntropy(Dictionary<string, int> datasetTopicAndCounts)
        {
            double fractionSum = 0;
            double entropy = 0;

            var datasetCount = datasetTopicAndCounts.CountValues();

            foreach(var topic in datasetTopicAndCounts)
            {
                double p = (double)topic.Value / datasetCount;
                fractionSum += p;
                entropy = entropy - p * Math.Log(p, 2);
            }

            if (fractionSum < 0.9999999 || fractionSum > 1.00000001)
            {
                Console.WriteLine("-------- !!! SUM OF FRACTIONS IS NOT 1 !!! --------");
            }
            return entropy;
        }

        public SortedDictionary<string, double> ComputeGain()
        {
            double gain;
            SortedDictionary<string, double> allUniqueWords = new SortedDictionary<string, double>();
            foreach(var word in AllUniqueWords)
            {
                double sum = 0;
                int valueTypesCount = Enum.GetNames(typeof(AttributeValueType)).Length;
                for (int i = 0; i < valueTypesCount; i++)
                {
                    List<Article> articleSubset = GetArticleSubsetForAttributeValueType((AttributeValueType)i, word.Key);

                    if (articleSubset.Count != 0)
                    {
                        Dictionary<string, int> subsetFirstTopicsAndCounts = GetSubsetFirstTopicsAndCounts(articleSubset);
                        sum += (double)subsetFirstTopicsAndCounts.Count / Articles.Count * ComputeEntropy(subsetFirstTopicsAndCounts);
                    }
                }

                gain = Entropy - sum;
                if (gain < 0 && gain > Entropy)
                {
                    Console.WriteLine("-------- !!! WRONG GAIN !!! --------");
                }
                allUniqueWords.Add(word.Key, gain);
            }

            return allUniqueWords;
        }

        private List<Article> GetArticleSubsetForAttributeValueType(AttributeValueType attributeValueType, string word)
        {
            int highestWordCount = 0;

            foreach(var article in Articles)
            {
                int value;
                if (article.AllWords.TryGetValue(word, out value) && value > highestWordCount)
                {
                    highestWordCount = value;
                }
            }

            List<Article> articles;
            switch (attributeValueType)
            {
                case AttributeValueType.Zero:
                    articles = GetArticleSubsetForAttributeTypeZero(word);
                    break;
                case AttributeValueType.LessHalf:
                    articles = GetArticleSubsetForAttributeTypeLessHalf(word, highestWordCount);
                    break;
                case AttributeValueType.MoreHalf:
                    articles = GetArticleSubsetForAttributeTypeMoreHalf(word, highestWordCount);
                    break;
                default: throw new Exception("Article subset selection not implemented for this Attribute Value Type");
            }

            return articles;
        }

        private List<Article> GetArticleSubsetForAttributeTypeZero(string word)
        {
            return Articles.Where(article => !article.AllWords.ContainsKey(word)).ToList();
        }

        private List<Article> GetArticleSubsetForAttributeTypeLessHalf(string word, int highestWordCount)
        {
            return Articles.Where(article => article.AllWords.ContainsKey(word) &&
                article.AllWords[word] < (double)highestWordCount / Constants.AttributesCountDivisionTreshold).ToList();
        }

        private List<Article> GetArticleSubsetForAttributeTypeMoreHalf(string word, int highestWordCount)
        {
            return Articles.Where(article => article.AllWords.ContainsKey(word) &&
                article.AllWords[word] >= (double)highestWordCount / Constants.AttributesCountDivisionTreshold).ToList();
        }

        private Dictionary<string, int> GetSubsetFirstTopicsAndCounts(List<Article> articleSubset)
        {
            Dictionary<string, int> firstTopicsAndCounts = new Dictionary<string, int>();
            foreach(var article in articleSubset)
            {
                if (firstTopicsAndCounts.ContainsKey(article.Topics[0]))
                {
                    firstTopicsAndCounts[article.Topics[0]] = ++firstTopicsAndCounts[article.Topics[0]];
                }
                else
                {
                    firstTopicsAndCounts.Add(article.Topics[0], 1);
                }
            }

            return firstTopicsAndCounts;
        }

        private void FilterArticlesByGain()
        {
            List<Article> articles = new List<Article>();
            List<string> words = AllUniqueWords.OrderBy(word => word.Value)
                .Take((int)(AllUniqueWords.Count * (1 - Constants.FilterByGainTresholdPercentage))).Select(w => w.Key).ToList();

            foreach(var word in words)
            {
                AllUniqueWords.Remove(word);
                Articles.ForEach(article => article.AllWords.Remove(word));
            }

            Articles.RemoveAll(a => a.AllWords.Count == 0);
        }

        private int[,] GetMatrix()
        {
            int[,] datasetMatrix = new int[Articles.Count, AllUniqueWords.Count];
            int wordsCounter = 0, articlesCounter = 0;
            foreach(var article in Articles)
            {
                foreach(var word in AllUniqueWords)
                {
                    if (article.Words.ContainsKey(word.Key))
                    {
                        datasetMatrix[articlesCounter, wordsCounter] = article.Words[word.Key];
                    }
                    else
                    {
                        datasetMatrix[articlesCounter, wordsCounter] = 0;
                    }
                    wordsCounter++;
                }

                ArticleTopics[articlesCounter] = article.Topics;
                articlesCounter++;
                wordsCounter = 0;
            }

            return datasetMatrix;
        }

        private List<string> ExtractArticleTopics(string file)
        {
            List<string> topics = new List<string>();
            string[] text = File.ReadAllLines(file);
            List<string> interestingLines = new List<string>();
            bool shouldTakelines = false; ;
            foreach (string line in text)
            {
                if (line.Contains("class=\"bip:topics"))
                {
                    shouldTakelines = true;
                }
                if (shouldTakelines)
                {
                    if (line.Contains("</codes>"))
                    {
                        break;
                    }
                    if (line.Contains("code="))
                    {
                        interestingLines.Add(line);
                    }
                }
            }

            string topic = Regex.Match(interestingLines[0], "\".*?\"").Value.Trim('\"');
            if (FirstTopicOfEachArticleAndCounts.ContainsKey(topic))
            {
                FirstTopicOfEachArticleAndCounts[topic] = ++FirstTopicOfEachArticleAndCounts[topic];
            }
            else
            {
                FirstTopicOfEachArticleAndCounts.Add(topic, 1);
            }

            foreach(var line in interestingLines)
            {
                topic = Regex.Match(line, "\".*?\"").Value.Trim('\"');
                topics.Add(topic);

                if (AllTopicsAndCounts.ContainsKey(topic))
                {
                    AllTopicsAndCounts[topic] = ++AllTopicsAndCounts[topic];
                }
                else
                {
                    AllTopicsAndCounts.Add(topic, 1);
                }
            }

            return topics;
        }

        private Dictionary<string, int> ExtractWords(string text)
        {
            text = text.Replace("\r", "").Replace("\n", "").Replace("<p>", " ").Replace("</p>", " ").Replace("quot;", "");
            string[] words = text.ExtractWords("<text>", "</text>");
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach(var word in words)
            {
                if (dictionary.ContainsKey(word))
                {
                    dictionary[word] = ++dictionary[word];
                }
                else
                {
                    dictionary.Add(word, 1);
                }

                if (!AllUniqueWords.ContainsKey(word))
                {
                    AllUniqueWords.Add(word, 0);
                }
            }

            return dictionary;
        }

        private string[] ExtractTitle(string text)
        {
            string[] words = text.ExtractWords("<title>", "</title>");
            foreach(var word in words)
            {
               if (!AllUniqueWords.ContainsKey(word))
               {
                   AllUniqueWords.Add(word, 0);
               }
            }

            return words;
        }

    }
}
