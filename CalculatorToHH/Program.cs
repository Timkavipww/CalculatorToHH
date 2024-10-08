using System.Globalization;

while (true)
{
    Console.WriteLine("enter expression: \t or press ESC to exit");

    var keyInfo = Console.ReadKey(true);

    if (keyInfo.Key == ConsoleKey.Escape)
    {
        Console.WriteLine("press any key to exit");
        break;
    }
    else
    {
        string inputConsoleRead = Console.ReadLine();

        try
        {
            double resultOfExpression = EvaluateExpression(inputConsoleRead);
            Console.WriteLine($"result: {resultOfExpression}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex.Message}");
        }
    }
}

//main loop to start others methods to evaluate
static double EvaluateExpression(string expression)
{
    var tokens = Tokenize(expression);
    var rpn = ConvertToRPN(tokens);
    return CalculateRPN(rpn);
}

//casting an array of strings
static List<string> Tokenize(string expression)     
{
    var tokens = new List<string>();
    var currentNumber = "";

    for (int i = 0; i < expression.Length; i++)
    {
        char c = expression[i];

        if (char.IsDigit(c) || c == '.')
        {
            currentNumber += c; // собираем число
        }
        else
        {
            if (currentNumber != "")
            {
                tokens.Add(currentNumber);
                currentNumber = "";
            }

            if ("+-*/()".Contains(c))
            {
                tokens.Add(c.ToString());
            }
        }
    }

    if (currentNumber != "")
    {
        tokens.Add(currentNumber);
    }

    return tokens;
}
//casting to evaluate with Shunting-yard algorithm to view like "{digit},{digit},{operator},{operator}" without { and }
static List<string> ConvertToRPN(List<string> tokens)
{
    var output = new List<string>();
    var operators = new Stack<string>();
    var operatorPriority = new Dictionary<string, int>
    {
        { "+", 1 }, { "-", 1 }, { "*", 2 }, { "/", 2 }
    };

    foreach (var token in tokens)
    {
        if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
        {
            output.Add(token);
        }
        else if (token == "(")
        {
            operators.Push(token);
        }
        else if (token == ")")
        {
            while (operators.Count > 0 && operators.Peek() != "(")
            {
                output.Add(operators.Pop());
            }
            operators.Pop();
        }
        else if (operatorPriority.ContainsKey(token))
        {
            while (operators.Count > 0 && operatorPriority[operators.Peek()] >= operatorPriority[token])
            {
                output.Add(operators.Pop());
            }
            operators.Push(token);
        }
    }

    while (operators.Count > 0)
    {
        output.Add(operators.Pop());
    }

    return output;
}

//using base Shunting-yard algorithm to evaluate expression 
static double CalculateRPN(List<string> rpnAlgh)
{
    var stack = new Stack<double>();

    foreach (var token in rpnAlgh)
    {
        if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
        {
            stack.Push(number);
        }
        else
        {
            var rightNum = stack.Pop();
            var leftNum = stack.Pop();
            double result = token switch
            {
                "+" => leftNum + rightNum,
                "-" => leftNum - rightNum,
                "*" => leftNum * rightNum,
                "/" => leftNum / rightNum,
                _ => throw new InvalidOperationException("Неизвестный оператор")
            };
            stack.Push(result);
        }
    }

    return stack.Pop();
}
