namespace PeopleManager.Screens;

public class OptionsScreen
{
    public static async Task DisplayOptionsAsync(Dictionary<int, Option> options, CancellationToken cancellationToken)
    {
        Console.WriteLine();
        Console.WriteLine("******** Options ********");
        Console.WriteLine();
        foreach (var option in options)
        {
            Console.WriteLine($"{option.Key}. {option.Value.Name}");
        }

        var number = GetValidOptionNumber(options);

        var action = options[number];
        await action.Action(action.ActionParams, cancellationToken).ConfigureAwait(false);
    }
    
    private static int GetValidOptionNumber(Dictionary<int, Option> options)
    {
        while (true)
        {
            Console.Write("Please enter a number: ");
            if (int.TryParse(Console.ReadLine(), out var input))
            {
                if (!options.ContainsKey(input))
                {
                    Console.WriteLine($"Please enter a valid number ({string.Join(", ", options.Keys)}).");
                }
                else
                {
                    Console.WriteLine();
                    return input;
                }
            }
            else
            {
                Console.WriteLine("Please enter a valid number.");
            }
        }
    }
}