namespace Shared.Results.Tests.Results;

public class ErrorResultTests
{
    [Fact]
    public void Constructor_WhenCalledWithSingleError_ShouldSetError()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test error message");

        // Act
        var result = new ErrorResult(error);

        // Assert
        Assert.Single(result.Errors);
        Assert.Contains(error, result.Errors);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Constructor_WhenCalledWithMultipleErrors_ShouldSetAllErrors()
    {
        // Arrange
        var error1 = new Error("ERROR1", "First error");
        var error2 = new Error("ERROR2", "Second error");
        var errors = new[] { error1, error2 };

        // Act
        var result = new ErrorResult(errors);

        // Assert
        Assert.Equal(2, result.Errors.Length);
        Assert.Contains(error1, result.Errors);
        Assert.Contains(error2, result.Errors);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void ErrorResult_ShouldInheritFromResult()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test message");

        // Act
        var result = new ErrorResult(error);

        // Assert
        Assert.IsAssignableFrom<Result>(result);
    }
}

public class GenericErrorResultTests
{
    [Fact]
    public void Constructor_WhenCalledWithSingleError_ShouldSetError()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test error message");

        // Act
        var result = new ErrorResult<string>(error);

        // Assert
        Assert.Single(result.Errors);
        Assert.Contains(error, result.Errors);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Null(result.Data);
    }

    [Fact]
    public void Constructor_WhenCalledWithMultipleErrors_ShouldSetAllErrors()
    {
        // Arrange
        var error1 = new Error("ERROR1", "First error");
        var error2 = new Error("ERROR2", "Second error");
        var errors = new[] { error1, error2 };

        // Act
        var result = new ErrorResult<int>(errors);

        // Assert
        Assert.Equal(2, result.Errors.Length);
        Assert.Contains(error1, result.Errors);
        Assert.Contains(error2, result.Errors);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(0, result.Data);
    }

    [Fact]
    public void GenericErrorResult_ShouldInheritFromGenericResult()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test message");

        // Act
        var result = new ErrorResult<string>(error);

        // Assert
        Assert.IsAssignableFrom<Result<string>>(result);
    }

    [Fact]
    public void Constructor_WhenCalledWithReferenceType_ShouldHaveNullData()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test error message");

        // Act
        var result = new ErrorResult<string>(error);

        // Assert
        Assert.Null(result.Data);
    }

    [Fact]
    public void Constructor_WhenCalledWithValueType_ShouldHaveDefaultData()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test error message");

        // Act
        var result = new ErrorResult<int>(error);

        // Assert
        Assert.Equal(0, result.Data);
    }
}
