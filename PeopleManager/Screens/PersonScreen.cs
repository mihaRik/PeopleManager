using PeopleManager.Domain.Entities;
using PeopleManager.Logic.Services;

namespace PeopleManager.Screens;

public class PersonScreen
{
    private readonly IPeopleService _peopleService;
    private readonly PersonUpdateScreen _personUpdateScreen;
    private readonly IServiceProvider _serviceProvider;

    public PersonScreen(IPeopleService peopleService, PersonUpdateScreen personUpdateScreen, IServiceProvider serviceProvider)
    {
        _peopleService = peopleService;
        _personUpdateScreen = personUpdateScreen;
        _serviceProvider = serviceProvider;
    }

    public async Task DisplayPersonByUsernameAsync(object[] actionParams, CancellationToken cancellationToken)
    {
        try
        {
            if (actionParams.Length < 1 || actionParams[0] is not string username || string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Username is required.");
            }
            else
            {
                Console.WriteLine("Loading...");
                var person = await _peopleService.GetPersonByUsernameAsync(username, cancellationToken)
                    .ConfigureAwait(false);
                Console.Clear();
                DisplayPerson(person);

                var updatePersonOption = new Option { Name = "Update info ℹ️", Action = _personUpdateScreen.DisplayPersonUpdateScreenAsync, ActionParams = [person] };
                var personDetailsOptions = new Dictionary<int, Option>
                {
                    { 1, updatePersonOption },
                    { 2, MainScreen.GetMainScreenOption(_serviceProvider) }
                };

                await OptionsScreen.DisplayOptionsAsync(personDetailsOptions, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (KeyNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    public static void DisplayPerson(Person person)
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

    // public void DisplayCreatePerson()
    // {
    //     _peopleService.CreatePersonAsync();
    // }
}