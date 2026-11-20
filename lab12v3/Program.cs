using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace lab12v3
{
    public class FileProcessor
    {
        private int _attempts = 0;

        public List<string> GetLines(string path)
        {
            _attempts++;
            if (_attempts <= 2)
            {
                throw new IOException($"Тимчасова помилка доступу до файлу '{path}' (спроба {_attempts}).");
            }

            return new List<string>
            {
                "endpoint_1=/api/v1/users",
                "endpoint_2=/api/v1/orders",
                "timeout=5000"
            };
        }

        public List<string> GetLinesWithFatalError(string path)
        {
            throw new UnauthorizedAccessException($"Доступ до файлу '{path}' заборонено.");
        }
    }

    public class NetworkClient
    {
        private int _attempts = 0;

        public string GetApiResponse(string endpoint)
        {
            _attempts++;
            if (_attempts <= 4)
            {
                throw new HttpRequestException($"Сервер '{endpoint}' тимчасово недоступний 503 (спроба {_attempts}).");
            }

            return "{\"status\": \"ok\", \"items\": [101, 102, 103]}";
        }
    }

    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool> shouldRetry = null)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            if (initialDelay == default) initialDelay = TimeSpan.FromMilliseconds(200);

            int attempt = 0;

            while (true)
            {
                try
                {
                    attempt++;
                    Console.WriteLine($"[Спроба #{attempt}] Виконання операції...");
                    T result = operation();
                    Console.WriteLine($"[Спроба #{attempt}] Успішно виконано!");
                    return result;
                }
                catch (Exception ex)
                {
                    bool canRetry = attempt <= retryCount && (shouldRetry == null || shouldRetry(ex));

                    Console.WriteLine($"[Спроба #{attempt}] Помилка ({ex.GetType().Name}): {ex.Message}");

                    if (!canRetry)
                    {
                        Console.WriteLine($"[Зупинка] Повторні спроби припинено.");
                        throw;
                    }

                    double delayMs = initialDelay.TotalMilliseconds * Math.Pow(2, attempt - 1);
                    TimeSpan delay = TimeSpan.FromMilliseconds(delayMs);
                    Console.WriteLine($"[Очікування] Затримка {delay.TotalMilliseconds} мс перед наступною спробою...");
                    Thread.Sleep(delay);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Func<Exception, bool> retryPolicy = ex =>
                ex is IOException || ex is HttpRequestException;

            var fileProcessor = new FileProcessor();
            var networkClient = new NetworkClient();

            Console.WriteLine("=== 1. FileProcessor.GetLines (2 невдачі -> 3-тя успішна) ===");
            try
            {
                var lines = RetryHelper.ExecuteWithRetry(
                    () => fileProcessor.GetLines("config.txt"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(200),
                    shouldRetry: retryPolicy
                );

                Console.WriteLine("Отримані рядки:");
                foreach (var line in lines)
                {
                    Console.WriteLine($"  - {line}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критична помилка: {ex.Message}");
            }

            Console.WriteLine("\n=== 2. NetworkClient.GetApiResponse (4 невдачі -> 5-та успішна) ===");
            try
            {
                string response = RetryHelper.ExecuteWithRetry(
                    () => networkClient.GetApiResponse("https://api.example.com/data"),
                    retryCount: 4,
                    initialDelay: TimeSpan.FromMilliseconds(150),
                    shouldRetry: retryPolicy
                );

                Console.WriteLine($"Відповідь API: {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критична помилка: {ex.Message}");
            }

            Console.WriteLine("\n=== 3. Перевірка shouldRetry (неповторюваний виняток UnauthorizedAccessException) ===");
            try
            {
                RetryHelper.ExecuteWithRetry(
                    () => fileProcessor.GetLinesWithFatalError("secret.txt"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(200),
                    shouldRetry: retryPolicy
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Перехоплено без повторів: {ex.GetType().Name} — {ex.Message}");
            }
        }
    }
}