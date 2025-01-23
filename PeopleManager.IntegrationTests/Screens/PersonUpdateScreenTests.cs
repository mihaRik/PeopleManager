using System.Reflection;
using FluentAssertions;
using Moq;
using PeopleManager.Domain.Entities;
using PeopleManager.Logic.Helpers;

namespace PeopleManager.IntegrationTests.Screens;

public class PersonUpdateScreenTests : BaseTestScreen
{
    [Test]
    public async Task Display_ShouldShowPersonPropertiesToUpdate()
    {
        // Arrange
        const string newValue = "new value";
        PeopleService.Setup(service => service.GetPeopleAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Person>
            {
                Items = TestPeople,
                CurrentPage = 1,
                PageSize = 2,
                TotalItems = 2
            });

        PeopleService.Setup(service => service.GetPersonByUsernameAsync(
                TestPeople[0].UserName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestPeople[0]);
        
        PeopleService.Setup(service => service.UpdatePersonAsync(
            TestPeople[0],
            It.IsAny<PropertyInfo>(),
            It.IsAny<object>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Person()
            {
                FirstName = newValue,
                LastName = TestPeople[0].LastName,
                UserName = TestPeople[0].UserName,
            });

        // Act
        // Steps:
        // 1. List People
        // 2. Select Person
        // 3. Update info
        // 4. Select Property
        // 5. Enter New Value
        // 6. Main Screen
        // 7. Close
        SetConsoleInput(
            1 + Environment.NewLine + // 1. List People
            1 + Environment.NewLine + // 2. Select Person
            1 + Environment.NewLine + // 3. Update info
            1 + Environment.NewLine + // 4. Select Property
            newValue + Environment.NewLine + // 5. Enter New Value
            1 + Environment.NewLine + // 6. Main Screen
            3 + Environment.NewLine); // 7. Close
        
        await MainScreen.StartAsync().ConfigureAwait(false);
        
        // Assert
        var output = ConsoleOutput.ToString();
        output.Should().ContainAll("FirstName", "LastName", "UserName");
    }
    
    [Test]
    public async Task Display_ShouldShowUpdateNotSupport_WhenPropertyIsNotUpdatable()
    {
        // Arrange
        const string newValue = "new value";
        PeopleService.Setup(service => service.GetPeopleAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Person>
            {
                Items = TestPeople,
                CurrentPage = 1,
                PageSize = 2,
                TotalItems = 2
            });

        PeopleService.Setup(service => service.GetPersonByUsernameAsync(
                TestPeople[0].UserName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestPeople[0]);
        
        PeopleService.Setup(service => service.UpdatePersonAsync(
                TestPeople[0],
                It.IsAny<PropertyInfo>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Person()
            {
                FirstName = newValue,
                LastName = TestPeople[0].LastName,
                UserName = TestPeople[0].UserName,
            });

        // Act
        // Steps:
        // 1. List People
        // 2. Select Person
        // 3. Update info
        // 4. Select Not Supported Property
        // 5. Select Supported Property
        // 6. Enter New Value
        // 7. Main Screen
        // 8. Close
        SetConsoleInput(
            1 + Environment.NewLine + // 1. List People
            1 + Environment.NewLine + // 2. Select Person
            1 + Environment.NewLine + // 3. Update info
            10 + Environment.NewLine + // 4. Select Not Supported Property
            1 + Environment.NewLine + // 5. Select Supported Property
            newValue + Environment.NewLine + // 6. Enter New Value
            1 + Environment.NewLine + // 7. Main Screen
            3 + Environment.NewLine); // 8. Close
        
        await MainScreen.StartAsync().ConfigureAwait(false);
        
        // Assert
        var output = ConsoleOutput.ToString();
        output.Should().Contain("Property update for this field is not supported.");
    }
    
    [Test]
    public async Task Display_ShouldPromptUserToEnterNewValue_WhenPropertyIsUpdatable()
    {
        // Arrange
        const string newValue = "new value";
        PeopleService.Setup(service => service.GetPeopleAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Person>
            {
                Items = TestPeople,
                CurrentPage = 1,
                PageSize = 2,
                TotalItems = 2
            });

        PeopleService.Setup(service => service.GetPersonByUsernameAsync(
                TestPeople[0].UserName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestPeople[0]);
        
        PeopleService.Setup(service => service.UpdatePersonAsync(
                TestPeople[0],
                It.IsAny<PropertyInfo>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Person()
            {
                FirstName = newValue,
                LastName = TestPeople[0].LastName,
                UserName = TestPeople[0].UserName,
            });

        // Act
        // Steps:
        // 1. List People
        // 2. Select Person
        // 3. Update info
        // 4. Select Property
        // 5. Enter New Value
        // 6. Main Screen
        // 7. Close
        SetConsoleInput(
            1 + Environment.NewLine + // 1. List People
            1 + Environment.NewLine + // 2. Select Person
            1 + Environment.NewLine + // 3. Update info
            1 + Environment.NewLine + // 4. Select Property
            newValue + Environment.NewLine + // 5. Enter New Value
            1 + Environment.NewLine + // 6. Main Screen
            3 + Environment.NewLine); // 7. Close
        
        await MainScreen.StartAsync().ConfigureAwait(false);
        
        // Assert
        var output = ConsoleOutput.ToString();
        output.Should().Contain("Enter new value for the field:");
    }
    
    [Test]
    public async Task Display_ShouldMainScreenOption_AfterUpdate()
    {
        // Arrange
        const string newValue = "new value";
        PeopleService.Setup(service => service.GetPeopleAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Person>
            {
                Items = TestPeople,
                CurrentPage = 1,
                PageSize = 2,
                TotalItems = 2
            });

        PeopleService.Setup(service => service.GetPersonByUsernameAsync(
                TestPeople[0].UserName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestPeople[0]);
        
        PeopleService.Setup(service => service.UpdatePersonAsync(
                TestPeople[0],
                It.IsAny<PropertyInfo>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Person()
            {
                FirstName = newValue,
                LastName = TestPeople[0].LastName,
                UserName = TestPeople[0].UserName,
            });

        // Act
        // Steps:
        // 1. List People
        // 2. Select Person
        // 3. Update info
        // 4. Select Property
        // 5. Enter New Value
        // 6. Main Screen
        // 7. Close
        SetConsoleInput(
            1 + Environment.NewLine + // 1. List People
            1 + Environment.NewLine + // 2. Select Person
            1 + Environment.NewLine + // 3. Update info
            1 + Environment.NewLine + // 4. Select Property
            newValue + Environment.NewLine + // 5. Enter New Value
            1 + Environment.NewLine + // 6. Main Screen
            3 + Environment.NewLine); // 7. Close
        
        await MainScreen.StartAsync().ConfigureAwait(false);
        
        // Assert
        var output = ConsoleOutput.ToString();
        output.Should().Contain("Main Screen", Exactly.Thrice());
    }
}