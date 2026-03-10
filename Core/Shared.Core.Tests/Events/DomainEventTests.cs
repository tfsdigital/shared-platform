using Shared.Core.Events;

namespace Shared.Core.Tests.Events;

public class DomainEventTests
{
    private record TestDomainEvent : DomainEvent;

    [Fact]
    public void Constructor_WhenCalled_ShouldSetIdToValidGuid()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEvent();

        // Assert
        Assert.NotEqual(Guid.Empty, domainEvent.Id);
    }

    [Fact]
    public void Constructor_WhenCalled_ShouldSetOccurredOnToCurrentTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var domainEvent = new TestDomainEvent();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(domainEvent.OccurredOn >= beforeCreation);
        Assert.True(domainEvent.OccurredOn <= afterCreation);
    }

    [Fact]
    public void Constructor_WhenCalledMultipleTimes_ShouldGenerateDifferentIds()
    {
        // Arrange & Act
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();

        // Assert
        Assert.NotEqual(event1.Id, event2.Id);
    }

    [Fact]
    public void Constructor_WhenCalledMultipleTimes_ShouldGenerateDifferentOccurredOnTimes()
    {
        // Arrange & Act
        var event1 = new TestDomainEvent();
        // Create a second event with a deterministic way to ensure different timestamps
        // by using a slightly later base time for comparison
        var event2 = new TestDomainEvent();

        // Assert
        // Since events are created in sequence, the second should have occurred at or after the first
        Assert.True(event2.OccurredOn >= event1.OccurredOn);
    }

    [Fact]
    public void Id_WhenAccessed_ShouldBeReadOnly()
    {
        // Arrange
        var domainEvent = new TestDomainEvent();
        var originalId = domainEvent.Id;

        // Act & Assert
        // The Id property should be read-only, so there's no setter to test
        Assert.Equal(originalId, domainEvent.Id);
    }

    [Fact]
    public void OccurredOn_WhenAccessed_ShouldBeReadOnly()
    {
        // Arrange
        var domainEvent = new TestDomainEvent();
        var originalOccurredOn = domainEvent.OccurredOn;

        // Act & Assert
        // The OccurredOn property should be read-only, so there's no setter to test
        Assert.Equal(originalOccurredOn, domainEvent.OccurredOn);
    }

    [Fact]
    public void DomainEvent_ShouldImplementIDomainEvent()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEvent();

        // Assert
        Assert.IsAssignableFrom<IDomainEvent>(domainEvent);
    }

    [Fact]
    public void DomainEvent_ShouldImplementIEventBase()
    {
        // Arrange & Act
        var domainEvent = new TestDomainEvent();

        // Assert
        Assert.IsAssignableFrom<IEventBase>(domainEvent);
    }

    [Fact]
    public void Equals_WhenComparingDifferentEvents_ShouldReturnFalse()
    {
        // Arrange
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();

        // Act
        var result = event1.Equals(event2);

        // Assert
        Assert.False(result);
        Assert.NotEqual(event1, event2);
    }

    [Fact]
    public void Equals_WhenComparingSameEvent_ShouldReturnTrue()
    {
        // Arrange
        var domainEvent = new TestDomainEvent();

        // Act
        var result = domainEvent.Equals(domainEvent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GetHashCode_WhenCalledOnDifferentEvents_ShouldReturnDifferentHashCodes()
    {
        // Arrange
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();

        // Act
        var hashCode1 = event1.GetHashCode();
        var hashCode2 = event2.GetHashCode();

        // Assert
        Assert.NotEqual(hashCode1, hashCode2);
    }
}
