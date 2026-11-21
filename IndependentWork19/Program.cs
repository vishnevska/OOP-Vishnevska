using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork19
{
    public class PaymentLimitExceededException : Exception
    {
        public string AccountId { get; } = string.Empty;
        public decimal AttemptedAmount { get; }
        public decimal DailyLimit { get; }

        public PaymentLimitExceededException() { }

        public PaymentLimitExceededException(string message) : base(message) { }

        public PaymentLimitExceededException(string message, Exception innerException)
            : base(message, innerException) { }

        public PaymentLimitExceededException(
            string message,
            string accountId,
            decimal attemptedAmount,
            decimal dailyLimit)
            : base(message)
        {
            AccountId = accountId;
            AttemptedAmount = attemptedAmount;
            DailyLimit = dailyLimit;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            DemonstrateCustomException();
            await Scenario1_PaymentApiRetryWithTimeoutAsync();
            await Scenario2_NotificationQueueCircuitBreakerAsync();
            await Scenario3_ExchangeRateFallbackAsync();
        }

        static void DemonstrateCustomException()
        {
            Console.WriteLine("=== 1. Демонстрація власного винятку PaymentLimitExceededException ===\n");

            try
            {
                ProcessPayment("ACC-7741", attemptedAmount: 25000m, dailyLimit: 15000m);
            }
            catch (PaymentLimitExceededException ex)
            {
                Console.WriteLine($"[Помилка ліміту] {ex.Message}");
                Console.WriteLine($"  Рахунок: {ex.AccountId} | Спроба: {ex.AttemptedAmount} грн | Денний ліміт: {ex.DailyLimit} грн");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Загальна помилка] {ex.Message}");
            }
            finally
            {
                Console.WriteLine("[Finally] Аудит платіжної транзакції зафіксовано.\n");
            }
        }

        static void ProcessPayment(string accountId, decimal attemptedAmount, decimal dailyLimit)
        {
            if (attemptedAmount > dailyLimit)
            {
                throw new PaymentLimitExceededException(
                    $"Сума транзакції перевищує дозволений денний ліміт для рахунку {accountId}.",
                    accountId,
                    attemptedAmount,
                    dailyLimit
                );
            }
            Console.WriteLine($"Платіж на {attemptedAmount} грн успішно проведено.");
        }

        static async Task Scenario1_PaymentApiRetryWithTimeoutAsync()
        {
            Console.WriteLine("=== 2. Сценарій 1 (Polly): Retry + Timeout при виклику платіжного шлюзу ===\n");

            int callAttempt = 0;

            var pipeline = new ResiliencePipelineBuilder<string>()
                .AddRetry(new RetryStrategyOptions<string>
                {
                    ShouldHandle = new PredicateBuilder<string>()
                        .Handle<HttpRequestException>()
                        .Handle<TimeoutRejectedException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(400),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        Console.WriteLine($"  [Retry #{args.AttemptNumber + 1}] Причина: {args.Outcome.Exception?.GetType().Name} ({args.Outcome.Exception?.Message}). Пауза: {args.RetryDelay.TotalMilliseconds:F0} мс");
                        return default;
                    }
                })
                .AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromMilliseconds(500),
                    OnTimeout = args =>
                    {
                        Console.WriteLine($"  [Timeout] Перевищено ліміт очікування ({args.Timeout.TotalMilliseconds} мс)!");
                        return default;
                    }
                })
                .Build();

            try
            {
                string response = await pipeline.ExecuteAsync(async ct =>
                {
                    callAttempt++;
                    Console.WriteLine($"-> Виклик платіжного API (спроба #{callAttempt})...");

                    if (callAttempt == 1)
                    {
                        await Task.Delay(800, ct);
                    }
                    if (callAttempt == 2)
                    {
                        throw new HttpRequestException("503 Service Unavailable: шлюз тимчасово перевантажений.");
                    }

                    return "TXN-2026-OK (Платіж підтверджено)";
                }, CancellationToken.None);

                Console.WriteLine($"[Успіх] Відповідь шлюзу: {response}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Остаточна невдача] {ex.Message}\n");
            }
        }

        static async Task Scenario2_NotificationQueueCircuitBreakerAsync()
        {
            Console.WriteLine("=== 3. Сценарій 2 (Polly): Circuit Breaker для черги SMS-сповіщень ===\n");

            var breakerPipeline = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<IOException>(),
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = 3,
                    BreakDuration = TimeSpan.FromSeconds(2),
                    OnOpened = args =>
                    {
                        Console.WriteLine($"  [Circuit OPEN] Вимикач розімкнено на {args.BreakDuration.TotalSeconds} с через серію збоїв!");
                        return default;
                    },
                    OnClosed = args =>
                    {
                        Console.WriteLine("  [Circuit CLOSED] Роботу черги відновлено, вимикач замкнено.");
                        return default;
                    },
                    OnHalfOpened = args =>
                    {
                        Console.WriteLine("  [Circuit HALF-OPEN] Тестова перевірка доступності черги...");
                        return default;
                    }
                })
                .Build();

            bool brokerDown = true;

            for (int i = 1; i <= 7; i++)
            {
                if (i == 6)
                {
                    Console.WriteLine("  ... Очікування 2.2 с до переходу вимикача у стан Half-Open ...");
                    await Task.Delay(2200);
                    brokerDown = false;
                }

                try
                {
                    await breakerPipeline.ExecuteAsync(async ct =>
                    {
                        await Task.Delay(50, ct);
                        if (brokerDown)
                        {
                            throw new IOException("Connection refused: брокер черги не відповідає.");
                        }
                        Console.WriteLine($"-> Повідомлення #{i}: успішно відправлено у чергу.");
                    }, CancellationToken.None);
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine($"-> Повідомлення #{i}: миттєво відхилено (ланцюг розімкнено, запит не надсилався).");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"-> Повідомлення #{i}: збій брокера ({ex.Message})");
                }
            }
            Console.WriteLine();
        }

        static async Task Scenario3_ExchangeRateFallbackAsync()
        {
            Console.WriteLine("=== 4. Сценарій 3 (Polly): Retry + Fallback для сервісу курсів валют ===\n");

            decimal cachedRate = 41.25m;

            var fallbackPipeline = new ResiliencePipelineBuilder<decimal>()
                .AddFallback(new FallbackStrategyOptions<decimal>
                {
                    ShouldHandle = new PredicateBuilder<decimal>().Handle<HttpRequestException>(),
                    FallbackAction = args =>
                    {
                        Console.WriteLine($"  [Fallback] Основний сервіс недоступний ({args.Outcome.Exception?.Message}). Повертаємо кешований курс.");
                        return Outcome.FromResultAsValueTask(cachedRate);
                    }
                })
                .AddRetry(new RetryStrategyOptions<decimal>
                {
                    ShouldHandle = new PredicateBuilder<decimal>().Handle<HttpRequestException>(),
                    MaxRetryAttempts = 2,
                    Delay = TimeSpan.FromMilliseconds(200),
                    OnRetry = args =>
                    {
                        Console.WriteLine($"  [Retry #{args.AttemptNumber + 1}] Повторний запит курсу USD/UAH...");
                        return default;
                    }
                })
                .Build();

            decimal activeRate = await fallbackPipeline.ExecuteAsync(
                async ct => await FetchExchangeRateAsync(ct),
                CancellationToken.None
            );

            Console.WriteLine($"[Результат] Актуальний курс для розрахунку кошика: {activeRate} грн\n");
        }

        static async ValueTask<decimal> FetchExchangeRateAsync(CancellationToken ct)
        {
            await Task.Delay(50, ct);
            throw new HttpRequestException("404 / Gateway Timeout: API НБУ недоступне.");
        }
    }
}