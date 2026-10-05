using System;

namespace IndependentWork3
{
    public class NetworkStream : IDisposable
    {
        private bool _disposed = false;
        private bool _isStreamOpen;
        private string _address;

        public string Address
        {
            get => _address;
            set => _address = string.IsNullOrWhiteSpace(value) ? "127.0.0.1" : value;
        }

        public bool IsStreamOpen => _isStreamOpen;

        public NetworkStream(string address)
        {
            Address = address;
            _isStreamOpen = true;
            Console.WriteLine($"Мережевий потік {Address} відкрито.");
        }

        public void Send(string data)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(NetworkStream), "Потік закритий!");
            }

            if (_isStreamOpen)
            {
                Console.WriteLine($"Надсилання на {Address}: {data}");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine("Звільнення керованих ресурсів.");
                }

                if (_isStreamOpen)
                {
                    Console.WriteLine($"Закриття мережевого потоку {Address}.");
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
            Console.WriteLine("Виклик деструктора.");
            Dispose(false);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine(" Сценарій 1: using ");
            using (NetworkStream stream1 = new NetworkStream("192.168.1.100"))
            {
                stream1.Send("Пакет 1");
                stream1.Send("Пакет 2");
            }

            Console.WriteLine("\n Сценарій 2: Явний Dispose ");
            NetworkStream stream2 = new NetworkStream("10.0.0.1");
            stream2.Send("Запит до сервера");
            stream2.Dispose();
            stream2.Dispose();

            Console.WriteLine("\n Сценарій 3: Робота GC ");
            CreateUnmanagedStream();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        static void CreateUnmanagedStream()
        {
            NetworkStream stream3 = new NetworkStream("172.16.0.5");
            stream3.Send("Фоновий потік");
        }
    }
}