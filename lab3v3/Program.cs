using System;

namespace Lab3
{
    public class NetworkStream : IDisposable
    {
        private bool _disposed = false;
        private string _address;
        private bool _isStreamOpen;

        public string Address => _address;
        public bool IsStreamOpen => _isStreamOpen;

        public NetworkStream(string address)
        {
            _address = address;
            _isStreamOpen = true;
            Console.WriteLine($"[Конструктор]: Мережевий потік до {_address} ВІДКРИТО.");
        }

        public void Send(string data)
        {
            if (_disposed || !_isStreamOpen)
            {
                throw new ObjectDisposedException(nameof(NetworkStream), "Помилка: Спроба надіслати дані через закритий потік!");
            }

            Console.WriteLine($"[Надсилання]: Передача даних '{data}' на адресу {_address}...");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine($"[Dispose(true)]: Звільнення керованих ресурсів для {_address}.");
                }

                if (_isStreamOpen)
                {
                    Console.WriteLine($"[Dispose]: Закриття мережевого потоку до {_address}...");
                    _isStreamOpen = false;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~NetworkStream()
        {
            Console.WriteLine($"[Фіналізатор ~NetworkStream]: Автоматичний виклик через GC для {_address}.");
            Dispose(false);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №3 (Варіант 3: NetworkStream) ===\n");
            Console.WriteLine("--- Сценарій 1: Використання оператора using ---");
            using (var stream1 = new NetworkStream("192.168.1.10"))
            {
                stream1.Send("Hello Server!");
            }
            Console.WriteLine();

            Console.WriteLine("--- Сценарій 2: Явний виклик Dispose() ---");
            var stream2 = new NetworkStream("10.0.0.1");
            stream2.Send("Ping request");
            stream2.Dispose();
            Console.WriteLine();

            Console.WriteLine("--- Сценарій 3: Робота деструктора (Garbage Collector) ---");
            CreateAndAbandonObject();

            Console.WriteLine("Викликаємо GC.Collect() та GC.WaitForPendingFinalizers()...");
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nПрограму завершено успішно.");
        }

        static void CreateAndAbandonObject()
        {
            var stream3 = new NetworkStream("172.16.0.5");
            stream3.Send("Background Telemetry");
        }
    }
}
