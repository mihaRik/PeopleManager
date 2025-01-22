using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PeopleManager.Domain.Entities;
using PeopleManager.Helpers;
using PeopleManager.Logic.Helpers;
using PeopleManager.Logic.Services;
using PeopleManager.Screens;

namespace PeopleManager.IntegrationTests.Screens;

public class MainScreenTests
{
    private static readonly Person[] TestPeople =
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

    private static readonly ConsoleKeyInfo SpacebarKey = new(' ', ConsoleKey.Spacebar, false, false, false);

    private IServiceProvider _serviceProvider;
    private Mock<IPeopleService> _peopleService;
    private Mock<IConsoleWrapper> _console;
    private StringReader _consoleInput;
    private StringWriter _consoleOutput;
    private MainScreen _mainScreen;

    [SetUp]
    public void SetUp()
    {
        _consoleOutput = new StringWriter();
        Console.SetOut(_consoleOutput);

        _peopleService = new Mock<IPeopleService>();
        _console = new Mock<IConsoleWrapper>();

        var services = new ServiceCollection()
            .AddScreens();

        services.AddScoped<IPeopleService>(_ => _peopleService.Object);
        services.AddScoped<IConsoleWrapper>(_ => _console.Object);

        _serviceProvider = services.BuildServiceProvider();
        _mainScreen = _serviceProvider.GetRequiredService<MainScreen>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _consoleInput?.Dispose();
        _consoleOutput?.Dispose();
        Console.SetOut(TextWriter.Null);
    }

    private void SetConsoleInput(string input)
    {
        _consoleInput = new StringReader(input);
        Console.SetIn(_consoleInput);
    }

    [Test]
    public async Task Display_ShouldShowMainScreen()
    {
        // Arrange
        // Act
        SetConsoleInput(3 + Environment.NewLine);
        await _mainScreen.StartAsync().ConfigureAwait(false);

        // Assert
        var output = _consoleOutput.ToString();
        output.Should().ContainAll("Welcome to People Manager", "List People", "Search", "Close",
            "Please enter a number:");
    }

    [Test]
    public async Task Display_ShouldShowListPeopleScreen()
    {
        // Arrange
        _peopleService.Setup(service => service.GetPeopleAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Person>()
            {
                Items = TestPeople,
                CurrentPage = 1,
                PageSize = 5,
                TotalItems = 2
            });

        _console.Setup(c => c.ReadKey())
            .Returns(SpacebarKey); // 2. Press any key to continue

        // Act
        // Steps:
        // 1. List People
        // 2. Press any key to continue
        // 3. Main screen
        // 4. Close
        SetConsoleInput(
            1 + Environment.NewLine + // 1. List People
            4 + Environment.NewLine + // 3. Main screen
            3 + Environment.NewLine); // 4. Close
        await _mainScreen.StartAsync().ConfigureAwait(false);

        // Assert
        var output = _consoleOutput.ToString();
        output.Should().ContainAll("People:", "john_doe", "jane_doe");
    }

    [Test]
    public async Task Display_ShouldShowSearchScreen()
    {
        // Arrange
        const string searchTerm = "doe";
        _peopleService.Setup(p => p.SearchPeopleAsync(
                searchTerm,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Person>
            {
                Items = TestPeople,
                CurrentPage = 1,
                PageSize = 5,
                TotalItems = 2
            });

        _console.Setup(c => c.ReadKey())
            .Returns(SpacebarKey); // 3. Press any key to continue

        // Act
        // Steps:
        // 1. Search
        // 2. Enter search term
        // 3. Press any key to continue
        // 4. Main screen
        // 5. Close
        SetConsoleInput(
            2 + Environment.NewLine +  // 1. Search
            searchTerm + Environment.NewLine + // 2. Enter search term
            4 + Environment.NewLine + // 4. Main screen
            3 + Environment.NewLine); // 5. Close
        await _mainScreen.StartAsync().ConfigureAwait(false);

        // Assert
        var output = _consoleOutput.ToString();
        output.Should().ContainAll("Enter search query:", "john_doe", "jane_doe");
    }
}