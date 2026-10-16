using System;
using System.Collections.Generic;

namespace lab8v3
{
    public class Employee
    {
        public string Name { get; set; }
        public Employee(string name)
        {
            Name = name;
        }
        public virtual decimal CalculatePay()
        {
            return 0m;
        }
    }

    public class HourlyEmployee : Employee
    {
        public double HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public HourlyEmployee(string name, double hoursWorked, decimal hourlyRate)
            : base(name)
        {
            HoursWorked = hoursWorked;
            HourlyRate = hourlyRate;
        }
        public override decimal CalculatePay()
        {
            return (decimal)HoursWorked * HourlyRate;
        }
    }

    public class SalariedEmployee : Employee
    {
        public decimal WeeklySalary { get; set; }
        public SalariedEmployee(string name, decimal weeklySalary)
            : base(name)
        {
            WeeklySalary = weeklySalary;
        }
        public override decimal CalculatePay()
        {
            return WeeklySalary;
        }
    }

    public class CommissionEmployee : Employee
    {
        public decimal SalesAmount { get; set; }
        public decimal CommissionRate { get; set; }
        public CommissionEmployee(string name, decimal salesAmount, decimal commissionRate)
            : base(name)
        {
            SalesAmount = salesAmount;
            CommissionRate = commissionRate;
        }
        public override decimal CalculatePay()
        {
            return SalesAmount * CommissionRate;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Employee> employees = new List<Employee>
            {
                new HourlyEmployee("Олексій", 160, 250m),
                new SalariedEmployee("Марія", 35000m),
                new CommissionEmployee("Дмитро", 150000m, 0.10m)
            };
            Console.WriteLine(" Розрахунок зарплати працівників");
            decimal totalPayout = 0m;

            foreach (var employee in employees)
            {
                decimal pay = employee.CalculatePay();
                totalPayout += pay;
                Console.WriteLine($"Працівник: {employee.Name,-10}  Виплата: {pay,10:C}");
            }

            Console.WriteLine($"Загальна сума виплат: {totalPayout:C}");
        }
    }
}