using System;
using System.Collections.Generic;

namespace CompositePattern
{
    public abstract class OrderComponent
    {
        public abstract string Name { get; }
        public abstract decimal GetPrice();
        public abstract decimal GetOwnPrice(); 
        public virtual void Add(OrderComponent component) =>
            throw new NotImplementedException();
        public virtual void Remove(OrderComponent component) =>
            throw new NotImplementedException();
        public virtual bool IsComposite() => false;
    }

    public class Product : OrderComponent
    {
        private readonly string _name;
        private readonly decimal _price;

        public Product(string name, decimal price)
        {
            _name = name;
            _price = price;
        }

        public override string Name => _name;

        public override decimal GetPrice() => _price;

        public override decimal GetOwnPrice() => _price; 
    }

    public class Box : OrderComponent
    {
        private readonly string _name;
        private readonly List<OrderComponent> _children = new List<OrderComponent>();
        private readonly decimal _packagingCost;

        public Box(string name, decimal packagingCost = 0)
        {
            _name = name;
            _packagingCost = packagingCost;
        }

        public override string Name => _name;

        public override void Add(OrderComponent component) =>
            _children.Add(component);

        public override void Remove(OrderComponent component) =>
            _children.Remove(component);

        public override bool IsComposite() => true;

        public override decimal GetPrice()
        {
            decimal totalPrice = _packagingCost;

            foreach (var child in _children)
                totalPrice += child.GetPrice();

            return totalPrice;
        }

        public override decimal GetOwnPrice() => _packagingCost;

        public decimal GetContentsPrice() 
        {
            decimal contentsPrice = 0;
            foreach (var child in _children)
                contentsPrice += child.GetPrice();
            return contentsPrice;
        }
    }

    public class Order
    {
        private readonly List<OrderComponent> _items = new List<OrderComponent>();

        public void AddItem(OrderComponent item) => _items.Add(item);

        public void RemoveItem(OrderComponent item) => _items.Remove(item);

        public decimal CalculateTotalPrice()
        {
            decimal total = 0;
            foreach (var item in _items)
                total += item.GetPrice();
            return total;
        }

        public void DisplayOrderContents()
        {
            Console.WriteLine("Состав заказа:");
            Console.WriteLine("==================");
            foreach (var item in _items)
                DisplayItem(item, 0);
            Console.WriteLine("==================");
            Console.WriteLine($"Общая стоимость заказа: {CalculateTotalPrice()} руб.");
        }

        private void DisplayItem(OrderComponent item, int depth)
        {
            string indent = new string(' ', depth * 2);

            if (item is Box box)
            {
                Console.WriteLine($"{indent}Коробка: {box.Name}");
                Console.WriteLine($"{indent}    ├─ Стоимость упаковки: {box.GetOwnPrice()} руб.");
                Console.WriteLine($"{indent}    ├─ Стоимость содержимого: {box.GetContentsPrice()} руб.");
                Console.WriteLine($"{indent}    └─ Общая стоимость: {box.GetPrice()} руб.");

                var children = GetBoxChildren(box);
                foreach (var child in children)
                    DisplayItem(child, depth + 1);
            }
            else
            {
                Console.WriteLine($"{indent} {item.Name}: {item.GetPrice()} руб.");
            }
        }

        private List<OrderComponent> GetBoxChildren(Box box)
        {
            var children = new List<OrderComponent>();
            var boxType = box.GetType();
            var field = boxType.GetField("_children",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null)
                children = (List<OrderComponent>)field.GetValue(box);

            return children;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Создаем продукты
            var smartphone = new Product("Смартфон", 50000);
            var headphones = new Product("Наушники", 8000);
            var charger = new Product("Зарядное устройство", 2500);
            var caseProduct = new Product("Чехол", 1200);
            var watch = new Product("Умные часы", 15000);

            // Создаем коробки
            var smallBox = new Box("Маленькая коробка", 50);
            var mediumBox = new Box("Средняя коробка", 100);
            var largeBox = new Box("Большая коробка", 200);
            var giftBox = new Box("Подарочная упаковка", 500);

            // Компоновка заказа
            smallBox.Add(smartphone);
            smallBox.Add(caseProduct);

            mediumBox.Add(smallBox);
            mediumBox.Add(headphones);
            mediumBox.Add(charger);

            largeBox.Add(mediumBox);
            largeBox.Add(watch);

            giftBox.Add(largeBox);

            // Создаем заказ
            var order = new Order();
            order.AddItem(giftBox);
            order.AddItem(new Product("Дополнительная гарантия", 3000));

            // Выводим информацию о заказе
            order.DisplayOrderContents();
        }
    }
}