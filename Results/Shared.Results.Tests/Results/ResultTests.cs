namespace Shared.Results.Tests.Results;

public class ResultTests
{
    [Fact]
    public void Success_WhenCalledWithoutParameters_ShouldReturnSuccessResult()
    {
        // Arrange & Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Error_WhenCalledWithSingleError_ShouldReturnErrorResult()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test description");

        // Act
        var result = Result.Error(error);

        // Assert
        Assert.IsType<ErrorResult>(result);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public void Error_WhenCalledWithMultipleErrors_ShouldReturnErrorResult()
    {
        // Arrange
        var error1 = new Error("ERROR1", "Description 1");
        var error2 = new Error("ERROR2", "Description 2");
        var errors = new[] { error1, error2 };

        // Act
        var result = Result.Error(errors);

        // Assert
        Assert.IsType<ErrorResult>(result);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(2, result.Errors.Length);
    }

    [Fact]
    public void NotFound_WhenCalledWithError_ShouldReturnNotFoundResult()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = Result.NotFound(error);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public void Success_WhenCalledWithResponse_ShouldReturnGenericSuccessResult()
    {
        // Arrange
        var response = "test response";

        // Act
        var result = Result.Success(response);

        // Assert
        Assert.IsType<Result<string>>(result);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void IsSuccess_WhenResultIsSuccess_ShouldReturnTrue()
    {
        // Arrange & Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void IsFailure_WhenResultIsError_ShouldReturnTrue()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test description");

        // Act
        var result = Result.Error(error);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void IsFailure_WhenResultIsSuccess_ShouldReturnFalse()
    {
        // Arrange & Act
        var result = Result.Success();

        // Assert
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void IsSuccess_WhenResultIsError_ShouldReturnFalse()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test description");

        // Act
        var result = Result.Error(error);

        // Assert
        Assert.False(result.IsSuccess);
    }
}

public class GenericResultTests
{
    [Fact]
    public void Success_WhenCalledWithData_ShouldReturnSuccessResult()
    {
        // Arrange
        var data = "test data";

        // Act
        var result = Result<string>.Success(data);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
        Assert.Equal(data, result.Data);
    }

    [Fact]
    public void Error_WhenCalledWithSingleError_ShouldReturnGenericErrorResult()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test description");

        // Act
        var result = Result<string>.Error(error);

        // Assert
        Assert.IsType<ErrorResult<string>>(result);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public void Error_WhenCalledWithMultipleErrors_ShouldReturnGenericErrorResult()
    {
        // Arrange
        var error1 = new Error("ERROR1", "Description 1");
        var error2 = new Error("ERROR2", "Description 2");
        var errors = new[] { error1, error2 };

        // Act
        var result = Result<string>.Error(errors);

        // Assert
        Assert.IsType<ErrorResult<string>>(result);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(2, result.Errors.Length);
    }

    [Fact]
    public void NotFound_WhenCalledWithError_ShouldReturnGenericNotFoundResult()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = Result<string>.NotFound(error);

        // Assert
        Assert.IsType<NotFoundResult<string>>(result);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public void Success_WhenCalledWithNullData_ShouldReturnSuccessResultWithNullData()
    {
        // Arrange & Act
        var result = Result<string?>.Success(null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
        Assert.Null(result.Data);
    }

    [Fact]
    public void Data_WhenResultIsSuccess_ShouldReturnData()
    {
        // Arrange
        var expectedData = "test data";

        // Act
        var result = Result<string>.Success(expectedData);

        // Assert
        Assert.Equal(expectedData, result.Data);
    }

    [Fact]
    public void Data_WhenResultIsError_ShouldReturnDefault()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test description");

        // Act
        var result = Result<string>.Error(error);

        // Assert
        Assert.Null(result.Data);
    }

    [Fact]
    public void IsSuccess_WhenResultHasData_ShouldReturnTrue()
    {
        // Arrange
        var data = "test data";

        // Act
        var result = Result<string>.Success(data);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void IsFailure_WhenResultHasError_ShouldReturnTrue()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test description");

        // Act
        var result = Result<string>.Error(error);

        // Assert
        Assert.True(result.IsFailure);
    }
}
