namespace Shared.Results.Tests.Results;

public class ErrorTests
{
    [Fact]
    public void Constructor_WhenCalledWithCodeAndDescription_ShouldSetProperties()
    {
        // Arrange
        var code = "TEST_ERROR";
        var description = "Test error description";

        // Act
        var error = new Error(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void Constructor_WhenCalledWithEmptyCode_ShouldSetEmptyCode()
    {
        // Arrange
        var code = "";
        var description = "Test description";

        // Act
        var error = new Error(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void Constructor_WhenCalledWithEmptyDescription_ShouldSetEmptyDescription()
    {
        // Arrange
        var code = "TEST_CODE";
        var description = "";

        // Act
        var error = new Error(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void Equals_WhenComparingErrorsWithSameCodeAndDescription_ShouldReturnTrue()
    {
        // Arrange
        var code = "TEST_ERROR";
        var description = "Test description";
        var error1 = new Error(code, description);
        var error2 = new Error(code, description);

        // Act
        var result = error1.Equals(error2);

        // Assert
        Assert.True(result);
        Assert.Equal(error1, error2);
    }

    [Fact]
    public void Equals_WhenComparingErrorsWithDifferentCodes_ShouldReturnFalse()
    {
        // Arrange
        var error1 = new Error("CODE1", "Same description");
        var error2 = new Error("CODE2", "Same description");

        // Act
        var result = error1.Equals(error2);

        // Assert
        Assert.False(result);
        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void Equals_WhenComparingErrorsWithDifferentDescriptions_ShouldReturnFalse()
    {
        // Arrange
        var error1 = new Error("SAME_CODE", "Description 1");
        var error2 = new Error("SAME_CODE", "Description 2");

        // Act
        var result = error1.Equals(error2);

        // Assert
        Assert.False(result);
        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void GetHashCode_WhenCalledOnErrorsWithSameValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var code = "TEST_ERROR";
        var description = "Test description";
        var error1 = new Error(code, description);
        var error2 = new Error(code, description);

        // Act
        var hashCode1 = error1.GetHashCode();
        var hashCode2 = error2.GetHashCode();

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void ToString_WhenCalled_ShouldReturnStringRepresentation()
    {
        // Arrange
        var code = "TEST_ERROR";
        var description = "Test description";
        var error = new Error(code, description);

        // Act
        var result = error.ToString();

        // Assert
        Assert.NotNull(result);
        Assert.Contains(code, result);
        Assert.Contains(description, result);
    }
}
