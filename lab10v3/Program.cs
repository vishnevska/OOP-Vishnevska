using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace lab10v3
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int AuthorId { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var authors = new List<Author>
            {
                new Author { Id = 1, Name = "Олена Коваленко", Email = "olena.k@blog.ua" },
                new Author { Id = 2, Name = "Максим Бондаренко", Email = "max.bond@blog.ua" },
                new Author { Id = 3, Name = "Ірина Ткаченко", Email = "iryna.t@blog.ua" },
                new Author { Id = 4, Name = "Дмитро Шевченко", Email = "dmytro.sh@blog.ua" },
                new Author { Id = 5, Name = "Софія Мельник", Email = "sofia.m@blog.ua" },
                new Author { Id = 6, Name = "Артем Кравчук", Email = "artem.k@blog.ua" },
                new Author { Id = 7, Name = "Наталія Вишневська", Email = "nataliia.v@blog.ua" },
                new Author { Id = 8, Name = "Богдан Савченко", Email = "bohdan.s@blog.ua" },
                new Author { Id = 9, Name = "Катерина Поліщук", Email = "kate.p@blog.ua" },
                new Author { Id = 10, Name = "Тарас Мороз", Email = "taras.m@blog.ua" }
            };
            var posts = new List<Post>
            {
                new Post { Id = 1, Title = "Вступ до LINQ у C#", Content = "LINQ дозволяє писати декларативні запити до колекцій у C#.", AuthorId = 1 },
                new Post { Id = 2, Title = "Основи ООП", Content = "Інкапсуляція, наслідування та поліморфізм є базою ООП.", AuthorId = 2 },
                new Post { Id = 3, Title = "Продуктивність LINQ запитів", Content = "Аналізуємо швидкість роботи Where, Select та Join на великих обсягах даних.", AuthorId = 1 },
                new Post { Id = 4, Title = "Колекції Dictionary та HashSet", Content = "Як хеш-таблиці забезпечують пошук за O(1).", AuthorId = 3 },
                new Post { Id = 5, Title = "Делегати та лямбда-вирази", Content = "Лямбда-вирази значно спрощують передачу логіки у методи.", AuthorId = 4 },
                new Post { Id = 6, Title = "Асинхронне програмування", Content = "Використання async та await для неблокуючих операцій вводу-виводу.", AuthorId = 5 },
                new Post { Id = 7, Title = "Групування даних через LINQ", Content = "Оператор GroupBy та GroupJoin допомагають агрегувати складні структури.", AuthorId = 7 },
                new Post { Id = 8, Title = "Патерни проєктування", Content = "Огляд популярних патернів: Singleton, Factory, Strategy та Observer.", AuthorId = 6 },
                new Post { Id = 9, Title = "Робота з файлами в .NET", Content = "Читання та запис текстових і бінарних файлів через StreamReader.", AuthorId = 8 },
                new Post { Id = 10, Title = "Оптимізація пам'яті в C#", Content = "Різниця між типами значень (struct) та посилальними типами (class).", AuthorId = 7 },
                new Post { Id = 11, Title = "Відкладене виконання в LINQ", Content = "Чому LINQ запити виконуються лише під час ітерації або виклику ToList().", AuthorId = 7 },
                new Post { Id = 12, Title = "Тестування коду з xUnit", Content = "Написання модульних тестів для перевірки бізнес-логіки застосунків.", AuthorId = 9 },
                new Post { Id = 13, Title = "Робота з Git та GitHub", Content = "Основні команди для контролю версій: commit, push, pull та merge.", AuthorId = 10 },
                new Post { Id = 14, Title = "Регулярні вирази в C#", Content = "Клас Regex для валідації та парсингу текстових рядків.", AuthorId = 3 },
                new Post { Id = 15, Title = "Порівняння LINQ синтаксисів", Content = "Детальний розбір Query Syntax та Method Syntax для фільтрації й об'єднання.", AuthorId = 1 }
            };

            _ = posts.Where(p => p.Id == 1).ToList();

            var sw = new Stopwatch();

            Console.WriteLine(" ЗАПИТ 1: Пости, що містять слово 'LINQ' ");

            sw.Restart();
            var q1Method = posts
                .Where(p => p.Title.Contains("LINQ", StringComparison.OrdinalIgnoreCase) ||
                            p.Content.Contains("LINQ", StringComparison.OrdinalIgnoreCase))
                .ToList();
            sw.Stop();
            long t1Method = sw.ElapsedTicks;

            sw.Restart();
            var q1Query = (from p in posts
                           where p.Title.Contains("LINQ", StringComparison.OrdinalIgnoreCase) ||
                                 p.Content.Contains("LINQ", StringComparison.OrdinalIgnoreCase)
                           select p).ToList();
            sw.Stop();
            long t1Query = sw.ElapsedTicks;

            foreach (var p in q1Method)
                Console.WriteLine($"  [{p.Id}] {p.Title}");
            Console.WriteLine($"Час виконання -> Method Syntax: {t1Method} ticks | Query Syntax: {t1Query} ticks\n");

            Console.WriteLine(" ЗАПИТ 2: Об'єднання постів з авторами (Join) ");

            sw.Restart();
            var q2Method = posts
                .Join(authors,
                      post => post.AuthorId,
                      author => author.Id,
                      (post, author) => new { post.Title, AuthorName = author.Name })
                .ToList();
            sw.Stop();
            long t2Method = sw.ElapsedTicks;

            sw.Restart();
            var q2Query = (from post in posts
                           join author in authors on post.AuthorId equals author.Id
                           select new { post.Title, AuthorName = author.Name }).ToList();
            sw.Stop();
            long t2Query = sw.ElapsedTicks;

            foreach (var item in q2Method.Take(6)) // Виводимо перші 6 для компактності
                Console.WriteLine($"  \"{item.Title}\" — Автор: {item.AuthorName}");
            Console.WriteLine($"  ... (всього об'єднано {q2Method.Count} записів)");
            Console.WriteLine($"Час виконання -> Method Syntax: {t2Method} ticks | Query Syntax: {t2Query} ticks\n");

            Console.WriteLine("ЗАПИТ 3: Кількість постів для кожного автора (GroupJoin / GroupBy) ");

            sw.Restart();
            var q3Method = authors
                .GroupJoin(posts,
                           author => author.Id,
                           post => post.AuthorId,
                           (author, authorPosts) => new
                           {
                               AuthorName = author.Name,
                               PostCount = authorPosts.Count()
                           })
                .OrderByDescending(x => x.PostCount)
                .ToList();
            sw.Stop();
            long t3Method = sw.ElapsedTicks;

            sw.Restart();
            var q3Query = (from author in authors
                           join post in posts on author.Id equals post.AuthorId into authorPosts
                           orderby authorPosts.Count() descending
                           select new
                           {
                               AuthorName = author.Name,
                               PostCount = authorPosts.Count()
                           }).ToList();
            sw.Stop();
            long t3Query = sw.ElapsedTicks;

            foreach (var item in q3Method)
                Console.WriteLine($"  {item.AuthorName}: {item.PostCount} пост(ів)");
            Console.WriteLine($"Час виконання -> Method Syntax: {t3Method} ticks | Query Syntax: {t3Query} ticks\n");
            Console.WriteLine(" ЗАПИТ 4: Пост з найдовшим вмістом ");

            sw.Restart();
            var q4Method = posts
                .OrderByDescending(p => p.Content.Length)
                .FirstOrDefault();
            sw.Stop();
            long t4Method = sw.ElapsedTicks;

            sw.Restart();
            var q4Query = (from p in posts
                           orderby p.Content.Length descending
                           select p).FirstOrDefault();
            sw.Stop();
            long t4Query = sw.ElapsedTicks;

            if (q4Method != null)
            {
                Console.WriteLine($"  [{q4Method.Id}] {q4Method.Title} (довжина вмісту: {q4Method.Content.Length} символів)");
                Console.WriteLine($"  Текст: {q4Method.Content}");
            }
            Console.WriteLine($"Час виконання -> Method Syntax: {t4Method} ticks | Query Syntax: {t4Query} ticks");
        }
    }
}