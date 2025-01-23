using FluentAssertions;

namespace PeopleManager.IntegrationTests.Screens;

public class MainScreenTests : BaseTestScreen
{
    [Test]
    public async Task Display_ShouldShowMainScreen()
    {
        // Arrange
        // Act
        SetConsoleInput(3 + Environment.NewLine);
        await MainScreen.StartAsync().ConfigureAwait(false);

        // Assert
        var output = ConsoleOutput.ToString();
        output.Should().ContainAll(
            "Welcome to People Manager",
            "List People",
            "Search",
            "Close",
            "Please enter a number:");
    }
}