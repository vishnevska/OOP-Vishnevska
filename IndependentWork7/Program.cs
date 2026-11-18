using System;

namespace IndependentWork7
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Cache<string, string> cache = new Cache<string, string>(3);

            Console.WriteLine(" Додавання 3 елементів (ємність = 3)");
            cache.Add("user_1", "Олена");
            cache.Add("user_2", "Максим");
            cache.Add("user_3", "Ірина");
            Console.WriteLine($"Кількість елементів у кеші: {cache.Count}");
            Console.WriteLine($"Отримано user_1: {cache.Get("user_1")}\n");

            Console.WriteLine(" Переповнення кешу (додаємо user_4)");
            cache.Add("user_4", "Тарас");
            Console.WriteLine($"Кількість елементів у кеші: {cache.Count}");
            Console.WriteLine($"Чи залишився найстаріший ключ user_1: {cache.ContainsKey("user_1")}");
            Console.WriteLine($"Чи є новий ключ user_4: {cache.ContainsKey("user_4")}");
        }
    }
}