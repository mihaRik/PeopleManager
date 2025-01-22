using System.Net;
using FluentAssertions;
using Moq;
using Moq.Protected;
using PeopleManager.Repository.Client;

namespace PeopleManager.Repository.Tests.Client;

public class ODataHttpClientTests
{
    private Mock<HttpMessageHandler> _mockMessageHandler;
    private HttpClient _mockHttpClient;
    private IODataHttpClient _oDataHttpClient;

    [SetUp]
    public void Setup()
    {
        _mockMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpClient = new HttpClient(_mockMessageHandler.Object);
        _oDataHttpClient = new ODataHttpClient(_mockHttpClient);
    }

    [TearDown]
    public void TearDown()
    {
        _mockHttpClient.Dispose();
    }

    [Test]
    public async Task MakeRequestAsync_ShouldReturnResponse_WhenRequestIsSuccessful()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "http://example.com");
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Success")
        };

        _mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(expectedResponse);

        // Act
        var response = await _oDataHttpClient.MakeRequestAsync(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.ReadAsStringAsync().Result.Should().Be("Success");

        _mockMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req == request),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public async Task MakeRequestAsync_ShouldThrowException_WhenHttpClientThrowsException()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "http://example.com");

        _mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Request failed"));

        // Act
        Func<Task> act = async () => await _oDataHttpClient.MakeRequestAsync(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>().WithMessage("Request failed");

        _mockMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req == request),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public async Task MakeRequestAsync_ShouldRespectCancellationToken_WhenCancelled()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "http://example.com");
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        _mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new TaskCanceledException("Task was cancelled"));

        // Act
        Func<Task> act = async () => await _oDataHttpClient.MakeRequestAsync(request, cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<TaskCanceledException>().WithMessage("Task was cancelled");

        _mockMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req == request),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Test]
    public async Task MakeRequestAsync_ShouldReturnErrorResponse_WhenServerReturnsError()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "http://example.com");
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Server Error")
        };

        _mockMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(expectedResponse);

        // Act
        var response = await _oDataHttpClient.MakeRequestAsync(request, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Content.ReadAsStringAsync().Result.Should().Be("Server Error");

        _mockMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req == request),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}