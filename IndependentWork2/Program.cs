using System;

namespace IndependentWork2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("  ПОРІВНЯННЯ ПРОЦЕДУРНОГО ТА ОБ'ЄКТНОГО ПІДХОДІВ  ");

            ProceduralDemo.Run();

            ObjectOrientedDemo.Run();

            Console.WriteLine("Результати обох підходів збігаються!");
        }
    }
}