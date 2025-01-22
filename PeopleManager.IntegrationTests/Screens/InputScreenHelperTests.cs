using FluentAssertions;
using PeopleManager.Screens;

namespace PeopleManager.IntegrationTests.Screens;

public class InputScreenHelperTests
{
    private StringReader _consoleInput;
    private StringWriter _consoleOutput;

    [SetUp]
    public void SetUp()
    {
        _consoleOutput = new StringWriter();
        Console.SetOut(_consoleOutput);
    }

    [TearDown]
    public void TearDown()
    {
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
    public void GetValidTextInput_ValidInput_ReturnsInput()
    {
        // Arrange
        const string prompt = "Enter your name";
        const string userInput = "John Doe";
        SetConsoleInput(userInput + Environment.NewLine);

        // Act
        var result = InputScreenHelper.GetValidTextInput(prompt);

        // Assert
        result.Should().Be(userInput);
        _consoleOutput.ToString().Should().Contain(prompt);
    }

    [Test]
    public void GetValidTextInput_EmptyInput_PromptsUntilValid()
    {
        // Arrange
        const string prompt = "Enter your name";
        const string userInput = "John Doe";
        SetConsoleInput(Environment.NewLine + Environment.NewLine + userInput + Environment.NewLine);

        // Act
        var result = InputScreenHelper.GetValidTextInput(prompt);

        // Assert
        result.Should().Be(userInput);
        var output = _consoleOutput.ToString();
        output.Should().Contain("Input cannot be empty. Please try again.");
        output.Should().Contain(prompt);
    }

    [Test]
    public void GetValidNumericInput_ValidNumber_ReturnsParsedInteger()
    {
        // Arrange
        const string validInput = "42";
        SetConsoleInput(validInput + Environment.NewLine);

        // Act
        var result = InputScreenHelper.GetValidNumericInput();

        // Assert
        result.Should().Be(42);
        _consoleOutput.ToString().Should().Contain("Enter number");
    }

    [Test]
    public void GetValidNumericInput_InvalidNumber_PromptsUntilValid()
    {
        // Arrange
        SetConsoleInput("abc" + Environment.NewLine + "" + Environment.NewLine + "42" + Environment.NewLine);

        // Act
        var result = InputScreenHelper.GetValidNumericInput();

        // Assert
        result.Should().Be(42);
        var output = _consoleOutput.ToString();
        output.Should().Contain("Please enter a number.");
        output.Should().Contain("Enter number");
    }
}