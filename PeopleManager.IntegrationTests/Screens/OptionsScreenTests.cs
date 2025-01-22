using FluentAssertions;
using PeopleManager.Screens;

namespace PeopleManager.IntegrationTests.Screens;

public class OptionsScreenTests
    {
        [Test]
        public async Task DisplayOptionsAsync_ShouldDisplayAllOptionsAndExecuteSelectedAction()
        {
            // Arrange
            var options = new Dictionary<int, Option>
            {
                {
                    1, new Option
                    {
                        Name = "Option 1",
                        Action = (_, _) =>
                        {
                            Console.WriteLine("Option 1 executed.");
                            return Task.CompletedTask;
                        },
                        ActionParams = []
                    }
                }
            };

            var input = new StringReader("1");
            Console.SetIn(input);

            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            await OptionsScreen.DisplayOptionsAsync(options, CancellationToken.None);

            // Assert
            var outputText = output.ToString();
            outputText.Should().Contain("Option 1 executed.");
            outputText.Should().Contain("******** Options ********");
            outputText.Should().Contain("1. Option 1");
            outputText.Should().Contain("Please enter a number:");
        }

        [Test]
        public async Task GetValidOptionNumber_ShouldPromptUntilValidInputIsProvided()
        {
            // Arrange
            var options = new Dictionary<int, Option>
            {
                { 1, new Option { Name = "Option 1" } },
                { 2, new Option
                {
                    Name = "Option 2",
                    Action = (_, _) =>
                    {
                        Console.WriteLine("Option 2 executed.");
                        return Task.CompletedTask;
                    },
                } }
            };

            var input = new StringReader("invalid" + Environment.NewLine + 3 + Environment.NewLine + 2 + Environment.NewLine);
            Console.SetIn(input);

            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            await OptionsScreen.DisplayOptionsAsync(options, CancellationToken.None);

            // Assert
            var outputText = output.ToString();
            outputText.Should().Contain("Option 2 executed.");
            outputText.Should().Contain("Please enter a number:");
            outputText.Should().Contain("Please enter a valid number.");
            outputText.Should().Contain("Please enter a valid option number (1, 2).");
        }
    }