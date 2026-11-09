using System;
using System.Collections.Generic;

class NegativeValueException : Exception
{
    public NegativeValueException() { }
    public NegativeValueException(string message) : base(message) { }
    public NegativeValueException(string message, Exception inner) : base(message, inner) { }
}


abstract class DeliveryMethod : IComparable<DeliveryMethod>
{
    public string name { get; set; }
    public int ratePerKg { get; set; }

    public double ReferenceCost;
    protected DeliveryMethod(string name, int ratePerKg)
    {
        this.name = name;
        this.ratePerKg = ratePerKg;
    }

    public abstract double CalculateCost(double weightKg);

    public virtual void PrintInfo()
    {
        Console.WriteLine($"{name}: ціна за кілограм = {ratePerKg}");
    }

    public int CompareTo(DeliveryMethod? other)
    {   
        if(other == null)
        {
            throw new NullReferenceException("Об'єкт, з яким здійснюється порівняння має бути ініціалізованим.");
        }

        if(ReferenceCost < other.ReferenceCost)
        {
            return -1;
        }
        else if(ReferenceCost > other.ReferenceCost)
        {
            return 1;
        }
        return 0;
    }
}

class StandardDelivery : DeliveryMethod
{

    public StandardDelivery() : base("Стандарт", 10)
    {
        ReferenceCost = CalculateCost(1);
    }

    public StandardDelivery(int rate_per_kg) : base("Стандарт", rate_per_kg)
    {
        ReferenceCost = CalculateCost(1);
    }

    public override double CalculateCost(double weightKg) 
    {
        if(weightKg < 0)
        {
            throw new NegativeValueException("Вага має бути додатною або дорівнювати 0.");
        }
        return weightKg * ratePerKg;
    }
}

class ExpressDelivery : DeliveryMethod
{
    private float speedMultiplier;
    private float surcharge;

    public float SpeedMultiplier
    {
        get {return speedMultiplier;}
        set
        {
            if(value <= 0)
            {
                throw new NegativeValueException("Множник швидкості має бути більшим за 0.");
            }
            speedMultiplier = value;
        }
    }

    public float Surcharge
    {
        get {return surcharge;}
        set
        {
            if(value < 0)
            {
                throw new NegativeValueException("Додаткова плата не має бути від'ємною.");
            }
            surcharge = value;
        }
    }

    public ExpressDelivery() : base("Експрес", 20)
    {
        speedMultiplier = 1;
        surcharge = 0;
        ReferenceCost = CalculateCost(1);
    }

    public ExpressDelivery(int rate_per_kg) : base("Експрес", rate_per_kg)
    {
        speedMultiplier = 1;
        surcharge = 0;
        ReferenceCost = CalculateCost(1);
    }

    public override double CalculateCost(double weightKg) 
    {
        if(weightKg < 0)
        {
            throw new NegativeValueException("Вага має бути додатною або дорівнювати 0.");
        }
        return weightKg * ratePerKg * speedMultiplier + surcharge;
    }

    public override void PrintInfo()
    {
        Console.WriteLine($"{name}: ціна за кілограм = {ratePerKg}, множник швидкості = {speedMultiplier}, додаткова плата = {surcharge}");
    }
}



class Program
{
    static void Main(string[] args)
    {
        List<DeliveryMethod> deliveryMethods = [new StandardDelivery(), new ExpressDelivery()];

        bool isRunning = true;

        while (isRunning)
        {
            Console.WriteLine("\n=== Меню ===");
            Console.WriteLine("0. Вихід");
            Console.WriteLine("1. Додати метод доставки");
            Console.WriteLine("2. Розрахувати вартість доставки");
            Console.WriteLine("3. Показати всі методи доставки");
            Console.WriteLine("4. Відсортувати методи від найдешевшого до найдорожчого");

            int choice = ReadInt("Ваш вибір: ", 0, 4);

            isRunning = choice switch
            {
                1 => HandleAddMethod(deliveryMethods),
                2 => HandleComputePrices(deliveryMethods),
                3 => HandleShowAll(deliveryMethods),
                4 => HandleSortRequest(deliveryMethods),
                0 => false,
                _ => true
            };
        }

        Console.WriteLine("Роботу завершено.");
    }

    static bool HandleAddMethod(List<DeliveryMethod> deliveryMethods)
    {
        try
        {
            int delivery_type = ReadInt("Вкажіть вид доставки: 0 - стандарт, 1 - експрес ", 0, 1);
            int rate_per_kg = ReadInt("Вкажіть ціну за кілограм: ", 0, 1000);

            if(delivery_type == 0)
            {
                deliveryMethods.Add(new StandardDelivery(rate_per_kg));
                return true;
            }

            ExpressDelivery new_method = new ExpressDelivery(rate_per_kg);

            // Мінімальне та максимальне значення спеціально залишено за замовчуванням для отримання Exception 
            int surcharge = ReadInt("Введіть додаткову плату за експрес доставку: ");
            new_method.Surcharge = surcharge;

            float speedMultiplier = ReadFloat("Введіть множник швидкості: ");
            new_method.SpeedMultiplier = speedMultiplier;

            deliveryMethods.Add(new_method);
        }
        catch (NegativeValueException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }

        return true;
    }

    static bool HandleShowAll(List<DeliveryMethod> deliveryMethods)
    {
        if (deliveryMethods.Count == 0)
        {
            Console.WriteLine("Немає даних.");
            return true;
        }

        // Демонстрація поліморфізму: виклик virtual/override-методу через базовий тип
        foreach (DeliveryMethod method in deliveryMethods)
        {
            method.PrintInfo();
        }

        return true;
    }

    static bool HandleComputePrices(List<DeliveryMethod> deliveryMethods)
    {

        int weight = ReadInt("Вкажіть вагу посилки: ", 0);

        foreach (var method in deliveryMethods)
        {
            method.PrintInfo();
            Console.Write($" Ціна посилки, враховуючи вагу посилки: {method.CalculateCost(weight)}\n");
        }

        return true;
    }

    static bool HandleSortRequest(List<DeliveryMethod> deliveryMethods)
    {
        deliveryMethods.Sort();
        return true;
    }

    // === 4. Допоміжні методи введення ===

    static float ReadFloat(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (float.TryParse(input, out float value))
            {
                return value;
            }

            Console.WriteLine("Некоректне значення. Введіть число.");
        }
    }

    static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int value) && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Некоректне значення. Введіть ціле число від {min} до {max}.");
        }
    }
}
