using FluentAssertions;
using UriBuilder = PeopleManager.Repository.Utils.UriBuilder;

namespace PeopleManager.Repository.Tests.Helpers;

public class UriBuilderTests
    {
        [Test]
        public void Constructor_WithSegment_ShouldInitializeBaseUri()
        {
            // Arrange
            const string segment = "https://example.com/api";

            // Act
            var uriBuilder = new UriBuilder(segment);
            var result = uriBuilder.Build();

            // Assert
            result.Should().Be(segment);
        }

        [Test]
        public void WithPagination_AddsPaginationQueryParams()
        {
            // Arrange
            var uriBuilder = new UriBuilder("https://example.com/api");

            // Act
            var result = uriBuilder.WithPagination(2, 10).Build();

            // Assert
            result.Should().Be("https://example.com/api?&$top=10&$skip=10");
        }

        [Test]
        public void WithPagination_HandlesFirstPageWithoutOffset()
        {
            // Arrange
            var uriBuilder = new UriBuilder("https://example.com/api");

            // Act
            var result = uriBuilder.WithPagination(1, 20).Build();

            // Assert
            result.Should().Be("https://example.com/api?&$top=20&$skip=0");
        }

        [Test]
        public void WithSelect_AddsSelectQueryParams()
        {
            // Arrange
            var uriBuilder = new UriBuilder("https://example.com/api");

            // Act
            var result = uriBuilder.WithSelect("Name", "Age").Build();

            // Assert
            result.Should().Be("https://example.com/api?&$select=Name,Age");
        }

        [Test]
        public void WithCount_AppendsCountSegment()
        {
            // Arrange
            var uriBuilder = new UriBuilder("https://example.com/api");

            // Act
            var result = uriBuilder.WithCount().Build();

            // Assert
            result.Should().Be("https://example.com/api/$count");
        }

        [Test]
        public void WithFilterById_AddsIdFilter()
        {
            // Arrange
            var uriBuilder = new UriBuilder("https://example.com/api");

            // Act
            var result = uriBuilder.WithFilterById("1234").Build();

            // Assert
            result.Should().Be("https://example.com/api('1234')");
        }

        [Test]
        public void WithFilterByProperties_AddsFilterQueryParams()
        {
            // Arrange
            var uriBuilder = new UriBuilder("https://example.com/api");

            // Act
            var result = uriBuilder.WithFilterByProperties("searchTerm", "Name", "Description").Build();

            // Assert
            result.Should().Be("https://example.com/api?&$filter=contains(Name, 'searchTerm') or contains(Description, 'searchTerm')");
        }

        [Test]
        public void WithFilterByProperties_AddsQuestionMarkOnce()
        {
            // Arrange
            var uriBuilder = new UriBuilder("https://example.com/api");

            // Act
            var result = uriBuilder
                .WithFilterByProperties("searchTerm", "Name")
                .WithPagination(1, 10)
                .Build();

            // Assert
            result.Should().Be("https://example.com/api?&$filter=contains(Name, 'searchTerm')&$top=10&$skip=0");
        }

        [Test]
        public void WithPagination_FollowsQuestionMarkUsageBehavior()
        {
            // Arrange
            var uriBuilder = new UriBuilder("https://example.com/api");

            // Act
            var result = uriBuilder
                .WithPagination(1, 10)
                .WithSelect("Name", "Email")
                .Build();

            // Assert
            result.Should().Be("https://example.com/api?&$top=10&$skip=0&$select=Name,Email");
        }

        [Test]
        public void Build_ReturnsFinalUri()
        {
            // Arrange
            var uriBuilder = new UriBuilder("https://example.com/api");

            // Act
            var result = uriBuilder
                .WithPagination(2, 5)
                .WithSelect("Id", "Name")
                .Build();

            // Assert
            result.Should().Be("https://example.com/api?&$top=5&$skip=5&$select=Id,Name");
        }
    }