using System;
using System.Collections.Generic;
using System.Linq;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
}

public class UserRepository
{
    private readonly List<User> _users = new();

    public void AddUser(User user)
    {
        _users.Add(user);
        Console.WriteLine($"Пользователь {user.Name} сохранен в базе пользователей");
    }

    public User GetUser(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public List<User> GetAllUsers()
    {
        return _users.ToList();
    }
}

public class OrderRepository
{
    private readonly List<Order> _orders = new();

    public void AddOrder(Order order)
    {
        _orders.Add(order);
        Console.WriteLine($"Заказ на сумму {order.Amount} сохранен в базе заказов");
    }

    public List<Order> GetOrdersByUser(int userId)
    {
        return _orders.Where(o => o.UserId == userId).ToList();
    }

    public List<Order> GetAllOrders()
    {
        return _orders.ToList();
    }
}

public class DataGateway
{
    private readonly UserRepository _userRepository;
    private readonly OrderRepository _orderRepository;

    public DataGateway()
    {
        _userRepository = new UserRepository();
        _orderRepository = new OrderRepository();
    }

    public void CreateUserWithOrder(User user, decimal initialOrderAmount)
    {
        _userRepository.AddUser(user);

        var newOrder = new Order
        {
            Id = _orderRepository.GetAllOrders().Count + 1,
            UserId = user.Id,
            Amount = initialOrderAmount,
            OrderDate = DateTime.Now
        };

        _orderRepository.AddOrder(newOrder);
        Console.WriteLine($"Создан пользователь {user.Name} с первоначальным заказом на сумму {initialOrderAmount}");
    }

    public void ShowUserProfile(int userId)
    {
        var user = _userRepository.GetUser(userId);
        if (user == null)
        {
            Console.WriteLine("Пользователь не найден");
            return;
        }

        var orders = _orderRepository.GetOrdersByUser(userId);

        Console.WriteLine($"\n--- Профиль пользователя ---");
        Console.WriteLine($"Имя: {user.Name}");
        Console.WriteLine($"Email: {user.Email}");
        Console.WriteLine($"Количество заказов: {orders.Count}");
        Console.WriteLine("Список заказов:");
        foreach (var order in orders)
        {
            Console.WriteLine($"- Заказ #{order.Id} на сумму {order.Amount} от {order.OrderDate:dd.MM.yyyy}");
        }
        Console.WriteLine("----------------------------\n");
    }

    public void AddUser(User user)
    {
        _userRepository.AddUser(user);
    }

    public void AddOrder(Order order)
    {
        _orderRepository.AddOrder(order);
    }
}

class FacadePattern
{
    static void Main()
    {
        var databaseGateway = new DataGateway();

        var user1 = new User { Id = 1, Name = "Иван Петров", Email = "ivan@example.com" };
        var user2 = new User { Id = 2, Name = "Мария Сидорова", Email = "maria@example.com" };

        databaseGateway.CreateUserWithOrder(user1, 1500.50m);
        databaseGateway.CreateUserWithOrder(user2, 2750.00m);

        databaseGateway.AddOrder(new Order
        {
            Id = 3,
            UserId = 1,
            Amount = 300.00m,
            OrderDate = DateTime.Now.AddDays(-1)
        });

        databaseGateway.ShowUserProfile(1);
        databaseGateway.ShowUserProfile(2);

        databaseGateway.ShowUserProfile(99);
    }
}