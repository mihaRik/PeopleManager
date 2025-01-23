using FluentAssertions;
using Moq;
using PeopleManager.Domain.Entities;
using PeopleManager.Logic.Helpers;

namespace PeopleManager.IntegrationTests.Screens;

public class PersonScreenTests : BaseTestScreen
{
    [Test]
    public async Task Display_ShouldShowPersonInformationScreen()
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

        PeopleService.Setup(service =>
                service.GetPersonByUsernameAsync(TestPeople[0].UserName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestPeople[0]);

        // Act
        // Steps:
        // 1. List People
        // 2. Select a person
        // 3. Main screen
        // 4. Close
        SetConsoleInput(
            1 + Environment.NewLine + // 1. List People
            1 + Environment.NewLine + // 2. Select a person
            2 + Environment.NewLine + // 3. Main screen
            3 + Environment.NewLine); // 4. Close
        await MainScreen.StartAsync().ConfigureAwait(false);

        // Assert
        var output = ConsoleOutput.ToString();
        output.Should().ContainAll(
            $"Detailed information about {TestPeople[0].FullName} ({TestPeople[0].UserName})",
            "Gender",
            "Age",
            "N/A");
    }
}