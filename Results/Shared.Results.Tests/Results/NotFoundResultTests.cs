namespace Shared.Results.Tests.Results;

public class NotFoundResultTests
{
    [Fact]
    public void Constructor_WhenCalledWithError_ShouldSetError()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = new NotFoundResult(error);

        // Assert
        Assert.Single(result.Errors);
        Assert.Contains(error, result.Errors);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void NotFoundResult_ShouldInheritFromResult()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = new NotFoundResult(error);

        // Assert
        Assert.IsAssignableFrom<Result>(result);
    }

    [Fact]
    public void Constructor_WhenCalledWithDifferentErrorTypes_ShouldAcceptAnyError()
    {
        // Arrange
        var error1 = new Error("NOT_FOUND_USER", "User not found");
        var error2 = new Error("NOT_FOUND_PRODUCT", "Product not found");

        // Act
        var result1 = new NotFoundResult(error1);
        var result2 = new NotFoundResult(error2);

        // Assert
        Assert.Contains(error1, result1.Errors);
        Assert.Contains(error2, result2.Errors);
    }

    [Fact]
    public void Constructor_WhenCalledWithEmptyErrorCode_ShouldAcceptError()
    {
        // Arrange
        var error = new Error(string.Empty, "Resource not found");

        // Act
        var result = new NotFoundResult(error);

        // Assert
        Assert.Contains(error, result.Errors);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Constructor_WhenCalledWithEmptyErrorMessage_ShouldAcceptError()
    {
        // Arrange
        var error = new Error("NOT_FOUND", string.Empty);

        // Act
        var result = new NotFoundResult(error);

        // Assert
        Assert.Contains(error, result.Errors);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
    }



    [Fact]
    public void NotFoundResult_WithDifferentErrors_ShouldNotBeEqual()
    {
        // Arrange
        var error1 = new Error("NOT_FOUND_1", "Resource not found");
        var error2 = new Error("NOT_FOUND_2", "Resource not found");
        var result1 = new NotFoundResult(error1);
        var result2 = new NotFoundResult(error2);

        // Act & Assert
        Assert.NotEqual(result1, result2);
        Assert.False(result1 == result2);
        Assert.True(result1 != result2);
    }
}

public class GenericNotFoundResultTests
{
    [Fact]
    public void Constructor_WhenCalledWithError_ShouldSetError()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = new NotFoundResult<string>(error);

        // Assert
        Assert.Single(result.Errors);
        Assert.Contains(error, result.Errors);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Null(result.Data);
    }

    [Fact]
    public void GenericNotFoundResult_ShouldInheritFromGenericResult()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = new NotFoundResult<string>(error);

        // Assert
        Assert.IsAssignableFrom<Result<string>>(result);
    }

    [Fact]
    public void Constructor_WhenCalledWithReferenceType_ShouldHaveNullData()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = new NotFoundResult<string>(error);

        // Assert
        Assert.Null(result.Data);
    }

    [Fact]
    public void Constructor_WhenCalledWithValueType_ShouldHaveDefaultData()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = new NotFoundResult<int>(error);

        // Assert
        Assert.Equal(0, result.Data);
    }

    [Fact]
    public void Constructor_WhenCalledWithNullableValueType_ShouldHaveNullData()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = new NotFoundResult<int?>(error);

        // Assert
        Assert.Null(result.Data);
    }

    [Fact]
    public void Constructor_WhenCalledWithComplexType_ShouldHaveNullData()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = new NotFoundResult<List<string>>(error);

        // Assert
        Assert.Null(result.Data);
    }

    [Fact]
    public void Constructor_WhenCalledWithBoolType_ShouldHaveDefaultBoolData()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = new NotFoundResult<bool>(error);

        // Assert
        Assert.False(result.Data);
    }

    [Fact]
    public void Constructor_WhenCalledWithDecimalType_ShouldHaveDefaultDecimalData()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = new NotFoundResult<decimal>(error);

        // Assert
        Assert.Equal(0m, result.Data);
    }

    [Fact]
    public void Constructor_WhenCalledWithDateTimeType_ShouldHaveDefaultDateTimeData()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = new NotFoundResult<DateTime>(error);

        // Assert
        Assert.Equal(default(DateTime), result.Data);
    }

    [Fact]
    public void Constructor_WhenCalledWithGuidType_ShouldHaveDefaultGuidData()
    {
        // Arrange
        var error = new Error("NOT_FOUND", "Resource not found");

        // Act
        var result = new NotFoundResult<Guid>(error);

        // Assert
        Assert.Equal(Guid.Empty, result.Data);
    }



    [Fact]
    public void GenericNotFoundResult_WithDifferentErrors_ShouldNotBeEqual()
    {
        // Arrange
        var error1 = new Error("NOT_FOUND_1", "Resource not found");
        var error2 = new Error("NOT_FOUND_2", "Resource not found");
        var result1 = new NotFoundResult<string>(error1);
        var result2 = new NotFoundResult<string>(error2);

        // Act & Assert
        Assert.NotEqual(result1, result2);
        Assert.False(result1 == result2);
        Assert.True(result1 != result2);
    }
}
