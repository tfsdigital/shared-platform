using Shared.Results;

namespace Shared.Handlers.Tests;

public class HandlerTests
{
    private class TestGenericHandler : Handler<string>
    {
        public static Result<string> TestNotFound(Error error) => NotFound(error);
        public static Result<string> TestError(Error error) => Error(error);
        public static Result<string> TestError(Error[] errors) => Error(errors);
        public static Result<string> TestSuccess(string response) => Success(response);
    }

    private class TestHandler : Handler
    {
        public static Result TestStaticNotFound(Error error) => NotFound(error);
        public static Result TestStaticError(Error error) => Error(error);
        public static Result TestStaticError(Error[] errors) => Error(errors);
        public static Result TestStaticSuccess() => Success();
        public static Result TestStaticSuccessWithResponse<T>(T response) => Success(response);
    }

    [Fact]
    public void GenericHandler_NotFound_WhenCalledWithError_ShouldReturnNotFoundResult()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = TestGenericHandler.TestNotFound(error);

        // Assert
        Assert.IsType<NotFoundResult<string>>(result);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public void GenericHandler_Error_WhenCalledWithSingleError_ShouldReturnErrorResult()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test error message");

        // Act
        var result = TestGenericHandler.TestError(error);

        // Assert
        Assert.IsType<ErrorResult<string>>(result);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public void GenericHandler_Error_WhenCalledWithMultipleErrors_ShouldReturnErrorResult()
    {
        // Arrange
        var error1 = new Error("ERROR1", "First error");
        var error2 = new Error("ERROR2", "Second error");
        var errors = new[] { error1, error2 };

        // Act
        var result = TestGenericHandler.TestError(errors);

        // Assert
        Assert.IsType<ErrorResult<string>>(result);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(2, result.Errors.Length);
        Assert.Contains(error1, result.Errors);
        Assert.Contains(error2, result.Errors);
    }

    [Fact]
    public void GenericHandler_Success_WhenCalledWithResponse_ShouldReturnSuccessResult()
    {
        // Arrange
        var response = "test response";

        // Act
        var result = TestGenericHandler.TestSuccess(response);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
        Assert.Equal(response, result.Data);
    }

    [Fact]
    public void StaticHandler_NotFound_WhenCalledWithError_ShouldReturnNotFoundResult()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = TestHandler.TestStaticNotFound(error);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public void StaticHandler_Error_WhenCalledWithSingleError_ShouldReturnErrorResult()
    {
        // Arrange
        var error = new Error("TEST_ERROR", "Test error message");

        // Act
        var result = TestHandler.TestStaticError(error);

        // Assert
        Assert.IsType<ErrorResult>(result);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(error, result.Errors);
    }

    [Fact]
    public void StaticHandler_Error_WhenCalledWithMultipleErrors_ShouldReturnErrorResult()
    {
        // Arrange
        var error1 = new Error("ERROR1", "First error");
        var error2 = new Error("ERROR2", "Second error");
        var errors = new[] { error1, error2 };

        // Act
        var result = TestHandler.TestStaticError(errors);

        // Assert
        Assert.IsType<ErrorResult>(result);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(2, result.Errors.Length);
        Assert.Contains(error1, result.Errors);
        Assert.Contains(error2, result.Errors);
    }

    [Fact]
    public void StaticHandler_Success_WhenCalledWithoutParameters_ShouldReturnSuccessResult()
    {
        // Arrange & Act
        var result = TestHandler.TestStaticSuccess();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void StaticHandler_Success_WhenCalledWithResponse_ShouldReturnGenericSuccessResult()
    {
        // Arrange
        var response = "test response";

        // Act
        var result = TestHandler.TestStaticSuccessWithResponse(response);

        // Assert
        Assert.IsType<Result<string>>(result);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void GenericHandler_Success_WhenCalledWithNullResponse_ShouldReturnSuccessWithNull()
    {
        // Arrange & Act
        var result = TestGenericHandler.TestSuccess(null!);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
        Assert.Null(result.Data);
    }

    [Fact]
    public void StaticHandler_Success_WhenCalledWithNullResponse_ShouldReturnGenericSuccessResult()
    {
        // Arrange & Act
        var result = TestHandler.TestStaticSuccessWithResponse<string?>(null);

        // Assert
        Assert.IsType<Result<string?>>(result);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void GenericHandler_ShouldInheritFromBaseHandler()
    {
        // Arrange & Act
        var handler = new TestGenericHandler();

        // Assert
        Assert.IsAssignableFrom<Handler<string>>(handler);
    }

    [Fact]
    public void StaticHandler_ShouldInheritFromBaseHandler()
    {
        // Arrange & Act
        var handler = new TestHandler();

        // Assert
        Assert.IsAssignableFrom<Handler>(handler);
    }
}
