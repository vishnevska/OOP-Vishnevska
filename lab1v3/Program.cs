using System;

class Student
{
    // Приватні поля
    private string name;
    private int id;
    private double averageMark;

    // Публічна властивість
    public double AverageMark
    {
        get { return averageMark; }
        set { averageMark = value; }
    }

    // Конструктор
    public Student(string name, int id, double averageMark)
    {
        this.name = name;
        this.id = id;
        this.averageMark = averageMark;
    }

    // Метод виведення студентського квитка
    public void PrintCard()
    {
        Console.WriteLine("----- Студентський квиток -----");
        Console.WriteLine($"Ім'я студента: {name}");
        Console.WriteLine($"Номер квитка: {id}");
        Console.WriteLine($"Середній бал: {AverageMark}");
        Console.WriteLine("-------------------------------");
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Створення трьох об'єктів
        Student student1 = new Student("Дарина Кондратюк", 101, 10.5);
        Student student2 = new Student("Олена Петренко", 102, 11.2);
        Student student3 = new Student("Андрій Іваненко", 103, 9.8);

        // Виклик методів
        student1.PrintCard();
        student2.PrintCard();
        student3.PrintCard();
    }
}