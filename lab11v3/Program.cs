using System;
using System.Collections.Generic;
using System.Linq;

namespace lab11v3
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Grade
    {
        public Subject Subject { get; set; } = new Subject();
        public int Score { get; set; }

        public override string ToString() => $"{Subject.Name}: {Score}";
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Grade> Grades { get; set; } = new List<Grade>();
    }

    public static class GradeExtensions
    {
        public static double AverageScore(this IEnumerable<Grade> grades)
        {
            if (grades == null) throw new ArgumentNullException(nameof(grades));

            var list = grades.ToList();
            if (list.Count == 0) return 0.0;

            return list.Average(g => g.Score);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var s1 = new Subject { Id = 1, Name = "ООП" };
            var s2 = new Subject { Id = 2, Name = "Бази даних" };
            var s3 = new Subject { Id = 3, Name = "Алгоритми" };
            var s4 = new Subject { Id = 4, Name = "Веб-розробка" };

            var students = new List<Student>
            {
                new Student { Id = 1, Name = "Олена Коваленко", Grades = new List<Grade> { new Grade { Subject = s1, Score = 95 }, new Grade { Subject = s2, Score = 88 } } },
                new Student { Id = 2, Name = "Максим Бондаренко", Grades = new List<Grade> { new Grade { Subject = s1, Score = 78 }, new Grade { Subject = s3, Score = 82 } } },
                new Student { Id = 3, Name = "Ірина Ткаченко", Grades = new List<Grade> { new Grade { Subject = s2, Score = 91 }, new Grade { Subject = s4, Score = 94 } } },
                new Student { Id = 4, Name = "Дмитро Шевченко", Grades = new List<Grade> { new Grade { Subject = s1, Score = 65 }, new Grade { Subject = s3, Score = 70 } } },
                new Student { Id = 5, Name = "Софія Мельник", Grades = new List<Grade> { new Grade { Subject = s1, Score = 99 }, new Grade { Subject = s2, Score = 96 }, new Grade { Subject = s4, Score = 92 } } },
                new Student { Id = 6, Name = "Артем Кравчук", Grades = new List<Grade> { new Grade { Subject = s3, Score = 84 }, new Grade { Subject = s4, Score = 79 } } },
                new Student { Id = 7, Name = "Наталія Вишневська", Grades = new List<Grade> { new Grade { Subject = s1, Score = 100 }, new Grade { Subject = s2, Score = 95 }, new Grade { Subject = s3, Score = 97 } } },
                new Student { Id = 8, Name = "Богдан Савченко", Grades = new List<Grade> { new Grade { Subject = s2, Score = 74 }, new Grade { Subject = s4, Score = 81 } } },
                new Student { Id = 9, Name = "Катерина Поліщук", Grades = new List<Grade> { new Grade { Subject = s1, Score = 89 }, new Grade { Subject = s3, Score = 90 } } },
                new Student { Id = 10, Name = "Тарас Мороз", Grades = new List<Grade> { new Grade { Subject = s2, Score = 68 }, new Grade { Subject = s4, Score = 73 } } },
                new Student { Id = 11, Name = "Вікторія Лисенко", Grades = new List<Grade> { new Grade { Subject = s1, Score = 92 }, new Grade { Subject = s4, Score = 87 } } },
                new Student { Id = 12, Name = "Андрій Романенко", Grades = new List<Grade> { new Grade { Subject = s2, Score = 85 }, new Grade { Subject = s3, Score = 88 } } }
            };

            Console.WriteLine("=== 1. SelectMany: Усі оцінки всіх студентів в одному списку ===");
            var allGrades = students.SelectMany(st => st.Grades).ToList();
            Console.WriteLine($"Всього оцінок: {allGrades.Count}");
            Console.WriteLine(string.Join(", ", allGrades));
            Console.WriteLine();

            Console.WriteLine("=== 2. Пагінація: Студенти по 5 на сторінку ===");
            int pageSize = 5;
            int totalPages = (int)Math.Ceiling((double)students.Count / pageSize);

            for (int page = 1; page <= totalPages; page++)
            {
                var pageItems = students
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                Console.WriteLine($"--- Сторінка {page} з {totalPages} ---");
                foreach (var st in pageItems)
                {
                    Console.WriteLine($"  [{st.Id}] {st.Name} (оцінок: {st.Grades.Count}, середній бал: {st.Grades.AverageScore():F2})");
                }
            }
            Console.WriteLine();

            Console.WriteLine("=== 3. Custom Extension: AverageScore() ===");
            double overallAverage = allGrades.AverageScore();
            Console.WriteLine($"Загальний середній бал по всіх оцінках усіх студентів: {overallAverage:F2}");

            var oopGrades = allGrades.Where(g => g.Subject.Name == "ООП");
            Console.WriteLine($"Середній бал з предмету 'ООП': {oopGrades.AverageScore():F2}");
        }
    }
}