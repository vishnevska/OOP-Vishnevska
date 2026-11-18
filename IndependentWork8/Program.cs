using System;
using System.Collections.Generic;
using System.Linq;

namespace IndependentWork8
{
    public class FileMetadata
    {
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; }

        public FileMetadata(string fileName, long size, DateTime lastModified, string hash)
        {
            FileName = fileName;
            Size = size;
            LastModified = lastModified;
            Hash = hash;
        }

        public override string ToString()
        {
            return $"{FileName} | {Size} байт | Змінено: {LastModified:dd.MM.yyyy HH:mm} | Hash: {Hash}";
        }
    }

    public class FileMetadataCache
    {
        private readonly Dictionary<string, FileMetadata> _cache = new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

        public void Add(string filePath, FileMetadata metadata)
        {
            _cache[filePath] = metadata;
        }

        public FileMetadata? Get(string filePath)
        {
            _cache.TryGetValue(filePath, out FileMetadata? metadata);
            return metadata;
        }

        public bool Remove(string filePath)
        {
            return _cache.Remove(filePath);
        }

        public bool Contains(string filePath)
        {
            return _cache.ContainsKey(filePath);
        }
    }

    public class TagManager
    {
        private readonly Dictionary<string, HashSet<string>> _articleTags = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        public void AddTag(string articleId, string tag)
        {
            if (!_articleTags.ContainsKey(articleId))
            {
                _articleTags[articleId] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            _articleTags[articleId].Add(tag);
        }

        public void RemoveTag(string articleId, string tag)
        {
            if (_articleTags.TryGetValue(articleId, out var tags))
            {
                tags.Remove(tag);
            }
        }

        public HashSet<string> GetAllUniqueTags()
        {
            var allTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tagSet in _articleTags.Values)
            {
                allTags.UnionWith(tagSet);
            }
            return allTags;
        }

        public HashSet<string> FindCommonTags(string articleId1, string articleId2)
        {
            if (!_articleTags.ContainsKey(articleId1) || !_articleTags.ContainsKey(articleId2))
            {
                return new HashSet<string>();
            }

            var common = new HashSet<string>(_articleTags[articleId1], StringComparer.OrdinalIgnoreCase);
            common.IntersectWith(_articleTags[articleId2]);
            return common;
        }

        public HashSet<string> FindUniqueTagsForFirstArticle(string articleId1, string articleId2)
        {
            if (!_articleTags.ContainsKey(articleId1))
            {
                return new HashSet<string>();
            }

            var diff = new HashSet<string>(_articleTags[articleId1], StringComparer.OrdinalIgnoreCase);
            if (_articleTags.ContainsKey(articleId2))
            {
                diff.ExceptWith(_articleTags[articleId2]);
            }
            return diff;
        }
    }

    public class SearchIndex
    {
        private readonly Dictionary<string, HashSet<int>> _invertedIndex = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

        public void AddDocument(int docId, string content)
        {
            char[] separators = { ' ', ',', '.', '!', '?', ';', ':', '-', '\n', '\r' };
            string[] words = content.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                if (!_invertedIndex.ContainsKey(word))
                {
                    _invertedIndex[word] = new HashSet<int>();
                }
                _invertedIndex[word].Add(docId);
            }
        }

        public HashSet<int> Search(string word)
        {
            if (_invertedIndex.TryGetValue(word, out var docIds))
            {
                return new HashSet<int>(docIds);
            }
            return new HashSet<int>();
        }

        public HashSet<int> SearchMultiple(params string[] words)
        {
            if (words.Length == 0)
            {
                return new HashSet<int>();
            }

            HashSet<int> result = Search(words[0]);
            for (int i = 1; i < words.Length; i++)
            {
                result.IntersectWith(Search(words[i]));
            }
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine(" Сценарій 1: Кеш метаданих файлів (Dictionary) ");
            var fileCache = new FileMetadataCache();
            fileCache.Add(@"C:\docs\report.pdf", new FileMetadata("report.pdf", 245000, DateTime.Now.AddHours(-2), "A1B2C3D4"));
            fileCache.Add(@"C:\code\Program.cs", new FileMetadata("Program.cs", 4120, DateTime.Now, "F9E8D7C6"));

            Console.WriteLine($"Чи є 'C:\\DOCS\\REPORT.PDF' у кеші (case-insensitive): {fileCache.Contains(@"C:\DOCS\REPORT.PDF")}");
            Console.WriteLine($"Отримано: {fileCache.Get(@"C:\docs\report.pdf")}");
            fileCache.Remove(@"C:\code\Program.cs");
            Console.WriteLine($"Після видалення Program.cs наявність: {fileCache.Contains(@"C:\code\Program.cs")}\n");

            Console.WriteLine(" Сценарій 2: Менеджер тегів (Dictionary + HashSet) ");
            var tagManager = new TagManager();
            tagManager.AddTag("art1", "C#");
            tagManager.AddTag("art1", "Collections");
            tagManager.AddTag("art1", "c#");
            tagManager.AddTag("art1", "Performance");

            tagManager.AddTag("art2", "C#");
            tagManager.AddTag("art2", "Collections");
            tagManager.AddTag("art2", "LINQ");

            Console.WriteLine($"Усі унікальні теги (UnionWith): {string.Join(", ", tagManager.GetAllUniqueTags())}");
            Console.WriteLine($"Спільні теги art1 та art2 (IntersectWith): {string.Join(", ", tagManager.FindCommonTags("art1", "art2"))}");
            Console.WriteLine($"Унікальні теги лише для art1 (ExceptWith): {string.Join(", ", tagManager.FindUniqueTagsForFirstArticle("art1", "art2"))}\n");

            Console.WriteLine(" Сценарій 3: Інвертований пошуковий індекс ");
            var searchIndex = new SearchIndex();
            searchIndex.AddDocument(1, "C# Dictionary та HashSet забезпечують швидкий пошук.");
            searchIndex.AddDocument(2, "Узагальнені колекції в C# оптимізують пам'ять.");
            searchIndex.AddDocument(3, "Dictionary використовує хеш-таблицю для пошуку.");

            Console.WriteLine($"Документи зі словом 'Dictionary': {string.Join(", ", searchIndex.Search("Dictionary"))}");
            Console.WriteLine($"Документи зі словами 'C#' та 'пошук': {string.Join(", ", searchIndex.SearchMultiple("C#", "пошук"))}");
        }
    }
}