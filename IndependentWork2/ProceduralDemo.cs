using System;
namespace IndependentWork2
{
    public static class ProceduralDemo
    {
        public static void Run()
        {
            Console.WriteLine(" 1. ПРОЦЕДУРНИЙ ПІДХІД ");
            string[] names = { "Ноутбук", "Мишка", "Клавіатура", "Монітор" };
            double[] prices = { 25000.0, 450.0, 800.0, 6500.0 };
            int[] quantities = { 1, 2, 1, 1 };
            double[] itemTotals = CalculateItemTotals(prices, quantities);
            double[] discounts = ApplyDiscounts(prices, itemTotals);

            double grandTotal = 0;
            for (int i = 0; i < names.Length; i++)
            {
                double finalItemPrice = itemTotals[i] - discounts[i];
                grandTotal += finalItemPrice;

                string discountInfo = discounts[i] > 0 ? $" (Знижка 10%: -{discounts[i]:F2} грн)" : "";
                Console.WriteLine($"{i + 1}. {names[i]} x{quantities[i]} — Ціна: {prices[i]} грн | Сума: {itemTotals[i]} грн{discountInfo}");
            }

            Console.WriteLine($"\nЗагальна вартість кошика (процедурно): {grandTotal:F2} грн\n");
        }

        private static double[] CalculateItemTotals(double[] prices, int[] quantities)
        {
            double[] totals = new double[prices.Length];
            for (int i = 0; i < prices.Length; i++)
            {
                totals[i] = prices[i] * quantities[i];
            }
            return totals;
        }

        private static double[] ApplyDiscounts(double[] prices, double[] itemTotals)
        {
            double[] discounts = new double[prices.Length];
            for (int i = 0; i < prices.Length; i++)
            {
                if (prices[i] > 500.0)
                {
                    discounts[i] = itemTotals[i] * 0.10; 
                }
            }
            return discounts;
        }
    }
}