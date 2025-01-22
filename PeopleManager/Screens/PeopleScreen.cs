using PeopleManager.Domain.Entities;
using PeopleManager.Helpers;
using PeopleManager.Logic.Helpers;
using PeopleManager.Logic.Services;

namespace PeopleManager.Screens;

public class PeopleListScreen
{
    private readonly IPeopleService _peopleService;
    private readonly PersonScreen _personScreen;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsoleWrapper _console;

    public PeopleListScreen(IPeopleService peopleService, PersonScreen personScreen, IServiceProvider serviceProvider, IConsoleWrapper console)
    {
        _peopleService = peopleService;
        _personScreen = personScreen;
        _serviceProvider = serviceProvider;
        _console = console;
    }

    private static Option GetBackToListOption (Option option) => new() { Name = "Back to list ⏪", Action = option.Action, ActionParams = option.ActionParams};

    public async Task DisplayPeopleListAsync(object[] actionParams, CancellationToken cancellationToken)
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

        var listPeopleAsync = new Option { Action = DisplayPeopleListAsync, ActionParams = [page, pageSize] };
        var peopleListOptions = await GetPeopleListOptionsAsync(pagedResult, listPeopleAsync,  cancellationToken).ConfigureAwait(false);

        await OptionsScreen.DisplayOptionsAsync(peopleListOptions, cancellationToken).ConfigureAwait(false);
    }
    
    public async Task DisplaySearchPeopleAsync(object[] actionParams, CancellationToken cancellationToken)
    {
        Console.Clear();
        var page = 1;
        var pageSize = 10;
        string searchQuery = null!;

        if (actionParams.Length == 3)
        {
            page = (int)actionParams[0];
            pageSize = (int)actionParams[1];
            searchQuery = (string)actionParams[2];
        }

        searchQuery ??= InputScreenHelper.GetValidTextInput("Enter search query");

        Console.WriteLine("Loading..."); 
        var pagedResult = await _peopleService.SearchPeopleAsync(searchQuery, page, pageSize, cancellationToken).ConfigureAwait(false);

        var searchPeopleAsync = new Option { Action = DisplaySearchPeopleAsync, ActionParams = [page, pageSize, searchQuery] };
        var peopleListOptions = await GetPeopleListOptionsAsync(pagedResult, searchPeopleAsync, cancellationToken).ConfigureAwait(false);

        await OptionsScreen.DisplayOptionsAsync(peopleListOptions, cancellationToken).ConfigureAwait(false);
    }
    
    private async Task<Dictionary<int, Option>> GetPeopleListOptionsAsync(
        PagedResult<Person> pagedResult,
        Option navigationFunction,
        CancellationToken cancellationToken)
    {
        var people = new Dictionary<int, Option>();

        Console.Clear();
        Console.WriteLine("People: ");
        for (var i = 0; i < pagedResult.Items.Count(); i++)
        {
            var person = pagedResult.Items.ElementAt(i);

            Console.WriteLine($"{i + 1}.{person.FullName} ({person.UserName})");
            people.Add(i + 1, new Option
            {
                Name = person.FullName,
                Action = _personScreen.DisplayPersonByUsernameAsync,
                ActionParams = [person.UserName]
            });
        }
        people.Add(pagedResult.Items.Count() + 1, GetBackToListOption(navigationFunction));
        people.Add(pagedResult.Items.Count() + 2, MainScreen.GetMainScreenOption(_serviceProvider));
        
        Console.WriteLine();
        if (!pagedResult.Items.Any())
        {
            Console.WriteLine("No people found.");
            return people;
        }

        Console.WriteLine($"Page #{pagedResult.CurrentPage} of {(pagedResult.TotalPages < 1 ? 1 : pagedResult.TotalPages)}");

        await GetArrowKey(pagedResult, navigationFunction, cancellationToken).ConfigureAwait(false);

        Console.Clear();
        Console.WriteLine("Enter a number of a person to get detailed information, or press any key to continue.");
        
        return people;
    }
    
    private async Task GetArrowKey<T>(
        PagedResult<T> pagedResult,
        Option navigationFunction,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            Console.WriteLine("You can use arrows(<- | ->) to navigate, or press any key to continue.");
            var key = _console.ReadKey();
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
}