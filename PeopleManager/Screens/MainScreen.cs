using Microsoft.Extensions.DependencyInjection;

namespace PeopleManager.Screens;

public class MainScreen
{
    private readonly Dictionary<int, Option> _options = new();
    private readonly CancellationTokenSource _cts = new();

    public MainScreen(PeopleListScreen peopleListScreen)
    {
        _options.Add(1, new Option { Name = "List People 👫", Action = peopleListScreen.DisplayPeopleListAsync, ActionParams = [1, 5] });
        _options.Add(2, new Option { Name = "Search 🔍", Action = peopleListScreen.DisplaySearchPeopleAsync, ActionParams = [1, 5, null!] });
        _options.Add(3, CloseOption);
    }

    public static Option GetMainScreenOption(IServiceProvider serviceProvider) => serviceProvider.GetRequiredService<MainScreen>().MainScreenOption;

    private Option MainScreenOption => new() { Name = "Main Screen 🖥️", Action = DisplayMainScreenAsync, ActionParams = [] };

    private Option CloseOption => new() { Name = "Close 🚫", Action = CloseAsync, ActionParams = [] };
    
    public async Task StartAsync()
    {
        await DisplayMainScreenAsync([], _cts.Token).ConfigureAwait(false);
    }

    private async Task DisplayMainScreenAsync(object[] actionParams, CancellationToken cancellationToken)
    {
        Console.Clear();
        Console.WriteLine("***  Welcome to People Manager!  ***");
        Console.WriteLine("***  This is a simple console application to manage people.  ***");
        Console.WriteLine();
        Console.WriteLine("Please select an option:");
        await OptionsScreen.DisplayOptionsAsync(_options, cancellationToken).ConfigureAwait(false);
    }
    
    private async Task CloseAsync(object[] actionParams, CancellationToken cancellationToken)
    {
        await _cts.CancelAsync();
    }
}