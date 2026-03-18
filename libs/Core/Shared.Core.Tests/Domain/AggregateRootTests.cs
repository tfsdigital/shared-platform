using Shared.Core.Domain;

namespace Shared.Core.Tests.Domain;

public class AggregateRootTests
{
    private class TestAggregateRoot : AggregateRoot
    {
        public void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
    }

    [Fact]
    public void Remove_WhenCalled_ShouldSetRemovedAtToCurrentTime()
    {
        // Arrange
        var aggregate = new TestAggregateRoot();
        var beforeRemove = DateTime.UtcNow;

        // Act
        aggregate.Remove();
        var afterRemove = DateTime.UtcNow;

        // Assert
        Assert.NotNull(aggregate.RemovedAt);
        Assert.True(aggregate.RemovedAt >= beforeRemove);
        Assert.True(aggregate.RemovedAt <= afterRemove);
    }

    [Fact]
    public void CreatedAt_WhenInstanceCreated_ShouldBeSetToCurrentTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var aggregate = new TestAggregateRoot();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(aggregate.CreatedAt >= beforeCreation);
        Assert.True(aggregate.CreatedAt <= afterCreation);
    }

    [Fact]
    public void UpdatedAt_WhenNotSet_ShouldBeNull()
    {
        // Arrange & Act
        var aggregate = new TestAggregateRoot();

        // Assert
        Assert.Null(aggregate.UpdatedAt);
    }

    [Fact]
    public void UpdatedAt_WhenSet_ShouldHaveValue()
    {
        // Arrange
        var aggregate = new TestAggregateRoot();
        var beforeUpdate = DateTime.UtcNow;

        // Act
        aggregate.SetUpdatedAt();
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(aggregate.UpdatedAt);
        Assert.True(aggregate.UpdatedAt >= beforeUpdate);
        Assert.True(aggregate.UpdatedAt <= afterUpdate);
    }
}
