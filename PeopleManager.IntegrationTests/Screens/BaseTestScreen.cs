using Microsoft.Extensions.DependencyInjection;
using Moq;
using PeopleManager.Domain.Entities;
using PeopleManager.Logic.Services;
using PeopleManager.Screens;

namespace PeopleManager.IntegrationTests.Screens;

public class BaseTestScreen
{
    protected static readonly Person[] TestPeople =
    [
        new()
        {
            FirstName = "John",
            LastName = "Doe",
            UserName = "john_doe",
        },
        new()
        {
            FirstName = "Jane",
            LastName = "Doe",
            UserName = "jane_doe",
        }
    ];

    protected IServiceProvider ServiceProvider;
    protected Mock<IPeopleService> PeopleService;
    protected StringReader ConsoleInput;
    protected StringWriter ConsoleOutput;
    protected MainScreen MainScreen;

    [SetUp]
    public virtual void SetUp()
    {
        ConsoleOutput = new StringWriter();
        Console.SetOut(ConsoleOutput);

        PeopleService = new Mock<IPeopleService>();

        var services = new ServiceCollection()
            .AddScreens();

        services.AddScoped<IPeopleService>(_ => PeopleService.Object);

        ServiceProvider = services.BuildServiceProvider();
        MainScreen = ServiceProvider.GetRequiredService<MainScreen>();
    }

    [TearDown]
    public virtual void TearDown()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        ConsoleInput?.Dispose();
        ConsoleOutput?.Dispose();
        Console.SetOut(TextWriter.Null);
    }
    
    protected void SetConsoleInput(string input)
    {
        ConsoleInput = new StringReader(input);
        Console.SetIn(ConsoleInput);
    }
}