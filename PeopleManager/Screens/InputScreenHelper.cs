namespace PeopleManager.Screens;

public static class InputScreenHelper
{
    public static string GetValidTextInput(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                Console.WriteLine();
                return input;
            }
    
            Console.WriteLine("Input cannot be empty. Please try again.");
        }
    }

    public static int GetValidNumericInput()
    {
        while (true)
        {
            Console.Write("Enter number: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var number))
            {
                Console.WriteLine();
                return number;
            }

            Console.WriteLine("Please enter a number.");
        }
    }
}