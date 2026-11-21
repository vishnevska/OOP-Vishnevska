using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IndependentWork11
{
    public record Product(int Id, string Name, string Category, decimal UnitPrice);
    public record Customer(int Id, string Name, string City);
    public record Sale(int Id, int ProductId, int Quantity, DateTime SaleDate, int CustomerId);

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var (products, customers, sales) = GenerateTestData(
                productCount: 60,
                customerCount: 80,
                saleCount: 500,
                seed: 42
            );

            Console.WriteLine($"Згенеровано: {products.Count} продуктів, {customers.Count} клієнтів, {sales.Count} продажів.\n");

            TopProductsByCategory(sales, products);
            TotalRevenueByCity(sales, products, customers);
            SalesByYearAndMonth(sales, products);
            TopVipCustomersInLargeOrders(sales, products, customers);
        }

        static (List<Product> Products, List<Customer> Customers, List<Sale> Sales) GenerateTestData(
            int productCount, int customerCount, int saleCount, int seed)
        {
            var random = new Random(seed);

            string[] categories = { "Електроніка", "Побутова техніка", "Аксесуари", "Офіс", "Спорт" };
            string[] cities = { "Рівне", "Київ", "Львів", "Дніпро", "Одеса", "Харків" };

            var products = Enumerable.Range(1, productCount)
                .Select(i => new Product(
                    Id: i,
                    Name: $"Товар #{i:D2}",
                    Category: categories[random.Next(categories.Length)],
                    UnitPrice: Math.Round((decimal)(random.NextDouble() * 4900 + 100), 2)
                ))
                .ToList();

            var customers = Enumerable.Range(1, customerCount)
                .Select(i => new Customer(
                    Id: i,
                    Name: $"Клієнт #{i:D2}",
                    City: cities[random.Next(cities.Length)]
                ))
                .ToList();

            var startDate = new DateTime(2024, 1, 1);
            int totalDays = (new DateTime(2025, 12, 31) - startDate).Days;

            var sales = Enumerable.Range(1, saleCount)
                .Select(i => new Sale(
                    Id: i,
                    ProductId: random.Next(1, productCount + 1),
                    Quantity: random.Next(1, 15),
                    SaleDate: startDate.AddDays(random.Next(totalDays + 1)),
                    CustomerId: random.Next(1, customerCount + 1)
                ))
                .ToList();

            return (products, customers, sales);
        }
        static void TopProductsByCategory(List<Sale> sales, List<Product> products)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(s => s.Quantity)
                })
                .Join(
                    products,
                    stat => stat.ProductId,
                    prod => prod.Id,
                    (stat, prod) => new
                    {
                        prod.Name,
                        prod.Category,
                        stat.TotalQuantity
                    })
                .GroupBy(x => x.Category)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Category = g.Key,
                    Top3 = g.OrderByDescending(x => x.TotalQuantity)
                            .Take(3)
                            .ToList()
                })
                .ToList();

            sw.Stop();

            Console.WriteLine($"=== Пайплайн 1: Найпопулярніші продукти за категоріями (Час: {sw.Elapsed.TotalMilliseconds:F3} мс) ===");
            foreach (var group in result)
            {
                Console.WriteLine($"Категорія: {group.Category}");
                foreach (var item in group.Top3)
                {
                    Console.WriteLine($"  - {item.Name}: {item.TotalQuantity} шт.");
                }
            }
            Console.WriteLine();
        }
        static void TotalRevenueByCity(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .Join(
                    products,
                    s => s.ProductId,
                    p => p.Id,
                    (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
                .Join(
                    customers,
                    sp => sp.CustomerId,
                    c => c.Id,
                    (sp, c) => new { c.City, sp.Revenue })
                .GroupBy(x => x.City)
                .Select(g => new
                {
                    City = g.Key,
                    TotalRevenue = g.Sum(x => x.Revenue),
                    OrdersCount = g.Count()
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            sw.Stop();

            Console.WriteLine($"=== Пайплайн 2: Загальний дохід за містами клієнтів (Час: {sw.Elapsed.TotalMilliseconds:F3} мс) ===");
            foreach (var row in result)
            {
                Console.WriteLine($"  - {row.City,-10} | Замовлень: {row.OrdersCount,3} | Дохід: {row.TotalRevenue,12:N2} грн");
            }
            Console.WriteLine();
        }

        static void SalesByYearAndMonth(List<Sale> sales, List<Product> products)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .Join(
                    products,
                    s => s.ProductId,
                    p => p.Id,
                    (s, p) => new
                    {
                        s.SaleDate.Year,
                        s.SaleDate.Month,
                        s.Quantity,
                        Revenue = s.Quantity * p.UnitPrice
                    })
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalUnits = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            sw.Stop();

            Console.WriteLine($"=== Пайплайн 3: Продажі за місяцями та роками (Час: {sw.Elapsed.TotalMilliseconds:F3} мс) ===");
            foreach (var row in result.Take(12))
            {
                Console.WriteLine($"  - {row.Year}-{row.Month:D2} | Продано одиниць: {row.TotalUnits,4} | Дохід: {row.TotalRevenue,12:N2} грн");
            }
            Console.WriteLine($"  ... (усього {result.Count} місячних періодів)\n");
        }
        static void TopVipCustomersInLargeOrders(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .Where(s => s.Quantity >= 5)
                .Join(
                    products,
                    s => s.ProductId,
                    p => p.Id,
                    (s, p) => new { s.CustomerId, Amount = s.Quantity * p.UnitPrice })
                .GroupBy(x => x.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    TotalSpent = g.Aggregate(0m, (acc, item) => acc + item.Amount),
                    BulkOrders = g.Count()
                })
                .Join(
                    customers,
                    stat => stat.CustomerId,
                    c => c.Id,
                    (stat, c) => new { c.Name, c.City, stat.BulkOrders, stat.TotalSpent })
                .OrderByDescending(x => x.TotalSpent)
                .Take(5)
                .ToList();

            sw.Stop();

            Console.WriteLine($"=== Пайплайн 4: Топ-5 VIP-клієнтів за оптовими покупками (Час: {sw.Elapsed.TotalMilliseconds:F3} мс) ===");
            foreach (var vip in result)
            {
                Console.WriteLine($"  - {vip.Name} ({vip.City}): {vip.BulkOrders} опт. замовлень, сума {vip.TotalSpent:N2} грн");
            }
            Console.WriteLine();
        }
    }
}