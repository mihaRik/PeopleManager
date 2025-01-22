using System.Net;
using System.Net.Http.Json;
using Moq;
using FluentAssertions;
using PeopleManager.Domain.Entities;
using PeopleManager.Repository.Client;

namespace PeopleManager.Repository.UnitTests;

public class PeopleRepositoryTests
{
    private Mock<IODataHttpClient> _mockHttpClient;
    private IPeopleRepository _repository;

    [SetUp]
    public void Setup()
    {
        _mockHttpClient = new Mock<IODataHttpClient>();
        _repository = new PeopleRepository(_mockHttpClient.Object);
    }

    [Test]
    public async Task GetPeopleAsync_ShouldReturnPeople_WhenResponseIsSuccessful()
    {
        // Arrange
        var expectedPeople = new[]
        {
            new Person { UserName = "user1", FirstName = "John", LastName = "Doe" },
            new Person { UserName = "user2", FirstName = "Jane", LastName = "Doe" }
        };

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(new { Value = expectedPeople })
        };

        _mockHttpClient
            .Setup(client => client.MakeRequestAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _repository.GetPeopleAsync(1, 10, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedPeople);
    }

    [Test]
    public async Task GetPeopleAsync_ShouldReturnEmpty_WhenResponseIsEmpty()
    {
        // Arrange
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(new { Value = Array.Empty<Person>() })
        };

        _mockHttpClient
            .Setup(client => client.MakeRequestAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _repository.GetPeopleAsync(1, 10, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetPeopleCountAsync_ShouldReturnCount_WhenResponseIsSuccessful()
    {
        // Arrange
        const int expectedCount = 42;

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(expectedCount.ToString())
        };

        _mockHttpClient
            .Setup(client => client.MakeRequestAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _repository.GetPeopleCountAsync("John", CancellationToken.None);

        // Assert
        result.Should().Be(expectedCount);
    }

    [Test]
    public async Task GetPeopleCountAsync_ShouldReturnZero_WhenResponseIsInvalid()
    {
        // Arrange
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("invalid_number")
        };

        _mockHttpClient
            .Setup(client => client.MakeRequestAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _repository.GetPeopleCountAsync("John", CancellationToken.None);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public async Task GetPersonByUsernameAsync_ShouldReturnPerson_WhenResponseIsSuccessful()
    {
        // Arrange
        var expectedPerson = new Person { UserName = "user1", FirstName = "John", LastName = "Doe" };

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedPerson)
        };

        _mockHttpClient
            .Setup(client => client.MakeRequestAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _repository.GetPersonByUsernameAsync("user1", CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedPerson);
    }

    [Test]
    public async Task SearchPeopleAsync_ShouldReturnPeople_WhenResponseIsSuccessful()
    {
        // Arrange
        var expectedPeople = new[]
        {
            new Person { UserName = "user1", FirstName = "John", LastName = "Doe" },
            new Person { UserName = "user2", FirstName = "Jane", LastName = "Doe" }
        };

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(new { Value = expectedPeople })
        };

        _mockHttpClient
            .Setup(client => client.MakeRequestAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _repository.SearchPeopleAsync("John", 1, 10, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedPeople);
    }

    [Test]
    public async Task SearchPeopleAsync_ShouldReturnEmpty_WhenResponseIsEmpty()
    {
        // Arrange
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(new { Value = Array.Empty<Person>() })
        };

        _mockHttpClient
            .Setup(client => client.MakeRequestAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _repository.SearchPeopleAsync("John", 1, 10, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task UpdatePersonAsync_ShouldReturnUpdatedPerson_WhenResponseIsSuccessful()
    {
        // Arrange
        var updatedPerson = new Person { UserName = "user1", FirstName = "John", LastName = "Smith" };

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(updatedPerson)
        };

        _mockHttpClient
            .Setup(client => client.MakeRequestAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _repository.UpdatePersonAsync("user1", updatedPerson, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(updatedPerson);
    }
}