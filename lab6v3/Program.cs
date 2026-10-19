
using System;
using System.Collections.Generic;
using System.Linq;

namespace lab6v3
{
    public interface IEncryptor
    {
        string Encrypt(string data);
        string Decrypt(string encryptedData);
    }
    public class CaesarCipher : IEncryptor
    {
        private const int Shift = 3;

        public string Encrypt(string data)
        {
            ArgumentNullException.ThrowIfNull(data);

            char[] buffer = data.ToCharArray();

            for (int i = 0; i < buffer.Length; i++)
            {
                char c = buffer[i];

                if (c >= 'A' && c <= 'Z')
                {
                    buffer[i] = (char)('A' + (c - 'A' + Shift) % 26);
                }
                else if (c >= 'a' && c <= 'z')
                {
                    buffer[i] = (char)('a' + (c - 'a' + Shift) % 26);
                }
            }

            return new string(buffer);
        }

        public string Decrypt(string encryptedData)
        {
            ArgumentNullException.ThrowIfNull(encryptedData);

            char[] buffer = encryptedData.ToCharArray();

            for (int i = 0; i < buffer.Length; i++)
            {
                char c = buffer[i];

                if (c >= 'A' && c <= 'Z')
                {
                    buffer[i] = (char)('A' + (c - 'A' - Shift + 26) % 26);
                }
                else if (c >= 'a' && c <= 'z')
                {
                    buffer[i] = (char)('a' + (c - 'a' - Shift + 26) % 26);
                }
            }

            return new string(buffer);
        }
    }
    public class ReverseEncryptor : IEncryptor
    {
        public string Encrypt(string data)
        {
            ArgumentNullException.ThrowIfNull(data);
            return new string(data.Reverse().ToArray());
        }

        public string Decrypt(string encryptedData)
        {
            ArgumentNullException.ThrowIfNull(encryptedData);
            return new string(encryptedData.Reverse().ToArray());
        }
    }
    public abstract class ReportGenerator
    {
        public string Title { get; set; }

        protected string ReportData = "";
        protected string FormattedContent = "";

        protected ReportGenerator(string title)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            Title = title;
        }
        public abstract void CollectData();
        public abstract void FormatReport();
        public void GenerateOutput()
        {
            CollectData();
            FormatReport();

            Console.WriteLine($" ЗВІТ: {Title}");
            Console.WriteLine(FormattedContent);
        }
    }

    public class PdfReportGenerator : ReportGenerator
    {
        public PdfReportGenerator(string title) : base(title)
        {
        }

        public override void CollectData()
        {
            ReportData = "Фінансові показники за поточний місяць.";
        }

        public override void FormatReport()
        {
            FormattedContent =
                $"[PDF]\nЗміст: {ReportData}\nФормат документа: PDF/A";
        }
    }
    public class HtmlReportGenerator : ReportGenerator
    {
        public HtmlReportGenerator(string title) : base(title)
        {
        }

        public override void CollectData()
        {
            ReportData = "Статистика відвідуваності вебсайту.";
        }

        public override void FormatReport()
        {
            FormattedContent =
                $"<html>\n" +
                $"  <body>\n" +
                $"    <h1>{Title}</h1>\n" +
                $"    <p>{ReportData}</p>\n" +
                $"  </body>\n" +
                $"</html>";
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("ІНТЕРФЕЙС IEncryptor");

            List<IEncryptor> encryptors = new List<IEncryptor>
            {
                new CaesarCipher(),
                new ReverseEncryptor()
            };

            string originalText = "Hello World C#";

            Console.WriteLine($"Оригінальний текст: {originalText}\n");

            foreach (IEncryptor encryptor in encryptors)
            {
                string encrypted = encryptor.Encrypt(originalText);
                string decrypted = encryptor.Decrypt(encrypted);

                Console.WriteLine($"Алгоритм: {encryptor.GetType().Name}");
                Console.WriteLine($"Зашифровано: {encrypted}");
                Console.WriteLine($"Розшифровано: {decrypted}");
                Console.WriteLine();
            }
            Console.WriteLine("\n АБСТРАКТНИЙ КЛАС ReportGenerator");

            List<ReportGenerator> reportGenerators =
                new List<ReportGenerator>
                {
                    new PdfReportGenerator("Фінансовий звіт"),
                    new HtmlReportGenerator("Аналітика сайту")
                };

            foreach (ReportGenerator generator in reportGenerators)
            {
                generator.GenerateOutput();
            }

            Console.WriteLine("\nПрограму завершено.");
        }
    }
}
