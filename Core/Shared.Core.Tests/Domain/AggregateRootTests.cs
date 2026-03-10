using Shared.Core.Domain;
using Shared.Core.Events;

namespace Shared.Core.Tests.Domain;

public class AggregateRootTests
{
    private class TestAggregateRoot : AggregateRoot
    {
        public void PublicRaiseEvent(IDomainEvent domainEvent) => RaiseEvent(domainEvent);

        public void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
    }

    private record TestDomainEvent : DomainEvent;

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
    public void RaiseEvent_WhenCalledWithDomainEvent_ShouldAddEventToDomainEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot();
        var domainEvent = new TestDomainEvent();

        // Act
        aggregate.PublicRaiseEvent(domainEvent);

        // Assert
        Assert.Single(aggregate.DomainEvents);
        Assert.Contains(domainEvent, aggregate.DomainEvents);
    }

    [Fact]
    public void RaiseEvent_WhenCalledMultipleTimes_ShouldAddAllEventsToDomainEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot();
        var firstEvent = new TestDomainEvent();
        var secondEvent = new TestDomainEvent();

        // Act
        aggregate.PublicRaiseEvent(firstEvent);
        aggregate.PublicRaiseEvent(secondEvent);

        // Assert
        Assert.Equal(2, aggregate.DomainEvents.Count);
        Assert.Contains(firstEvent, aggregate.DomainEvents);
        Assert.Contains(secondEvent, aggregate.DomainEvents);
    }

    [Fact]
    public void ClearEvents_WhenCalled_ShouldRemoveAllDomainEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot();
        var domainEvent = new TestDomainEvent();
        aggregate.PublicRaiseEvent(domainEvent);

        // Act
        aggregate.ClearEvents();

        // Assert
        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void DomainEvents_WhenAccessed_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        var aggregate = new TestAggregateRoot();
        var domainEvent = new TestDomainEvent();
        aggregate.PublicRaiseEvent(domainEvent);

        // Act
        var events = aggregate.DomainEvents;

        // Assert
        Assert.IsAssignableFrom<IReadOnlyCollection<IDomainEvent>>(events);
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

    [Fact]
    public void Constructor_WhenCalled_ShouldInitializeWithEmptyDomainEvents()
    {
        // Arrange & Act
        var aggregate = new TestAggregateRoot();

        // Assert
        Assert.Empty(aggregate.DomainEvents);
    }
}
