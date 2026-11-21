using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork12
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Частина 1: Порівняння продуктивності LINQ та PLINQ ===\n");

            int[] sizes = { 1_000_000, 5_000_000, 10_000_000 };

            foreach (int size in sizes)
            {
                RunPerformanceBenchmark(size);
            }

            Console.WriteLine("=== Частина 2: Дослідження потокобезпечності та побічних ефектів ===\n");
            RunThreadSafetyDemo();
        }

        static List<int> GenerateNumbers(int count, int seed = 42)
        {
            var random = new Random(seed);
            var list = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(random.Next(10_000, 500_000));
            }
            return list;
        }

        static bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            int limit = (int)Math.Sqrt(number);
            for (int i = 3; i <= limit; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        static double HeavyComputation(int value)
        {
            double result = 0;
            for (int i = 1; i <= 15; i++)
            {
                result += Math.Sqrt(value * i) * Math.Sin(value + i);
            }
            return result;
        }

        static void RunPerformanceBenchmark(int size)
        {
            var data = GenerateNumbers(size);

            var swLinq = Stopwatch.StartNew();
            var linqResult = data
                .Where(IsPrime)
                .Select(HeavyComputation)
                .ToList();
            swLinq.Stop();

            var swPlinq = Stopwatch.StartNew();
            var plinqResult = data
                .AsParallel()
                .Where(IsPrime)
                .Select(HeavyComputation)
                .ToList();
            swPlinq.Stop();

            double speedup = swLinq.Elapsed.TotalMilliseconds / swPlinq.Elapsed.TotalMilliseconds;

            Console.WriteLine($"Розмір колекції: {size:N0} елементів (знайдено простих: {linqResult.Count:N0})");
            Console.WriteLine($"  - LINQ:  {swLinq.Elapsed.TotalMilliseconds,8:F2} мс");
            Console.WriteLine($"  - PLINQ: {swPlinq.Elapsed.TotalMilliseconds,8:F2} мс (прискорення: {speedup:F2}x)\n");
        }

        static void RunThreadSafetyDemo()
        {
            var numbers = Enumerable.Range(1, 200_000).ToList();
            int expectedSum = numbers.Count;

            Console.WriteLine($"Еталонна кількість елементів: {expectedSum:N0}\n");
            Console.WriteLine("1. Небезпечна модифікація спільної змінної без синхронізації (Race Condition):");

            for (int attempt = 1; attempt <= 3; attempt++)
            {
                int unsafeCounter = 0;

                var _ = numbers
                    .AsParallel()
                    .Select(x =>
                    {
                        unsafeCounter++;
                        return x * 2;
                    })
                    .ToList();

                Console.WriteLine($"  Спроба #{attempt}: отримано {unsafeCounter:N0} (втрачено {expectedSum - unsafeCounter:N0})");
            }

            Console.WriteLine("\n2. Виправлення через Interlocked.Increment:");
            int safeInterlockedCounter = 0;
            var interlockedList = numbers
                .AsParallel()
                .Select(x =>
                {
                    Interlocked.Increment(ref safeInterlockedCounter);
                    return x * 2;
                })
                .ToList();
            Console.WriteLine($"  Результат з Interlocked: {safeInterlockedCounter:N0} (коректно)");

            Console.WriteLine("\n3. Виправлення через lock та потокобезпечну колекцію ConcurrentBag<int>:");
            int safeLockCounter = 0;
            object locker = new object();
            var safeBag = new ConcurrentBag<int>();

            var lockList = numbers
                .AsParallel()
                .Select(x =>
                {
                    lock (locker)
                    {
                        safeLockCounter++;
                    }
                    safeBag.Add(x * 2);
                    return x * 2;
                })
                .ToList();

            Console.WriteLine($"  Результат з lock:          {safeLockCounter:N0} (коректно)");
            Console.WriteLine($"  Елементів у ConcurrentBag: {safeBag.Count:N0} (коректно)\n");
        }
    }
}