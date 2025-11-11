using System;
using System.Collections.Generic;

namespace DecoratorPattern
{
    public abstract class DeliveryService
    {
        public string Description { get; protected set; } = "Неизвестная доставка";

        public virtual string GetDescription()
        {
            return Description;
        }

        public abstract decimal CalculateCost();
        public abstract string GetDeliveryTime();
    }

    public class CourierDelivery : DeliveryService
    {
        public CourierDelivery()
        {
            Description = "Курьерская доставка";
        }

        public override decimal CalculateCost()
        {
            return 5.00m;
        }

        public override string GetDeliveryTime()
        {
            return "5-7 рабочих дней";
        }
    }

    public class PostalDelivery : DeliveryService
    {
        public PostalDelivery()
        {
            Description = "Почтовая доставка";
        }

        public override decimal CalculateCost()
        {
            return 3.50m;
        }

        public override string GetDeliveryTime()
        {
            return "7-10 рабочих дней";
        }
    }

    public class PickupDelivery : DeliveryService
    {
        public PickupDelivery()
        {
            Description = "Самовывоз из пункта выдачи";
        }

        public override decimal CalculateCost()
        {
            return 0.00m;
        }

        public override string GetDeliveryTime()
        {
            return "1-2 рабочих дня";
        }
    }

    public class ExpressDeliveryService : DeliveryService
    {
        private readonly DeliveryService _baseDelivery;

        public ExpressDeliveryService(DeliveryService baseDelivery)
        {
            _baseDelivery = baseDelivery;
            Description = _baseDelivery.GetDescription() + " (экспресс)";
        }

        public override decimal CalculateCost()
        {
            return _baseDelivery.CalculateCost() + 10.00m;
        }

        public override string GetDeliveryTime()
        {
            return "1-2 рабочих дня";
        }

        public string TrackExpressDelivery()
        {
            return GetTrackingStatusFromCourierAPI();
        }

        public decimal CalculateExpressCost()
        {
            return GetExpressCostFromAPI();
        }

        private string GetTrackingStatusFromCourierAPI()
        {
            string[] statuses = { "Обрабатывается", "В пути", "Готов к выдаче", "Доставлено" };
            Random random = new Random();
            return statuses[random.Next(statuses.Length)];
        }

        private decimal GetExpressCostFromAPI()
        {
            return CalculateCost() * 1.2m;
        }
    }

    public class ExpressDeliveryDecorator : DeliveryService
    {
        private readonly DeliveryService _deliveryService;
        private readonly ExpressDeliveryService _expressDelivery;

        public ExpressDeliveryDecorator(DeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
            _expressDelivery = new ExpressDeliveryService(deliveryService);
            Description = _expressDelivery.GetDescription();
        }

        public override decimal CalculateCost()
        {
            return _expressDelivery.CalculateExpressCost();
        }

        public override string GetDeliveryTime()
        {
            return _expressDelivery.GetDeliveryTime();
        }

        public string TrackDelivery()
        {
            return _expressDelivery.TrackExpressDelivery();
        }

        public string GetExpressDeliveryInfo()
        {
            return $"Экспресс-доставка: {GetDeliveryTime()}, стоимость: {CalculateCost():F2} руб., статус: {TrackDelivery()}";
        }
    }

    class DecoratorPattern
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Интернет-магазин - Система доставки ===\n");

            List<DeliveryService> deliveryOptions = new List<DeliveryService>
            {
                new CourierDelivery(),
                new PostalDelivery(),
                new PickupDelivery()
            };

            Console.WriteLine("Базовые способы доставки:");
            Console.WriteLine("=========================");

            foreach (var delivery in deliveryOptions)
            {
                DisplayDeliveryInfo(delivery);
            }

            Console.WriteLine("\nСпособы доставки с экспресс-опцией:");
            Console.WriteLine("===================================");

            foreach (var delivery in deliveryOptions)
            {
                var expressDelivery = new ExpressDeliveryDecorator(delivery);
                DisplayExpressDeliveryInfo(expressDelivery);
            }

            Console.WriteLine("\nДетальная информация по курьерской доставке:");
            Console.WriteLine("==========================================");

            DeliveryService courier = new CourierDelivery();
            ExpressDeliveryDecorator expressCourier = new ExpressDeliveryDecorator(courier);

            DisplayDeliveryInfo(courier);
            DisplayExpressDeliveryInfo(expressCourier);

            Console.WriteLine("\nОтслеживание статуса экспресс-доставки:");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"Проверка {i + 1}: {expressCourier.TrackDelivery()}");
            }
        }

        static void DisplayDeliveryInfo(DeliveryService delivery)
        {
            Console.WriteLine($"{delivery.GetDescription()}");
            Console.WriteLine($"  Срок: {delivery.GetDeliveryTime()}");
            Console.WriteLine($"  Стоимость: {delivery.CalculateCost():F2} руб.");
            Console.WriteLine();
        }

        static void DisplayExpressDeliveryInfo(ExpressDeliveryDecorator expressDelivery)
        {
            Console.WriteLine($"{expressDelivery.GetDescription()}");
            Console.WriteLine($"  Срок: {expressDelivery.GetDeliveryTime()}");
            Console.WriteLine($"  Стоимость: {expressDelivery.CalculateCost():F2} руб.");
            Console.WriteLine($"  Текущий статус: {expressDelivery.TrackDelivery()}");
            Console.WriteLine();
        }
    }
}