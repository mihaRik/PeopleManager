using PeopleManager.Domain.Entities;
using PeopleManager.Logic.Helpers;
using PeopleManager.Logic.Services;

namespace PeopleManager.Screens;

public class PeopleScreen
{
    private const int DefaultPageSize = 5;

    private readonly IPeopleService _peopleService;
    private readonly PersonScreen _personScreen;
    private readonly IServiceProvider _serviceProvider;

    public PeopleScreen(IPeopleService peopleService, PersonScreen personScreen, IServiceProvider serviceProvider)
    {
        _peopleService = peopleService;
        _personScreen = personScreen;
        _serviceProvider = serviceProvider;
    }

    public Option ListPeopleOption => new() { Name = "List People 👫", Action = DisplayPeopleListAsync, ActionParams = [1, DefaultPageSize] };
    public Option SearchOption => new() { Name = "Search 🔍", Action = DisplaySearchPeopleAsync, ActionParams = [1, DefaultPageSize, null!] };
    private static Option NextPage (Option option) => new() { Name = "Next Page ➡️", Action = option.Action, ActionParams = option.ActionParams};
    private static Option PreviousPage (Option option) => new() { Name = "Previous Page ⬅️", Action = option.Action, ActionParams = option.ActionParams};

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
        var peopleListOptions = GetPeopleListOptions(pagedResult, listPeopleAsync);

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
        var peopleListOptions = GetPeopleListOptions(pagedResult, searchPeopleAsync);

        await OptionsScreen.DisplayOptionsAsync(peopleListOptions, cancellationToken).ConfigureAwait(false);
    }
    
    private Dictionary<int, Option> GetPeopleListOptions(
        PagedResult<Person> pagedResult,
        Option navigationOption)
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
        
        var optionIndex = pagedResult.Items.Count();

        Console.WriteLine();
        if (!pagedResult.Items.Any())
        {
            Console.WriteLine("No people found.");
        }
        else
        {
            if (pagedResult.CurrentPage < pagedResult.TotalPages)
            {
                var nextPageOption = navigationOption.Clone();
                nextPageOption.ActionParams[0] = pagedResult.CurrentPage + 1;
                people.Add(++optionIndex, NextPage(nextPageOption));
            }

            if (pagedResult.CurrentPage > 1)
            {
                var previousPageOption = navigationOption.Clone();
                previousPageOption.ActionParams[0] = pagedResult.CurrentPage - 1;
                people.Add(++optionIndex, PreviousPage(previousPageOption));
            }

            Console.WriteLine($"Page #{pagedResult.CurrentPage} of {(pagedResult.TotalPages < 1 ? 1 : pagedResult.TotalPages)}");
        }

        people.Add(++optionIndex, SearchOption);
        people.Add(++optionIndex, MainScreen.GetMainScreenOption(_serviceProvider));


        return people;
    }
}