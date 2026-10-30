using System;
using System.Collections.Generic;

namespace lab7v3
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public class Order : IEntity
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int CustomerId { get; set; }

        public Order() { }

        public Order(int id, DateTime orderDate, decimal totalAmount, int customerId)
        {
            Id = id;
            OrderDate = orderDate;
            TotalAmount = totalAmount;
            CustomerId = customerId;
        }

        public override string ToString()
        {
            return $"Замовлення #{Id} | Дата: {OrderDate:yyyy-MM-dd} | Сума: {TotalAmount:C} | Клієнт ID: {CustomerId}";
        }
    }

    public interface IRepository<T> where T : class, IEntity
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(int id);
    }

    public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly List<T> _items = new List<T>();

        public T GetById(int id)
        {
            return _items.Find(item => item.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return _items;
        }

        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _items.Add(item);
        }

        public void Update(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            int index = _items.FindIndex(existingItem => existingItem.Id == item.Id);
            if (index != -1)
            {
                _items[index] = item;
            }
        }

        public void Delete(int id)
        {
            T item = GetById(id);
            if (item != null)
            {
                _items.Remove(item);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IRepository<Order> orderRepository = new InMemoryRepository<Order>();

            orderRepository.Add(new Order(1, DateTime.Now.AddDays(-2), 1500.50m, 101));
            orderRepository.Add(new Order(2, DateTime.Now.AddDays(-1), 3200.00m, 102));

            Console.WriteLine(" Усі замовлення ");
            foreach (var order in orderRepository.GetAll())
            {
                Console.WriteLine(order);
            }

            Console.WriteLine("\n Оновлення замовлення #1 ");
            orderRepository.Update(new Order(1, DateTime.Now.AddDays(-2), 1800.00m, 101));
            Console.WriteLine(orderRepository.GetById(1));

            Console.WriteLine("\n Видалення замовлення #2 ");
            orderRepository.Delete(2);

            Console.WriteLine("\n Оновлений список ");
            foreach (var order in orderRepository.GetAll())
            {
                Console.WriteLine(order);
            }
        }
    }
}