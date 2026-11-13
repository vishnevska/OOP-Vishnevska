using System;
using System.Collections.Generic;

namespace lab8v3
{
    public static class CollectionHelper<T>
    {
        public static IEnumerable<T> Filter(IEnumerable<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            List<T> result = new List<T>();
            foreach (T item in collection)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public override string ToString()
        {
            return $"{Name} — {Price:C}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            IEnumerable<int> evenNumbers = CollectionHelper<int>.Filter(numbers, n => n % 2 == 0);

            Console.WriteLine("Парні числа ");
            foreach (int num in evenNumbers)
            {
                Console.Write(num + " ");
            }
            Console.WriteLine("\n");

            List<string> words = new List<string> { "Кіт", "Програмування", "C#", "Алгоритм", "Код" };
            IEnumerable<string> longWords = CollectionHelper<string>.Filter(words, w => w.Length > 4);

            Console.WriteLine("Слова довші за 4 символи ");
            foreach (string word in longWords)
            {
                Console.WriteLine(word);
            }
            Console.WriteLine();

            List<Product> products = new List<Product>
            {
                new Product("Зошит", 35m),
                new Product("Навушники", 1250m),
                new Product("Ручка", 15m),
                new Product("Клавіатура", 890m)
            };

            IEnumerable<Product> expensiveProducts = CollectionHelper<Product>.Filter(products, p => p.Price > 100m);

            Console.WriteLine(" Товари з ціною > 100 ");
            foreach (Product product in expensiveProducts)
            {
                Console.WriteLine(product);
            }
        }
    }
}