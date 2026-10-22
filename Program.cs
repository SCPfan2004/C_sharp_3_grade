class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        List<string> history = [];
        
        bool isRunning = true;

        while(isRunning)
        {
            Console.WriteLine("\n=== Меню ===");
            Console.WriteLine("1. Ввести вираз");
            Console.WriteLine("2. Показати історію обчислень");
            Console.WriteLine("0. Вихід");            
            Console.WriteLine("\nБудь ласка, оберіть дію:");
            
            int userInput = ReadInt(0, 2);

            isRunning = userInput switch
            {
              1 => HandleNewExpression(history),
              2 => ShowHistory(history),
              0 => false,
              _ => false,
            };
        }

        Console.WriteLine("Роботу програми завершено.");
    }

    static bool ShowHistory(List<string> history)
    {
        Console.WriteLine("\nІсторія виразів:\n");
        foreach(string line in history)
        {
            Console.WriteLine(line, "\n");
        }
        return true;
    }

    static bool HandleNewExpression(List<string> history)
    {
        string? userInput = null;
        Console.WriteLine("Будь ласка, введіть вираз, наприклад: 12 + 7\n");
        userInput = Console.ReadLine();

        while(string.IsNullOrEmpty(userInput)) {
            Console.WriteLine("Введене значення не містить інформації. Будь ласка, спробуйте ще раз:\n");
            userInput = Console.ReadLine();
        }
        
        string[] parsedArray = new string[3];
        
        if(ParseExpression(userInput, parsedArray))
        {
            string result = parsedArray[0] + " " + parsedArray[1] + " " + parsedArray[2] + " = " + (ComputeExpression(parsedArray)).ToString();
            Console.WriteLine($"\nРезультат обчислення: {result}\n");
            history.Add(result);   
        }
        else
        {
            Console.WriteLine("Введений вираз містить непідтримувані символи або не містить операнда... Операцію скасовано, вираз не було додано до історії.");
        }
        return true;
    }

    static bool ParseExpression(string expression, string[] parsedArray)
    {
        bool hitOperand = false;
        
        string firstNumber = "";
        string secondNumber = "";
        string operand = "";

        foreach(char sign in expression)
        {
            if(char.IsNumber(sign) && !hitOperand)
            {
                firstNumber += sign;
            }
            else if(char.IsNumber(sign) && hitOperand)
            {
                secondNumber += sign;
            }
            else if(sign == '+' || sign == '-' || sign == '*' || sign == '/')
            {
                operand += sign;
                hitOperand = true;
            }
            else if(sign != ' ')
            {
                return false;
            }
        }

        if(!hitOperand)
        {
            return false;
        }

        parsedArray[0] = firstNumber;
        parsedArray[1] = operand;
        parsedArray[2] = secondNumber;

        return true;
    }

    static int ComputeExpression(string[] parsedExpression)
    {
        if(!int.TryParse(parsedExpression[0], out int first) || !int.TryParse(parsedExpression[2], out int second))
        {
            Console.WriteLine($"\nТехнічний збій: не вийшло відокремити введені числа. number_1 = {parsedExpression[0]} number_2 = {parsedExpression[2]}\n");
            return 0;
        }

        int result = parsedExpression[1] switch
        {
            "+" => first + second,
            "-" => first - second,
            "*" => first * second,
            "/" => first / second,
            _ => 0,
        };

        return result;
    }

    static bool showHistory()
    {
        return true;
    }

    static int ReadInt(int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            string? userInput = Console.ReadLine();

            int parsedInput;
            if(int.TryParse(userInput, out parsedInput) && parsedInput >= min && parsedInput <= max)
            {
                return parsedInput;
            }

            Console.WriteLine($"\nВведене значення не є підтримуваним. Будь ласка, введіть число від {min} до {max}:");
        }
    }
}