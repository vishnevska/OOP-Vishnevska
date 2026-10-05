using System;
using System.Collections.Generic;

namespace IndependentWork2
{
  
    public class Product
    {
        public string Name { get; set; }
        public double Price { get; set; }

        public Product(string name, double price)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Без назви" : name;
            Price = price >= 0 ? price : 0;
        }
    }
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity > 0 ? quantity : 1;
        }

        public double GetSubtotal()
        {
            return Product.Price * Quantity;
        }

        public double GetDiscount()
        {
            if (Product.Price > 500.0)
            {
                return GetSubtotal() * 0.10;
            }
            return 0;
        }

        public double GetFinalTotal()
        {
            return GetSubtotal() - GetDiscount();
        }
    }

    public class Cart
    {
        private List<CartItem> _items = new List<CartItem>();

        public void AddItem(Product product, int quantity)
        {
            _items.Add(new CartItem(product, quantity));
        }

        public double GetTotal()
        {
            double total = 0;
            foreach (var item in _items)
            {
                total += item.GetFinalTotal();
            }
            return total;
        }

        public void PrintCart()
        {
            int index = 1;
            foreach (var item in _items)
            {
                double discount = item.GetDiscount();
                string discountInfo = discount > 0 ? $" (Знижка 10%: -{discount:F2} грн)" : "";
                Console.WriteLine($"{index++}. {item.Product.Name} x{item.Quantity} — Ціна: {item.Product.Price} грн | Сума: {item.GetSubtotal()} грн{discountInfo}");
            }
        }
    }

    public static class ObjectOrientedDemo
    {
        public static void Run()
        {
            Console.WriteLine(" 2. ОБ'ЄКТНО-ОРІЄНТОВАНИЙ ПІДХІД ");

            Cart cart = new Cart();
            cart.AddItem(new Product("Ноутбук", 25000.0), 1);
            cart.AddItem(new Product("Мишка", 450.0), 2);
            cart.AddItem(new Product("Клавіатура", 800.0), 1);
            cart.AddItem(new Product("Монітор", 6500.0), 1);

            cart.PrintCart();

            Console.WriteLine($"\nЗагальна вартість кошика (ООП): {cart.GetTotal():F2} грн\n");
        }
    }
}