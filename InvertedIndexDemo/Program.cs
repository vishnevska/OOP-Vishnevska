using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace InvertedIndexDemo
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public override string ToString() => $"[{Id}] {Title}";
    }

    public static class TextProcessor
    {
        private static readonly HashSet<string> StopWords = new HashSet<string>
        {
            "a", "an", "the", "is", "are", "was", "were",
            "in", "on", "at", "to", "for", "of", "and", "or"
        };
        public static IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Enumerable.Empty<string>();

            var matches = Regex.Matches(text.ToLowerInvariant(), @"[\w#]+");
            return matches.Select(m => m.Value);
        }
        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
        {
            return words.Where(w => !StopWords.Contains(w));
        }
        public static IEnumerable<string> NormalizeAndTokenize(string text)
        {
            return RemoveStopWords(Tokenize(text));
        }
    }

    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<int>> _index;
        private readonly Dictionary<int, Document> _documents;

        public InvertedIndex()
        {
            _index = new Dictionary<string, HashSet<int>>();
            _documents = new Dictionary<int, Document>();
        }

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;

        public void AddDocument(Document doc)
        {
            _documents[doc.Id] = doc;

            var tokens = TextProcessor.NormalizeAndTokenize(doc.Content);
            foreach (var word in tokens)
            {
                if (!_index.TryGetValue(word, out var docIds))
                {
                    docIds = new HashSet<int>();
                    _index[word] = docIds;
                }
                docIds.Add(doc.Id);
            }
        }

        public List<Document> SearchSingleWord(string word)
        {
            var normalized = TextProcessor.NormalizeAndTokenize(word).FirstOrDefault();
            if (string.IsNullOrEmpty(normalized))
                return new List<Document>();

            if (_index.TryGetValue(normalized, out var docIds))
            {
                return docIds.Select(id => _documents[id]).ToList();
            }

            return new List<Document>();
        }

        public List<Document> SearchMultipleWords(params string[] words)
        {
            var normalizedWords = words
                .SelectMany(w => TextProcessor.NormalizeAndTokenize(w))
                .Distinct()
                .ToList();

            if (normalizedWords.Count == 0)
                return new List<Document>();

            if (!_index.TryGetValue(normalizedWords[0], out var firstSet))
                return new List<Document>();

            var resultIds = new HashSet<int>(firstSet);

            for (int i = 1; i < normalizedWords.Count; i++)
            {
                if (_index.TryGetValue(normalizedWords[i], out var nextSet))
                {
                    resultIds.IntersectWith(nextSet);
                }
                else
                {
                    return new List<Document>();
                }
            }

            return resultIds.Select(id => _documents[id]).ToList();
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            var normalizedWords = words
                .SelectMany(w => TextProcessor.NormalizeAndTokenize(w))
                .Distinct()
                .ToList();

            var resultIds = new HashSet<int>();

            foreach (var word in normalizedWords)
            {
                if (_index.TryGetValue(word, out var docIds))
                {
                    resultIds.UnionWith(docIds);
                }
            }

            return resultIds.Select(id => _documents[id]).ToList();
        }
    }

    public static class PerformanceTest
    {
        public static List<Document> GenerateDocuments(int count)
        {
            string[] topics =
            {
                "C# is a modern object-oriented programming language developed by Microsoft.",
                "Java is a popular programming language known for its portability and performance.",
                "Python is a high-level programming language with simple syntax.",
                "C# supports LINQ, async/await, and many modern programming paradigms.",
                "C#, Java, and Python are among the most popular programming languages today.",
                "Databases use inverted indexes and hash tables for fast full-text search.",
                "Algorithms and data structures optimize memory and execution speed."
            };

            var list = new List<Document>(count);
            var rnd = new Random(42);

            for (int i = 1; i <= count; i++)
            {
                string part1 = topics[i % topics.Length];
                string part2 = topics[rnd.Next(topics.Length)];
                list.Add(new Document
                {
                    Id = i,
                    Title = $"Document #{i}",
                    Content = $"{part1} {part2} Additional keyword_batch_{i % 50}."
                });
            }

            return list;
        }

        public static void RunBenchmarkSuite()
        {
            int[] sizes = { 10, 100, 1000, 10000 };
            string searchWord = "programming";

            Console.WriteLine(" Вимірювання продуктивності (пошук слова 'programming') ");
            Console.WriteLine($"{"Документів",-12} | {"Індексація (мс)",-18} | {"Linear (тіки)",-16} | {"Index (тіки)",-15} | {"Прискорення",-14} | {"Збігів"}");
            Console.WriteLine(new string('-', 95));

            foreach (int size in sizes)
            {
                var docs = GenerateDocuments(size);
                var sw = new Stopwatch();

                var normalizedWord = TextProcessor.NormalizeAndTokenize(searchWord).First();
                _ = docs.Take(1).Where(d => TextProcessor.NormalizeAndTokenize(d.Content).Contains(normalizedWord)).ToList();
                sw.Restart();
                var linearResults = docs
                    .Where(d => TextProcessor.NormalizeAndTokenize(d.Content).Contains(normalizedWord))
                    .ToList();
                sw.Stop();
                long linearTicks = sw.ElapsedTicks;
                var index = new InvertedIndex();
                sw.Restart();
                foreach (var doc in docs)
                {
                    index.AddDocument(doc);
                }
                sw.Stop();
                double buildMs = sw.Elapsed.TotalMilliseconds;
                sw.Restart();
                var indexResults = index.SearchSingleWord(searchWord);
                sw.Stop();
                long indexTicks = Math.Max(1, sw.ElapsedTicks);

                double speedup = (double)linearTicks / indexTicks;

                Console.WriteLine($"{size,-12} | {buildMs,-18:F3} | {linearTicks,-16} | {indexTicks,-15} | {speedup,-13:F1}x | {linearResults.Count} == {indexResults.Count}");
            }
            Console.WriteLine();
        }
    }

    class Program
    {
        static List<Document> GetSampleDocuments()
        {
            return new List<Document>
            {
                new Document
                {
                    Id = 1,
                    Title = "Introduction to C#",
                    Content = "C# is a modern object-oriented programming language developed by Microsoft."
                },
                new Document
                {
                    Id = 2,
                    Title = "Java Basics",
                    Content = "Java is a popular programming language known for its portability and performance."
                },
                new Document
                {
                    Id = 3,
                    Title = "Python Overview",
                    Content = "Python is a high-level programming language with simple syntax."
                },
                new Document
                {
                    Id = 4,
                    Title = "C# Advanced Features",
                    Content = "C# supports LINQ, async/await, and many modern programming paradigms."
                },
                new Document
                {
                    Id = 5,
                    Title = "Comparing Languages",
                    Content = "C#, Java, and Python are among the most popular programming languages today."
                }
            };
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var index = new InvertedIndex();
            foreach (var doc in GetSampleDocuments())
            {
                index.AddDocument(doc);
            }

            Console.WriteLine($"Loaded {index.DocumentCount} documents");
            Console.WriteLine($"Index contains {index.UniqueWords} unique words\n");

            Console.WriteLine(" Тест 1: SearchSingleWord('c#') ");
            foreach (var d in index.SearchSingleWord("c#"))
                Console.WriteLine($"  {d}");

            Console.WriteLine("\n Тест 2: SearchMultipleWords('programming', 'language') [AND] ");
            foreach (var d in index.SearchMultipleWords("programming", "language"))
                Console.WriteLine($"  {d}");

            Console.WriteLine("\n Тест 3: SearchAnyWord('java', 'async') [OR] ");
            foreach (var d in index.SearchAnyWord("java", "async"))
                Console.WriteLine($"  {d}");

            Console.WriteLine();

            PerformanceTest.RunBenchmarkSuite();

            while (true)
            {
                Console.Write("Enter search query (or 'exit'): ");
                var query = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(query) || query.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine("\n Search Results (AND) ");
                var results = index.SearchMultipleWords(words);
                foreach (var doc in results)
                {
                    Console.WriteLine(doc);
                    Console.WriteLine($"  {doc.Content.Substring(0, Math.Min(100, doc.Content.Length))}...");
                }
                Console.WriteLine($"\nFound {results.Count} documents\n");
            }
        }
    }
}