using PeopleManager.Domain.Entities;
using PeopleManager.Logic.Helpers;
using PeopleManager.Logic.Services;

namespace PeopleManager;

public class OptionSelector
{
    private readonly IPeopleService _peopleService;
    private readonly Dictionary<int, Option> _options = new();
    private readonly CancellationTokenSource _cts = new();

    private Option _exitOption => new() { Name = "Exit 🚫", Action = ExitAsync, ActionParams = [] };

    private Option _mainScreenOption => new() { Name = "Main Screen 🖥️", Action = DisplayMainScreenAsync, ActionParams = [] };
    
    private Option GetBackToListOption (Option option) => new() { Name = "Back to list ⏪", Action = option.Action, ActionParams = option.ActionParams};

    public OptionSelector(IPeopleService peopleService)
    {
        _peopleService = peopleService;

        _options.Add(1, new Option { Name = "List People 👫", Action = ListPeopleAsync, ActionParams = [1, 2] });
        _options.Add(2, new Option { Name = "Search 🔍", Action = SearchPeopleAsync, ActionParams = [1, 2, null] });
        _options.Add(10, _exitOption);
    }

    public async Task StartAsync()
    {
        await DisplayMainScreenAsync([], _cts.Token);
    }

    private async Task ListPeopleAsync(object[] actionParams, CancellationToken cancellationToken)
    {
        Console.Clear();
        var page = 1;
        var pageSize = 10;

        if (actionParams.Length == 2)
        {
            page = (int)actionParams[0];
            pageSize = (int)actionParams[1];
        }

        Console.WriteLine("Loading...");
        var pagedResult = await _peopleService.GetPeopleAsync(page, pageSize, cancellationToken).ConfigureAwait(false);
        var people = new Dictionary<int, Option>();

        var listPeopleAsync = new Option { Action = ListPeopleAsync, ActionParams = [page, pageSize] };
        await DisplayPeopleListAsync(pagedResult, people, listPeopleAsync,  cancellationToken).ConfigureAwait(false);

        var navigation = new Dictionary<int, Option>
        {
            { pageSize + 1, GetBackToListOption(listPeopleAsync) },
            { pageSize + 2, _mainScreenOption }
        };
        await DisplayOptionsAsync(people, cancellationToken).ConfigureAwait(false);

        await DisplayOptionsAsync(navigation, cancellationToken).ConfigureAwait(false);
    }

    private async Task GetPersonByUsernameAsync(object[] actionParams, CancellationToken cancellationToken)
    {
        try
        {
            if (actionParams.Length < 1 || actionParams[0] is not string username || string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Username is required.");
            }
            else
            {
                var person = await _peopleService.GetPersonByUsernameAsync(username, cancellationToken)
                    .ConfigureAwait(false);
                Console.Clear();
                DisplayPerson(person);
            }
        }
        catch (KeyNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private async Task SearchPeopleAsync(object[] actionParams, CancellationToken cancellationToken)
    {
        Console.Clear();
        var page = 1;
        var pageSize = 10;
        string searchQuery = null;

        if (actionParams.Length == 3)
        {
            page = (int)actionParams[0];
            pageSize = (int)actionParams[1];
            searchQuery = (string)actionParams[2];
        }

        if (searchQuery is null)
        {
            searchQuery = GetValidStringInput();
        }

        Console.WriteLine("Loading..."); 
        var pagedResult = await _peopleService.SearchPeopleAsync(searchQuery, page, pageSize, cancellationToken).ConfigureAwait(false);
        var people = new Dictionary<int, Option>();

        var searchPeopleAsync = new Option { Action = SearchPeopleAsync, ActionParams = [page, pageSize, searchQuery] };
        await DisplayPeopleListAsync(pagedResult, people, searchPeopleAsync, cancellationToken).ConfigureAwait(false);

        var navigation = new Dictionary<int, Option>
        {
            { pageSize + 1, GetBackToListOption(searchPeopleAsync) },
            { pageSize + 2, _mainScreenOption }
        };
        await DisplayOptionsAsync(people, cancellationToken).ConfigureAwait(false);

        await DisplayOptionsAsync(navigation, cancellationToken).ConfigureAwait(false);
    }

    private async Task ExitAsync(object[] actionParams, CancellationToken cancellationToken)
    {
        await _cts.CancelAsync();
    }

    private static int GetValidNumericInput(Dictionary<int, Option> options)
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

    private static string GetValidStringInput()
    {
        while (true)
        {
            Console.Write("Search query: ");
            var input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                Console.WriteLine();
                return input;
            }

            Console.WriteLine("Please enter a search query.");
        }
    }

    private async Task GetArrowKey<T>(
        PagedResult<T> pagedResult,
        Option navigationFunction,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            Console.WriteLine("You can use arrows(<- | ->) to navigate, or press any key to continue.");
            var key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow when pagedResult.CurrentPage > 1:
                    navigationFunction.ActionParams[0] = pagedResult.CurrentPage - 1;
                    break;
                case ConsoleKey.LeftArrow:
                    Console.WriteLine();
                    Console.WriteLine("You are already on the first page.");
                    continue;
                case ConsoleKey.RightArrow when pagedResult.CurrentPage < pagedResult.TotalPages:
                    navigationFunction.ActionParams[0] = pagedResult.CurrentPage + 1;
                    break;
                case ConsoleKey.RightArrow:
                    Console.WriteLine();
                    Console.WriteLine("You are already on the last page.");
                    continue;
                default:
                    return;
            }

            await navigationFunction.Action(navigationFunction.ActionParams, cancellationToken).ConfigureAwait(false);

            break;
        }
    }
    
    private async Task DisplayPeopleListAsync(
        PagedResult<Person> pagedResult,
        Dictionary<int, Option> people,
        Option navigationFunction,
        CancellationToken cancellationToken)
    {
        Console.Clear();
        Console.WriteLine("People: ");
        for (var i = 0; i < pagedResult.Items.Count(); i++)
        {
            var person = pagedResult.Items.ElementAt(i);

            Console.WriteLine($"{i + 1}.{person.FullName} ({person.UserName})");
            people.Add(i + 1, new Option
            {
                Name = person.FullName,
                Action = GetPersonByUsernameAsync,
                ActionParams = [person.UserName]
            });
        }
        people.Add(pagedResult.Items.Count() + 1, GetBackToListOption(navigationFunction));
        people.Add(pagedResult.Items.Count() + 2, _mainScreenOption);
        
        Console.WriteLine();
        if (!pagedResult.Items.Any())
        {
            Console.WriteLine("No people found.");
            return;
        }

        Console.WriteLine($"Page #{pagedResult.CurrentPage} of {(pagedResult.TotalPages < 1 ? 1 : pagedResult.TotalPages)}");

        await GetArrowKey(pagedResult, navigationFunction, cancellationToken).ConfigureAwait(false);

        Console.Clear();
        Console.WriteLine("Enter a number of a person to get detailed information, or press any key to continue.");
    }

    private async Task DisplayMainScreenAsync(object[] actionParams, CancellationToken cancellationToken)
    {
        Console.Clear();
        Console.WriteLine("***  Welcome to the People Manager!  ***");
        Console.WriteLine("***  This is a simple console application to manage people.  ***");
        Console.WriteLine();
        Console.WriteLine("Please select an option:");
        await DisplayOptionsAsync(_options, cancellationToken).ConfigureAwait(false);
    }

    private static async Task DisplayOptionsAsync(Dictionary<int, Option> options, CancellationToken cancellationToken)
    {
        Console.WriteLine();
        Console.WriteLine("******** Options ********");
        Console.WriteLine();
        foreach (var option in options)
        {
            Console.WriteLine($"{option.Key}. {option.Value.Name}");
        }

        var number = GetValidNumericInput(options);

        var action = options[number];
        await action.Action(action.ActionParams, cancellationToken).ConfigureAwait(false);
    }

    private void DisplayPerson(Person person)
    {
        var addresses = person.AddressInfo?.Where(a => a is not null).Select(a => a.Address).ToList();
        var emails = person.Emails?.Where(e => e is not null).ToList();
        var features = person.Features?.ToList();
        var friends = person.Friends?.Where(f => f is not null).Select(f => f.FullName).ToList();
        var trips = person.Trips?.Where(t => t is not null).Select(t => t.Name).ToList();

        Console.WriteLine($"Detailed information about {person.FullName} ({person.UserName}):");
        Console.WriteLine($"Gender: {person.Gender}");
        Console.WriteLine($"Age: {person.Age?.ToString() ?? "N/A"}");
        Console.WriteLine($"Home Address: {person.HomeAddress?.Address ?? "N/A"}");
        Console.WriteLine($"Best Friend: {person.BestFriend?.FullName ?? "N/A"}");
        Console.WriteLine($"Favorite Feature: {person.FavoriteFeature}");
        Console.WriteLine($"Emails: {(emails is not null && emails.Count != 0 ? string.Join(" | ", emails) : "N/A")}");
        Console.WriteLine(
            $"Addresses: {(addresses is not null && addresses.Count != 0 ? string.Join(" | ", addresses) : "N/A")}");
        Console.WriteLine(
            $"Features: {(features is not null && features.Count != 0 ? string.Join(" | ", features) : "N/A")}");
        Console.WriteLine(
            $"Friends: {(friends is not null && friends.Count != 0 ? string.Join(" | ", friends) : "N/A")}");
        Console.WriteLine($"Trips: {(trips is not null && trips.Count != 0 ? string.Join(" | ", trips) : "N/A")}");
    }
}