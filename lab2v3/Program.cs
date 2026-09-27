using System;
namespace Lab2
{
    public class Student
    {
        private string _name;
        private string _studentId;
        private double _averageMark;
        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value) ? "New Student" : value;
        }
        public string StudentId
        {
            get => _studentId;
            set => _studentId = string.IsNullOrWhiteSpace(value) ? "N/A" : value;
        }
        public double AverageMark
        {
            get => _averageMark;
            set
            {
                if (value < 0.0 || value > 100.0)
                {
                    Console.WriteLine($"[Помилка валідації] Оцінка {value} виходить за межі [0..100]. Встановлено 0.0.");
                    _averageMark = 0.0;
                }
                else
                {
                    _averageMark = value;
                }
            }
        }
        public Student(string name, string studentId, double averageMark)
        {
            Name = name;
            StudentId = studentId;
            AverageMark = averageMark;
            Console.WriteLine($"[Конструктор] Створено студента: {Name}");
        }
        public Student() : this("New Student", "N/A", 0.0)
        {
        }
        public string GetStudentCard()
        {
            return $"Студент: {Name} | ID: {StudentId} | Середній бал: {AverageMark:F1}";
        }
        ~Student()
        {
            Console.WriteLine($"[Деструктор] Об'єкт студента \"{_name}\" (ID: {_studentId}) знищено з пам'яті.");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Початок виконання роботи ===");
            CreateAndUseStudents();
            Console.WriteLine("\n=== Об'єкти вийшли з області видимості ===");
            Console.WriteLine("Запуск збирача сміття (Garbage Collector)...");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Console.WriteLine("=== Програму завершено ===");
        }
        static void CreateAndUseStudents()
        {
            Console.WriteLine("\n--- Створення об'єктів ---");
            Student student1 = new Student();
            Student student2 = new Student("Олександр Бойко", "KB-102938", 92.5);
            Student student3 = new Student("Анна Коваль", "KB-102939", 105.0);
            Console.WriteLine("\n--- Виклики методів ---");
            Console.WriteLine(student1.GetStudentCard());
            Console.WriteLine(student2.GetStudentCard());
            Console.WriteLine(student3.GetStudentCard());
        }
    }
}