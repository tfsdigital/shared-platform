using FluentValidation;

namespace Shared.Validations.Tests;

public class CommandValidatorTests
{
    private class TestCommand
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private class TestCommandValidator : AbstractValidator<TestCommand>
    {
        public TestCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Age).GreaterThan(0).WithMessage("Age must be greater than 0");
        }
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var commandValidator = new CommandValidator<TestCommand>(validator);
        var command = new TestCommand { Name = "Test", Age = 25 };

        // Act
        var result = commandValidator.Validate(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_InvalidCommand_ReturnsErrors()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var commandValidator = new CommandValidator<TestCommand>(validator);
        var command = new TestCommand { Name = "", Age = -1 };

        // Act
        var result = commandValidator.Validate(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(2, result.Errors.Length);
        Assert.Contains(result.Errors, e => e.Code == "Name" && e.Description == "Name is required");
        Assert.Contains(result.Errors, e => e.Code == "Age" && e.Description == "Age must be greater than 0");
    }

    [Fact]
    public void Validate_PartiallyValidCommand_ReturnsSpecificErrors()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var commandValidator = new CommandValidator<TestCommand>(validator);
        var command = new TestCommand { Name = "Test", Age = -1 };

        // Act
        var result = commandValidator.Validate(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("Age", result.Errors[0].Code);
        Assert.Equal("Age must be greater than 0", result.Errors[0].Description);
    }

    [Fact]
    public void Validate_NullCommand_ThrowsInvalidOperationException()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var commandValidator = new CommandValidator<TestCommand>(validator);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => commandValidator.Validate(null!));
    }

    [Fact]
    public void Validate_CommandWithMultipleErrorsOnSameProperty_ReturnsAllErrors()
    {
        // Arrange
        var validator = new TestCommandWithMultipleRulesValidator();
        var commandValidator = new CommandValidator<TestCommand>(validator);
        var command = new TestCommand { Name = "", Age = 25 };
        var collection = new[] { "Name is required", "Name must be at least 2 characters" };

        // Act
        var result = commandValidator.Validate(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.Errors.Length >= 2);
        Assert.All(result.Errors.Where(e => e.Code == "Name"), error =>
            Assert.Contains(error.Description, collection));
    }

    [Fact]
    public void Constructor_WithNullValidator_DoesNotThrowImmediately()
    {
        // Arrange & Act
        var commandValidator = new CommandValidator<TestCommand>(null!);

        // Assert
        Assert.NotNull(commandValidator);

        // But should throw when trying to validate
        var command = new TestCommand { Name = "Test", Age = 25 };
        Assert.Throws<NullReferenceException>(() => commandValidator.Validate(command));
    }

    private class TestCommandWithMultipleRulesValidator : AbstractValidator<TestCommand>
    {
        public TestCommandWithMultipleRulesValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters");
            RuleFor(x => x.Age).GreaterThan(0).WithMessage("Age must be greater than 0");
        }
    }
}
