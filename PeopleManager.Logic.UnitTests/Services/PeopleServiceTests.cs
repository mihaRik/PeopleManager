using FluentAssertions;
using Moq;
using PeopleManager.Domain.Entities;
using PeopleManager.Logic.Services;
using PeopleManager.Repository;

namespace PeopleManager.Logic.UnitTests.Services;

public class PeopleServiceTests
    {
        private Mock<IPeopleRepository> _repositoryMock;
        private IPeopleService _peopleService;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IPeopleRepository>();
            _peopleService = new PeopleService(_repositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _peopleService.Dispose();
        }

        [Test]
        public async Task GetPeopleAsync_ShouldReturnPagedResult()
        {
            // Arrange
            var people = new List<Person> { new(), new() };
            const int page = 1;
            const int pageSize = 2;
            const int totalCount = 5;
            var cancellationToken = CancellationToken.None;

            _repositoryMock.Setup(repo => repo.GetPeopleAsync(page, pageSize, cancellationToken))
                .ReturnsAsync(people);
            
            _repositoryMock.Setup(repo => repo.GetPeopleCountAsync(null!, cancellationToken))
                .ReturnsAsync(totalCount);

            // Act
            var result = await _peopleService.GetPeopleAsync(page, pageSize, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEquivalentTo(people);
            result.CurrentPage.Should().Be(page);
            result.PageSize.Should().Be(pageSize);
            result.TotalItems.Should().Be(totalCount);

            _repositoryMock.Verify(repo => repo.GetPeopleAsync(page, pageSize, cancellationToken), Times.Once);
            _repositoryMock.Verify(repo => repo.GetPeopleCountAsync(null!, cancellationToken), Times.Once);
        }

        [Test]
        public async Task GetPersonByUsernameAsync_ShouldReturnPerson_WhenPersonExists()
        {
            // Arrange
            const string username = "john_doe";
            var person = new Person();
            var cancellationToken = CancellationToken.None;

            _repositoryMock.Setup(repo => repo.GetPersonByUsernameAsync(username, cancellationToken))
                .ReturnsAsync(person);

            // Act
            var result = await _peopleService.GetPersonByUsernameAsync(username, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(person);

            _repositoryMock.Verify(repo => repo.GetPersonByUsernameAsync(username, cancellationToken), Times.Once);
        }

        [Test]
        public async Task GetPersonByUsernameAsync_ShouldThrowKeyNotFoundException_WhenPersonDoesNotExist()
        {
            // Arrange
            var username = "nonexistent_user";
            var cancellationToken = CancellationToken.None;

            _repositoryMock.Setup(repo => repo.GetPersonByUsernameAsync(username, cancellationToken))
                .ReturnsAsync((Person)null!);

            // Act
            var act = async () => await _peopleService.GetPersonByUsernameAsync(username, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Person with username '{username}' not found.");

            _repositoryMock.Verify(repo => repo.GetPersonByUsernameAsync(username, cancellationToken), Times.Once);
        }

        [Test]
        public async Task SearchPeopleAsync_ShouldReturnPagedResult()
        {
            // Arrange
            var searchQuery = "John";
            var page = 1;
            var pageSize = 3;
            var cancellationToken = CancellationToken.None;

            var people = new List<Person> { new Person(), new Person(), new Person() };
            var totalCount = 10;

            _repositoryMock.Setup(repo => repo.SearchPeopleAsync(searchQuery, page, pageSize, cancellationToken))
                .ReturnsAsync(people);
            _repositoryMock.Setup(repo => repo.GetPeopleCountAsync(searchQuery, cancellationToken))
                .ReturnsAsync(totalCount);

            // Act
            var result = await _peopleService.SearchPeopleAsync(searchQuery, page, pageSize, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEquivalentTo(people);
            result.CurrentPage.Should().Be(page);
            result.PageSize.Should().Be(pageSize);
            result.TotalItems.Should().Be(totalCount);

            _repositoryMock.Verify(repo => repo.SearchPeopleAsync(searchQuery, page, pageSize, cancellationToken), Times.Once);
            _repositoryMock.Verify(repo => repo.GetPeopleCountAsync(searchQuery, cancellationToken), Times.Once);
        }

        [Test]
        public async Task UpdatePersonAsync_ShouldUpdateAndReturnUpdatedPerson()
        {
            // Arrange
            var propertyToUpdate = typeof(Person).GetProperty("LastName")!;
            var person = new Person { UserName = "john_doe" };
            const string newValue = "Doe Updated";
            var cancellationToken = CancellationToken.None;

            _repositoryMock.Setup(repo => repo.UpdatePersonAsync(person.UserName, person, cancellationToken))
                .ReturnsAsync(person);

            // Act
            propertyToUpdate.SetValue(person, newValue);
            var result = await _peopleService.UpdatePersonAsync(person, propertyToUpdate, newValue, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            propertyToUpdate.GetValue(result).Should().BeEquivalentTo(newValue);

            _repositoryMock.Verify(repo => repo.UpdatePersonAsync(person.UserName, person, cancellationToken), Times.Once);
        }
    }