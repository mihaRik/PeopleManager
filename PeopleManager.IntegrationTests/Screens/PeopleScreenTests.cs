using FluentAssertions;
using Moq;
using PeopleManager.Domain.Entities;
using PeopleManager.Logic.Helpers;

namespace PeopleManager.IntegrationTests.Screens;

public class PeopleScreenTests : BaseTestScreen
{
    
    [Test]
    public async Task Display_ShouldShowListPeopleScreen()
    {
        // Arrange
        PeopleService.Setup(service => service.GetPeopleAsync(
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

        // Act
        // Steps:
        // 1. List People
        // 2. Main screen
        // 3. Close
        SetConsoleInput(
            1 + Environment.NewLine + // 1. List People
            4 + Environment.NewLine + // 2. Main screen
            3 + Environment.NewLine); // 3. Close
        await MainScreen.StartAsync().ConfigureAwait(false);

        // Assert
        var output = ConsoleOutput.ToString();
        output.Should().ContainAll("Loading...", "People:", "john_doe", "jane_doe");
    }

    [Test]
    public async Task Display_ShouldShowOptionsToSelectPersonScreen()
    {
        // Arrange
        PeopleService.Setup(service => service.GetPeopleAsync(
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

        // Act
        // Steps:
        // 1. List People
        // 2. Main screen
        // 3. Close
        SetConsoleInput(
            1 + Environment.NewLine + // 1. List People
            4 + Environment.NewLine + // 2. Main screen
            3 + Environment.NewLine); // 3. Close
        await MainScreen.StartAsync().ConfigureAwait(false);

        // Assert
        var output = ConsoleOutput.ToString();
        output.Should().ContainAll("Loading...", "People:", "john_doe", "jane_doe", "******** Options ********");
    }

    [Test]
    public async Task Display_ShouldShowSearchScreen()
    {
        // Arrange
        const string searchTerm = "doe";
        PeopleService.Setup(p => p.SearchPeopleAsync(
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

        // Act
        // Steps:
        // 1. Search
        // 2. Enter search term
        // 3. Main screen
        // 4. Close
        SetConsoleInput(
            2 + Environment.NewLine +  // 1. Search
            searchTerm + Environment.NewLine + // 2. Enter search term
            4 + Environment.NewLine + // 3. Main screen
            3 + Environment.NewLine); // 4. Close
        await MainScreen.StartAsync().ConfigureAwait(false);

        // Assert
        var output = ConsoleOutput.ToString();
        output.Should().ContainAll("Enter search query:", "john_doe", "jane_doe");
    }

    [Test]
    public async Task Display_ShouldShowEmptyListScreen()
    {
        // Arrange
        const string searchTerm = "not in a list";
        PeopleService.Setup(p => p.SearchPeopleAsync(
                searchTerm,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Person>
            {
                Items = [],
                CurrentPage = 1,
                PageSize = 5,
                TotalItems = 0
            });

        // Act
        // Steps:
        // 1. Search
        // 2. Enter search term
        // 3. Main screen
        // 4. Close
        SetConsoleInput(
            2 + Environment.NewLine +  // 1. Search
            searchTerm + Environment.NewLine + // 2. Enter search term
            2 + Environment.NewLine + // 3. Main screen
            3 + Environment.NewLine); // 4. Close
        await MainScreen.StartAsync().ConfigureAwait(false);

        // Assert
        var output = ConsoleOutput.ToString();
        output.Should().ContainAll("Enter search query:", "No people found.");
    }

    [Test]
    public async Task Display_ShouldShowOnlyNextPageOption_WhenUserOnFirstPage()
    {
        // Arrange
        PeopleService.Setup(service => service.GetPeopleAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Person>()
            {
                Items = TestPeople,
                CurrentPage = 1,
                PageSize = 2,
                TotalItems = 4
            });

        // Act
        // Steps:
        // 1. List People
        // 2. Main screen
        // 3. Close
        SetConsoleInput(
            1 + Environment.NewLine + // 1. List People
            5 + Environment.NewLine + // 2. Main screen
            3 + Environment.NewLine); // 3. Close
        await MainScreen.StartAsync().ConfigureAwait(false);

        // Assert
        // Assert
        var output = ConsoleOutput.ToString();
        output.Should().Contain("Next Page");
        output.Should().NotContain("Previous Page");
    }
    
    [Test]
    public async Task Display_ShouldShowOnlyPreviousPageOption_WhenUserOnLastPage()
    {
        // Arrange
        PeopleService.Setup(service => service.GetPeopleAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Person>()
            {
                Items = TestPeople,
                CurrentPage = 2,
                PageSize = 2,
                TotalItems = 4
            });
        // Act
        // Steps:
        // 1. List People
        // 2. Next Page
        // 3. Main screen
        // 4. Close
        SetConsoleInput(
            1 + Environment.NewLine + // 1. List People
            3 + Environment.NewLine + // 2. Next Page
            5 + Environment.NewLine + // 3. Main screen
            3 + Environment.NewLine); // 4. Close
        await MainScreen.StartAsync().ConfigureAwait(false);

        // Assert
        var output = ConsoleOutput.ToString();
        output.Should().Contain("Previous Page");
        output.Should().NotContain("Next Page");
    }
    
    [Test]
    public async Task Display_ShouldShowSearchAgainOption_AfterSearch()
    {
        // Arrange
        const string searchTerm = "doe";
        PeopleService.Setup(p => p.SearchPeopleAsync(
                searchTerm,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Person>
            {
                Items = TestPeople,
                CurrentPage = 1,
                PageSize = 4,
                TotalItems = 2
            });

        // Act
        // Steps:
        // 1. Search
        // 2. Enter search term
        // 3. Main screen
        // 4. Close
        SetConsoleInput(
            2 + Environment.NewLine +  // 1. Search
            searchTerm + Environment.NewLine + // 2. Enter search term
            4 + Environment.NewLine + // 3. Main screen
            3 + Environment.NewLine); // 4. Close
        await MainScreen.StartAsync().ConfigureAwait(false);

        // Assert
        var output = ConsoleOutput.ToString();
        output.Should().Contain("Search", Exactly.Thrice());
    }
}