using System;
using System.Collections.Generic;

namespace lab9v3
{
    public class PrintDocument
    {
        public string Title { get; set; }
        public string User { get; set; }
        public int Pages { get; set; }

        public PrintDocument(string title, string user, int pages)
        {
            Title = title;
            User = user;
            Pages = pages;
        }

        public override string ToString()
        {
            return $"\"{Title}\" ({Pages} стор.) — від {User}";
        }
    }

    public class PrintSystem
    {
        private readonly HashSet<string> _uniqueUsers = new HashSet<string>();
        private readonly Queue<PrintDocument> _printQueue = new Queue<PrintDocument>();
        private readonly Stack<PrintDocument> _printedHistory = new Stack<PrintDocument>();

        public void SubmitDocument(PrintDocument doc)
        {
            bool isNewUser = _uniqueUsers.Add(doc.User);
            _printQueue.Enqueue(doc);

            string userNote = isNewUser ? "(новий користувач)" : "(вже є у списку)";
            Console.WriteLine($"[Черга +] Додано: {doc} {userNote}");
        }

        public void CheckUser(string user)
        {
            bool exists = _uniqueUsers.Contains(user);
            Console.WriteLine($"Перевірка користувача '{user}': {(exists ? "надсилав документи" : "не знайдено")}");
        }

        public void ShowUniqueUsers()
        {
            Console.WriteLine($"Унікальні користувачі ({_uniqueUsers.Count}): {string.Join(", ", _uniqueUsers)}");
        }

        public void PeekNextDocument()
        {
            if (_printQueue.Count > 0)
            {
                Console.WriteLine($"[Queue Peek] Наступний у черзі: {_printQueue.Peek()}");
            }
            else
            {
                Console.WriteLine("[Queue Peek] Черга друку порожня.");
            }
        }

        public void ProcessNextDocument()
        {
            if (_printQueue.Count == 0)
            {
                Console.WriteLine("Черга порожня, нічого друкувати.");
                return;
            }

            PrintDocument current = _printQueue.Dequeue();
            Console.WriteLine($"[Друк FIFO] Надруковано: {current}");
            _printedHistory.Push(current);
        }

        public void PeekLastPrinted()
        {
            if (_printedHistory.Count > 0)
            {
                Console.WriteLine($"[Stack Peek] Останній надрукований документ: {_printedHistory.Peek()}");
            }
            else
            {
                Console.WriteLine("[Stack Peek] Історія друку порожня.");
            }
        }

        public void UndoLastPrintRecord()
        {
            if (_printedHistory.Count > 0)
            {
                PrintDocument removed = _printedHistory.Pop();
                Console.WriteLine($"[Історія LIFO / Pop] Видалено з історії останній запис: {removed}");
            }
            else
            {
                Console.WriteLine("Історія друку порожня.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            PrintSystem printSystem = new PrintSystem();

            Console.WriteLine(" 1. Додавання документів у чергу та облік унікальних користувачів (HashSet + Queue) ");
            printSystem.SubmitDocument(new PrintDocument("Лабораторна_8.pdf", "Олена", 5));
            printSystem.SubmitDocument(new PrintDocument("Курсова_робота.docx", "Максим", 32));
            printSystem.SubmitDocument(new PrintDocument("Договір.pdf", "Олена", 3));
            printSystem.SubmitDocument(new PrintDocument("Презентація.pptx", "Ірина", 14));
            Console.WriteLine();

            Console.WriteLine("2. Робота з HashSet<string> (Унікальні користувачі, O(1)) ");
            printSystem.ShowUniqueUsers();
            printSystem.CheckUser("Олена");
            printSystem.CheckUser("Тарас");
            Console.WriteLine();

            Console.WriteLine(" 3. Робота з Queue<PrintDocument> (FIFO — Першим прийшов, першим пішов) ");
            printSystem.PeekNextDocument();
            printSystem.ProcessNextDocument();
            printSystem.ProcessNextDocument();
            printSystem.PeekNextDocument();
            printSystem.ProcessNextDocument();
            printSystem.ProcessNextDocument();
            Console.WriteLine();

            Console.WriteLine(" 4. Робота зі Stack<PrintDocument> (LIFO — Останнім прийшов, першим пішов) ");
            printSystem.PeekLastPrinted();
            printSystem.UndoLastPrintRecord();
            printSystem.PeekLastPrinted();
            printSystem.UndoLastPrintRecord();
        }
    }
}