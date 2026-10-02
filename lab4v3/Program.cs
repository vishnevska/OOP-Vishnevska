using System;
using System.Collections.Generic;

namespace Lab4v3
{
    public class Vehicle
    {
        private string _brand;
        private int _year;

        public string Brand
        {
            get => _brand;
            set => _brand = string.IsNullOrWhiteSpace(value) ? "Unknown Brand" : value;
        }

        public int Year
        {
            get => _year;
            set => _year = value > 1885 ? value : 2000;
        }

        public Vehicle(string brand, int year)
        {
            Brand = brand;
            Year = year;
        }
        public virtual void Drive()
        {
            Console.WriteLine($"[Vehicle] Трандспорту {Brand} ({Year} року) рухається.");
        }

        public string GetVehicleType()
        {
            return "Транспортний засіб (Базовий тип)";
        }
    }

    public class Car : Vehicle
    {
        public int NumDoors { get; set; }

        public Car(string brand, int year, int numDoors) : base(brand, year)
        {
            NumDoors = numDoors;
        }

        public override void Drive()
        {
            Console.WriteLine($"[Car] Автомобіль {Brand} їде по дорозі на 4 колесах ({NumDoors} дверей).");
        }

        public void OpenTrunk()
        {
            Console.WriteLine($"[Car] Багажник автомобіля {Brand} відкрито.");
        }

        public new string GetVehicleType()
        {
            return "Легковий автомобіль (Похідний тип Car з new)";
        }
    }

    public class Motorcycle : Vehicle
    {
        public bool HasSidecar { get; set; }

        public Motorcycle(string brand, int year, bool hasSidecar) : base(brand, year)
        {
            HasSidecar = hasSidecar;
        }

        public override void Drive()
        {
            string sidecarInfo = HasSidecar ? "із коляскою" : "без коляски";
            Console.WriteLine($"[Motorcycle] Мотоцикл {Brand} мчить по трасі {sidecarInfo}.");
        }
        public void Wheelie()
        {
            Console.WriteLine($"[Motorcycle] Мотоцикл {Brand} піднімається на заднє колесо!");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== 1. Демонстрація роботи унікальних методів ===");
            Car myCar = new Car("Toyota", 2021, 4);
            Motorcycle myMoto = new Motorcycle("Harley-Davidson", 2019, false);

            myCar.OpenTrunk();
            myMoto.Wheelie();

            Console.WriteLine("\n=== 2. Демонстрація поліморфізму (override) ===");
            List<Vehicle> garage = new List<Vehicle>
            {
                new Vehicle("Generic Truck", 2015),
                myCar,
                myMoto
            };

            foreach (Vehicle v in garage)
            {
                v.Drive();
            }

            Console.WriteLine("\n=== 3. Демонстрація різниці між override та new ===");
            
            Console.WriteLine($"Викликаємо через Car: {myCar.GetVehicleType()}");

            Vehicle carAsVehicle = myCar;
       
            Console.WriteLine($"Викликаємо через Vehicle: {carAsVehicle.GetVehicleType()}");
            
            Console.WriteLine("\nПояснення: При new викликається метод відповідно до ТИПУ ПОСИЛАННЯ, а не типу об'єкта!");
        }
    }
}