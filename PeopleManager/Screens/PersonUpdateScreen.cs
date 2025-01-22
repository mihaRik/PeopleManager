using System.Reflection;
using PeopleManager.Domain.Entities;
using PeopleManager.Logic.Services;

namespace PeopleManager.Screens;

public class PersonUpdateScreen : IDisposable
{
    private readonly IPeopleService _peopleService;

    public PersonUpdateScreen(IPeopleService peopleService)
    {
        _peopleService = peopleService;
    }

    public async Task DisplayPersonUpdateScreenAsync(object[] actionParams, CancellationToken cancellationToken)
    {
        if (actionParams.Length < 1 || actionParams[0] is not Person person)
        {
            Console.WriteLine("Person is required.");
            return;
        }

        Console.WriteLine();
        var properties = typeof(Person).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name != nameof(Person.FullName));
        var propertiesOptions = properties.Select((p, i) => new { Key = i + 1, Value = p.Name })
            .ToDictionary(p => p.Key, p => new Option
            {
                Name = p.Value,
                Action = DisplayPropertyUpdateScreenAsync,
                ActionParams = [person, p.Value],
            });

        await OptionsScreen.DisplayOptionsAsync(propertiesOptions, cancellationToken).ConfigureAwait(false);
    }
    
    private async Task DisplayPropertyUpdateScreenAsync(object[] actionParams, CancellationToken cancellationToken)
    {
        if (actionParams.Length < 2 || actionParams[0] is not Person person ||
            actionParams[1] is not string propertyName || string.IsNullOrEmpty(propertyName))
        {
            Console.WriteLine("Person and property name are required.");
            return;
        }

        try
        {
            var propertyInfo = typeof(Person).GetProperty(propertyName);

            if (propertyInfo == null)
            {
                Console.WriteLine("Property not found.");
                await DisplayPersonUpdateScreenAsync([person], cancellationToken).ConfigureAwait(false);
                return;
            }
            
            if (propertyInfo.PropertyType != typeof(string) && propertyInfo.PropertyType != typeof(long?))
            {
                Console.WriteLine("Property update for this field is not supported.");
                await DisplayPersonUpdateScreenAsync([person], cancellationToken).ConfigureAwait(false);
                return; 
            }

            Console.WriteLine();
            Console.WriteLine($"Updating the {propertyInfo.Name}");
            object newValue = null!;
            if (propertyInfo.PropertyType == typeof(string))
            {
                newValue = InputScreenHelper.GetValidTextInput("Enter new value for the field");
            }
            else if (propertyInfo.PropertyType == typeof(long?))
            {
                newValue = (long)InputScreenHelper.GetValidNumericInput();
            }
            
            Console.WriteLine();
            Console.WriteLine("Updating...");
            var updatedPerson = await _peopleService.UpdatePersonAsync(person, propertyInfo, newValue, cancellationToken).ConfigureAwait(false);
            Console.Clear();
            PersonScreen.DisplayPerson(updatedPerson);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine();
            await DisplayPersonUpdateScreenAsync([person], cancellationToken).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _peopleService.Dispose();
        }
    }

    ~PersonUpdateScreen()
    {
        Dispose(false);
    }
}