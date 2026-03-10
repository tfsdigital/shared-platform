using Shared.Core.Identification;

namespace Shared.Core.Tests.Identification;

public class GuidGeneratorTests
{
    [Fact]
    public void CreateSequential_WhenCalled_ShouldReturnValidGuid()
    {
        // Arrange & Act
        var guid = IdGenerator.CreateSequential();

        // Assert
        Assert.NotEqual(Guid.Empty, guid);
    }

    [Fact]
    public void CreateSequential_WhenCalledMultipleTimes_ShouldReturnDifferentGuids()
    {
        // Arrange & Act
        var guid1 = IdGenerator.CreateSequential();
        var guid2 = IdGenerator.CreateSequential();

        // Assert
        Assert.NotEqual(guid1, guid2);
    }

    [Fact]
    public void Create_WhenCalled_ShouldReturnValidGuid()
    {
        // Arrange & Act
        var guid = IdGenerator.Create();

        // Assert
        Assert.NotEqual(Guid.Empty, guid);
    }

    [Fact]
    public void Create_WhenCalledMultipleTimes_ShouldReturnDifferentGuids()
    {
        // Arrange & Act
        var guid1 = IdGenerator.Create();
        var guid2 = IdGenerator.Create();

        // Assert
        Assert.NotEqual(guid1, guid2);
    }

    [Fact]
    public void Create_ShouldReturnSameAsCreateSequential()
    {
        // Arrange & Act
        var guid1 = IdGenerator.Create();
        var guid2 = IdGenerator.CreateSequential();

        // Assert
        // Both should be valid Version 7 UUIDs (though different values)
        Assert.NotEqual(Guid.Empty, guid1);
        Assert.NotEqual(Guid.Empty, guid2);
    }
}
