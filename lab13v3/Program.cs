using System;
using System.Collections.Generic;
using System.IO;

namespace lab13v3
{
    public class InvalidFileFormatException : Exception
    {
        public string FileName { get; }
        public string ExpectedFormat { get; }

        public InvalidFileFormatException()
        {
            FileName = string.Empty;
            ExpectedFormat = string.Empty;
        }

        public InvalidFileFormatException(string message) : base(message)
        {
            FileName = string.Empty;
            ExpectedFormat = string.Empty;
        }

        public InvalidFileFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
            FileName = string.Empty;
            ExpectedFormat = string.Empty;
        }

        public InvalidFileFormatException(string fileName, string expectedFormat)
            : base($"Файл '{fileName}' має невірний формат. Очікуваний формат: '{expectedFormat}'.")
        {
            FileName = fileName;
            ExpectedFormat = expectedFormat;
        }
    }

    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }

        private Result(T value, bool isSuccess, string error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result<T> Success(T value) => new Result<T>(value, true, string.Empty);

        public static Result<T> Failure(string error) => new Result<T>(default!, false, error);

        public static Result<T> Failure(Exception ex) => new Result<T>(default!, false, ex.Message);
    }

    public class FileProcessorService
    {
        private readonly Dictionary<string, string> _virtualStorage = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["settings.json"] = "{ \"theme\": \"dark\", \"version\": 1 }",
            ["image.png"] = "BINARY_DATA"
        };

        private readonly HashSet<string> _readOnlyFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "system.json"
        };

        public string ReadFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(path));

            string fileName = Path.GetFileName(path);
            string extension = Path.GetExtension(path);

            if (!extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidFileFormatException(fileName, ".json");
            }

            if (!_virtualStorage.TryGetValue(fileName, out string? content))
            {
                throw new FileNotFoundException($"Файл '{fileName}' не знайдено.", fileName);
            }

            return content;
        }

        public Result<bool> WriteToFile(string path, string content)
        {
            if (string.IsNullOrWhiteSpace(path) || path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return Result<bool>.Failure("Невалідний шлях до файлу.");
            }

            string fileName = Path.GetFileName(path);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return Result<bool>.Failure("Не вказано ім'я файлу.");
            }

            if (_readOnlyFiles.Contains(fileName))
            {
                return Result<bool>.Failure($"Відмовлено у доступі: файл '{fileName}' захищений від запису.");
            }

            _virtualStorage[fileName] = content ?? string.Empty;
            return Result<bool>.Success(true);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var fileService = new FileProcessorService();

            Console.WriteLine("=== 1. ReadFile (власний виняток InvalidFileFormatException) ===");
            try
            {
                string data = fileService.ReadFile("settings.json");
                Console.WriteLine($"Успішно прочитано settings.json: {data}");

                Console.WriteLine("Спроба прочитати image.png...");
                fileService.ReadFile("image.png");
            }
            catch (InvalidFileFormatException ex)
            {
                Console.WriteLine($"Перехоплено власний виняток: {ex.Message}");
                Console.WriteLine($"Деталі -> FileName: {ex.FileName}, ExpectedFormat: {ex.ExpectedFormat}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Інша помилка: {ex.Message}");
            }

            Console.WriteLine("\n=== 2. WriteToFile (патерн Result<T>) ===");
            var writeResult1 = fileService.WriteToFile("report.json", "{ \"status\": \"done\" }");
            if (writeResult1.IsSuccess)
            {
                Console.WriteLine($"Запис у report.json успішний (Value = {writeResult1.Value}).");
            }
            else
            {
                Console.WriteLine($"Помилка запису: {writeResult1.Error}");
            }

            var writeResult2 = fileService.WriteToFile("system.json", "{ \"hack\": true }");
            if (writeResult2.IsSuccess)
            {
                Console.WriteLine($"Запис успішний: {writeResult2.Value}");
            }
            else
            {
                Console.WriteLine($"Очікувана відмова при записі у system.json -> Error: {writeResult2.Error}");
            }

            var writeResult3 = fileService.WriteToFile("   ", "test");
            if (!writeResult3.IsSuccess)
            {
                Console.WriteLine($"Очікувана відмова при невалідному шляху -> Error: {writeResult3.Error}");
            }
        }
    }
}