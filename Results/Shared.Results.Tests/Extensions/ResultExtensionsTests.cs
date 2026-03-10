using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Results.Extensions;

namespace Shared.Results.Tests.Extensions;

public class ResultExtensionsTests
{
    [Fact]
    public void Ok_WithSuccessResult_ReturnsOkObjectResult()
    {
        // Arrange
        var data = "test data";
        var result = Result<string>.Success(data);

        // Act
        var httpResult = result.Ok();

        // Assert
        Assert.IsType<Ok<string>>(httpResult);
        var okResult = (Ok<string>)httpResult;
        Assert.Equal(data, okResult.Value);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public void Ok_WithErrorResult_ReturnsBadRequestObjectResult()
    {
        // Arrange
        var error = new Error("VALIDATION_ERROR", "Test error message");
        var result = Result<string>.Error(error);

        // Act
        var httpResult = result.Ok();

        // Assert
        Assert.IsType<BadRequest<Error[]>>(httpResult);
        var badRequestResult = (BadRequest<Error[]>)httpResult;
        Assert.NotNull(badRequestResult.Value);
        Assert.Single(badRequestResult.Value);
        Assert.Equal(error, badRequestResult.Value[0]);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public void Ok_WithNotFoundResult_ReturnsNotFoundObjectResult()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");
        var result = Result<string>.NotFound(error);

        // Act
        var httpResult = result.Ok();

        // Assert
        Assert.IsType<NotFound<Error[]>>(httpResult);
        var notFoundResult = (NotFound<Error[]>)httpResult;
        Assert.NotNull(notFoundResult.Value);
        Assert.Single(notFoundResult.Value);
        Assert.Equal(error, notFoundResult.Value[0]);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public void NoContent_WithSuccessResult_ReturnsNoContentResult()
    {
        // Arrange
        var result = Result.Success();

        // Act
        var httpResult = result.NoContent();

        // Assert
        Assert.IsType<NoContent>(httpResult);
        var noContentResult = (NoContent)httpResult;
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public void NoContent_WithErrorResult_ReturnsBadRequestObjectResult()
    {
        // Arrange
        var error = new Error("VALIDATION_ERROR", "Test error message");
        var result = Result.Error(error);

        // Act
        var httpResult = result.NoContent();

        // Assert
        Assert.IsType<BadRequest<Error[]>>(httpResult);
        var badRequestResult = (BadRequest<Error[]>)httpResult;
        Assert.NotNull(badRequestResult.Value);
        Assert.Single(badRequestResult.Value);
        Assert.Equal(error, badRequestResult.Value[0]);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
    }

    [Fact]
    public void Created_WithSuccessResult_ReturnsCreatedAtRouteResult()
    {
        // Arrange
        var data = "test data";
        var result = Result<string>.Success(data);
        var uri = "/api/test/1";

        // Act
        var httpResult = result.Created(uri);

        // Assert
        Assert.IsType<Created<string>>(httpResult);
        var createdResult = (Created<string>)httpResult;
        Assert.Equal(data, createdResult.Value);
        Assert.Equal(uri, createdResult.Location);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
    }

    [Fact]
    public void Created_WithoutUri_ReturnsCreatedResult()
    {
        // Arrange
        var data = "test data";
        var result = Result<string>.Success(data);

        // Act
        var httpResult = result.Created();

        // Assert
        Assert.IsType<Created<string>>(httpResult);
        var createdResult = (Created<string>)httpResult;
        Assert.Equal(data, createdResult.Value);
        Assert.Null(createdResult.Location);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
    }
}
