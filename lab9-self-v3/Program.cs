using System;
using System.Collections.Generic;

namespace lab9_self_v3
{
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Rating { get; set; }

        public Product(string name, decimal price, int rating)
        {
            Name = name;
            Price = price;
            Rating = rating;
        }
    }

    public static class CollectionUtils
    {
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection,
            Func<TSource, TResult> mapper)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            var result = new List<TResult>();
            foreach (var item in collection)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        public static T MaxBy<T, TKey>(
            IEnumerable<T> collection,
            Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            T maxItem = default!;
            TKey maxKey = default!;
            bool first = true;

            foreach (var item in collection)
            {
                TKey key = keySelector(item);
                if (first || key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                    first = false;
                }
            }

            if (first)
            {
                throw new InvalidOperationException("Колекція порожня, неможливо знайти максимальний елемент.");
            }

            return maxItem;
        }

        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> collection,
            Predicate<T> predicate)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<T>();
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static void ForEach<T>(
            IEnumerable<T> collection,
            Action<T> action)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in collection)
            {
                action(item);
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine(" 1. Колекція List<int> ");
            var numbers = new List<int> { 4, 15, 8, 23, 42, 11 };

            var evenNumbers = CollectionUtils.Filter(numbers, n => n % 2 == 0);
            Console.WriteLine("Парні числа (Filter): " + string.Join(", ", evenNumbers));

            var squares = CollectionUtils.Map(evenNumbers, n => n * n);
            Console.WriteLine("Їхні квадрати (Map): " + string.Join(", ", squares));

            int maxAbsDiff = CollectionUtils.MaxBy(numbers, n => Math.Abs(n - 20));
            Console.WriteLine($"Число, найдальше від 20 (MaxBy): {maxAbsDiff}");

            Console.Write("Вивід елементів через ForEach: ");
            CollectionUtils.ForEach(numbers, n => Console.Write($"[{n}] "));
            Console.WriteLine("\n");

            Console.WriteLine("2. Колекція List<string> ");
            var cities = new List<string> { "Рівне", "Київ", "Львів", "Івано-Франківськ", "Одеса" };

            var longCities = CollectionUtils.Filter(cities, c => c.Length > 5);
            Console.WriteLine("Міста довші за 5 літер (Filter): " + string.Join(", ", longCities));

            var upperWithLen = CollectionUtils.Map(cities, c => $"{c.ToUpper()} ({c.Length})");
            Console.WriteLine("Верхній регістр + довжина (Map): " + string.Join(", ", upperWithLen));

            string longestCity = CollectionUtils.MaxBy(cities, c => c.Length);
            Console.WriteLine($"Найдовша назва міста (MaxBy): {longestCity}\n");

            Console.WriteLine(" 3. Колекція List<Product> ");
            var products = new List<Product>
            {
                new Product("Ноутбук", 32000m, 5),
                new Product("Мишка", 650m, 4),
                new Product("Клавіатура", 1850m, 5),
                new Product("Монітор", 9400m, 3),
                new Product("Навушники", 2100m, 4)
            };

            var expensiveProducts = CollectionUtils.Filter(products, p => p.Price > 2000m && p.Rating >= 4);
            Console.WriteLine("Товари дорожчі за 2000 грн з рейтингом >= 4 (Filter + ForEach):");
            CollectionUtils.ForEach(expensiveProducts, p => Console.WriteLine($" - {p.Name}: {p.Price} грн (рейтинг: {p.Rating}/5)"));

            var priceTags = CollectionUtils.Map(products, p => $"{p.Name} — {p.Price} грн");
            Console.WriteLine("Прайс-лист (Map): " + string.Join(" | ", priceTags));

            var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"Найдорожчий товар (MaxBy): {mostExpensive.Name} ({mostExpensive.Price} грн)");
        }
    }
}