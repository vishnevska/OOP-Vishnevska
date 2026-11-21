using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace IndependentWork14
{
    public class RetryPolicy
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _initialDelay;
        private readonly double _jitterFactor;
        private readonly Random _random = new Random();

        public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
        {
            if (maxRetries < 0)
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Кількість повторних спроб не може бути від'ємною.");
            if (initialDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(initialDelay), "Початкова затримка не може бути від'ємною.");
            if (jitterFactor < 0 || jitterFactor > 1)
                throw new ArgumentOutOfRangeException(nameof(jitterFactor), "Коефіцієнт jitter має бути в межах [0; 1].");

            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
            _jitterFactor = jitterFactor;
        }

        public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            int retryAttempt = 0;

            while (true)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex)
                {
                    bool canRetry = shouldRetry == null || shouldRetry(ex);

                    if (!canRetry || retryAttempt >= _maxRetries)
                    {
                        Console.WriteLine($"[RetryPolicy] Спроби вичерпано або виняток не підлягає повтору ({ex.GetType().Name}: {ex.Message}).");
                        throw;
                    }

                    double jitterMultiplier = 1.0 + _random.NextDouble() * _jitterFactor * 2.0 - _jitterFactor;
                    double delayMs = _initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt) * jitterMultiplier;
                    TimeSpan delay = TimeSpan.FromMilliseconds(Math.Max(0, delayMs));

                    retryAttempt++;
                    Console.WriteLine($"[Retry #{retryAttempt}/{_maxRetries}] Помилка: {ex.GetType().Name} — \"{ex.Message}\". Очікування {delay.TotalMilliseconds:F0} мс...");
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

            var policy = new RetryPolicy(
                maxRetries: 4,
                initialDelay: TimeSpan.FromMilliseconds(300),
                jitterFactor: 0.25
            );

            Func<Exception, bool> transientFilter = ex =>
                ex is HttpRequestException || ex is TimeoutException;

            Console.WriteLine("=== Сценарій 1: Тимчасовий збій сервера з подальшим успіхом ===\n");
            int attempt1 = 0;
            try
            {
                string weatherData = policy.Execute(() =>
                {
                    attempt1++;
                    Console.WriteLine($"-> Запит прогнозу погоди (виконання #{attempt1})...");
                    if (attempt1 <= 2)
                    {
                        throw new HttpRequestException("503 Service Temporarily Unavailable");
                    }
                    return "{\"city\": \"Rivne\", \"temp\": 14.5, \"status\": \"OK\"}";
                }, transientFilter);

                Console.WriteLine($"[Успіх] Отримано відповідь: {weatherData}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Критична помилка] {ex.Message}\n");
            }

            Console.WriteLine("=== Сценарій 2: Постійна недоступність API (вичерпання усіх спроб) ===\n");
            int attempt2 = 0;
            try
            {
                int balance = policy.Execute<int>(() =>
                {
                    attempt2++;
                    Console.WriteLine($"-> Запит балансу до банківського шлюзу (виконання #{attempt2})...");
                    throw new TimeoutException("Перевищено час очікування відповіді від хоста.");
                }, transientFilter);

                Console.WriteLine($"[Успіх] Баланс: {balance}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Фінальний виняток у Main] Перехоплено {ex.GetType().Name}: {ex.Message}\n");
            }

            Console.WriteLine("=== Сценарій 3: Неповторюваний виняток (фільтр shouldRetry повертає false) ===\n");
            try
            {
                policy.Execute<string>(() =>
                {
                    Console.WriteLine("-> Спроба читання відсутнього файлу конфігурації...");
                    throw new FileNotFoundException("Файл appsettings.prod.json не знайдено.");
                }, transientFilter);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Миттєвий вихід без повторів] Перехоплено {ex.GetType().Name}: {ex.Message}\n");
            }
        }
    }
}