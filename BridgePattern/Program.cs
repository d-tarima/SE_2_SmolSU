using System;
using System.IO;

public interface ILoggingImplementation
{
    void Log(string message, LogLevel level);
}

public enum LogLevel
{
    Info,
    Warning,
    Error
}


public class ConsoleLogging : ILoggingImplementation
{
    public void Log(string message, LogLevel level)
    {
        var color = Console.ForegroundColor;
        switch (level)
        {
            case LogLevel.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogLevel.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.White;
                break;
        }

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {level}: {message}");
        Console.ForegroundColor = color;
    }
}

public class FileLogging : ILoggingImplementation
{
    private readonly string _filePath;

    public FileLogging(string filePath)
    {
        _filePath = filePath;
    }

    public void Log(string message, LogLevel level)
    {
        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {level}: {message}{Environment.NewLine}";
        File.AppendAllText(_filePath, logEntry);
    }
}

public abstract class Logger
{
    protected ILoggingImplementation _loggingImplementation;

    protected Logger(ILoggingImplementation loggingImplementation)
    {
        _loggingImplementation = loggingImplementation;
    }

    public abstract void Log(string message, LogLevel level);
}

public class StoreLogger : Logger
{
    public StoreLogger(ILoggingImplementation loggingImplementation) : base(loggingImplementation)
    {
    }

    public override void Log(string message, LogLevel level)
    {
        var processedMessage = $"Магазин: {message}";
        _loggingImplementation.Log(processedMessage, level);
    }

    public void LogUserAction(string userName, string action)
    {
        Log($"Пользователь {userName} выполнил действие: {action}", LogLevel.Info);
    }

    public void LogPurchase(string orderId, decimal amount)
    {
        Log($"Заказ {orderId} на сумму {amount:F2} руб. успешно оформлен", LogLevel.Info);
    }

    public void LogError(string error, string component)
    {
        Log($"Ошибка в компоненте {component}: {error}", LogLevel.Error);
    }
}


class BridgePattern
{
    static void Main(string[] args)
    {
        Console.WriteLine("Демонстрация системы логирования магазина\n");

        var consoleLogging = new ConsoleLogging();
        var storeConsoleLogger = new StoreLogger(consoleLogging);

        storeConsoleLogger.LogUserAction("Иван Петров", "Вход в систему");
        storeConsoleLogger.LogPurchase("ORD-12345", 1599.99m);
        storeConsoleLogger.LogError("Недостаточно товара на складе", "Корзина");

        Console.WriteLine("\n" + new string('-', 50) + "\n");

        var fileLogging = new FileLogging("store_log.txt");
        var storeFileLogger = new StoreLogger(fileLogging);

        storeFileLogger.LogUserAction("Анна Сидорова", "Добавление товара в корзину");
        storeFileLogger.LogPurchase("ORD-12346", 2750.50m);
        storeFileLogger.LogError("Таймаут подключения к платежной системе", "Оплата");

        Console.WriteLine("Логи успешно записаны в файл store_log.txt");

    }
}