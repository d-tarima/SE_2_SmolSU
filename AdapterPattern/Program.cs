using System;
interface IRoad
{
    string Name { get; }
    void TravelOn();
}

interface ITransport
{
    void Move(IRoad road);
}

class Highway : IRoad
{
    public string Name { get; }

    public Highway(string name)
    {
        Name = name;
    }

    public void TravelOn()
    {
        Console.WriteLine($"Путешествие по дороге: {Name}");
    }
}

class CountryRoad : IRoad
{
    public string Name { get; }

    public CountryRoad(string name)
    {
        Name = name;
    }

    public void TravelOn()
    {
        Console.WriteLine($"Путешествие по грунтовой дороге: {Name}");
    }
}


class Car : ITransport
{
    public string Model { get; }

    public Car(string model)
    {
        Model = model;
    }

    public void Move(IRoad road)
    {
        Console.WriteLine($"Автомобиль {Model} начал движение");
        road.TravelOn();
    }
}

class Donkey
{
    public string Name { get; }

    public Donkey(string name)
    {
        Name = name;
    }

    public void Eat()
    {
        Console.WriteLine($"Осёл {Name} кушает сено");
    }
}

class Saddle : ITransport
{
    private Donkey _donkey;

    public Saddle(Donkey donkey)
    {
        _donkey = donkey;
    }

    public void Move(IRoad road)
    {
        Console.WriteLine($"Осел {_donkey.Name} с седлом начал движение");
        road.TravelOn();
    }
}

class AdapterPattern
{
    static void Main()
    {

        IRoad highway = new Highway("М-7 Волга");
        IRoad countryRoad = new CountryRoad("Полевая тропа");

        Car car = new Car("Toyota Camry");
        car.Move(highway);

        Console.WriteLine();
        Donkey donkey = new Donkey("Иа");
        donkey.Eat();

        Console.WriteLine();

        Saddle donkeyWithSaddle = new Saddle(donkey);
        donkeyWithSaddle.Move(countryRoad);

        Console.WriteLine();

        IRoad forestPath = new CountryRoad("Лесная тропинка");
        donkeyWithSaddle.Move(forestPath);
    }
}