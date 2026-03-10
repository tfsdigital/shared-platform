using Shared.Core.Events;

namespace Shared.Core.Tests.Events;

public class IntegrationEventTests
{
    private record TestIntegrationEvent : IntegrationEvent
    {
        public string Name { get; init; } = string.Empty;
        public int Value { get; init; }
    }

    [Fact]
    public void IntegrationEvent_ShouldImplementIIntegrationEvent()
    {
        // Arrange & Act
        var integrationEvent = new TestIntegrationEvent { Name = "Test", Value = 123 };

        // Assert
        Assert.IsAssignableFrom<IIntegrationEvent>(integrationEvent);
    }

    [Fact]
    public void IntegrationEvent_ShouldImplementIEventBase()
    {
        // Arrange & Act
        var integrationEvent = new TestIntegrationEvent { Name = "Test", Value = 123 };

        // Assert
        Assert.IsAssignableFrom<IEventBase>(integrationEvent);
    }

    [Fact]
    public void Id_WhenNotSet_ShouldBeEmptyGuid()
    {
        // Arrange & Act
        var integrationEvent = new TestIntegrationEvent { Name = "Test", Value = 123 };

        // Assert
        Assert.Equal(Guid.Empty, integrationEvent.Id);
    }

    [Fact]
    public void Id_WhenExplicitlySet_ShouldReturnSetValue()
    {
        // Arrange
        var expectedId = Guid.NewGuid();

        // Act
        var integrationEvent = new TestIntegrationEvent { Name = "Test", Value = 123, Id = expectedId };

        // Assert
        Assert.Equal(expectedId, integrationEvent.Id);
    }

    [Fact]
    public void OccurredOn_WhenNotSet_ShouldBeDefaultDateTime()
    {
        // Arrange & Act
        var integrationEvent = new TestIntegrationEvent { Name = "Test", Value = 123 };

        // Assert
        Assert.Equal(default(DateTime), integrationEvent.OccurredOn);
    }

    [Fact]
    public void OccurredOn_WhenExplicitlySet_ShouldReturnSetValue()
    {
        // Arrange
        var expectedDateTime = DateTime.UtcNow;

        // Act
        var integrationEvent = new TestIntegrationEvent { Name = "Test", Value = 123, OccurredOn = expectedDateTime };

        // Assert
        Assert.Equal(expectedDateTime, integrationEvent.OccurredOn);
    }

    [Fact]
    public void Constructor_WhenCalledMultipleTimes_ShouldAllowDifferentIds()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        // Act
        var event1 = new TestIntegrationEvent { Name = "Test1", Value = 1, Id = id1 };
        var event2 = new TestIntegrationEvent { Name = "Test2", Value = 2, Id = id2 };

        // Assert
        Assert.NotEqual(event1.Id, event2.Id);
        Assert.Equal(id1, event1.Id);
        Assert.Equal(id2, event2.Id);
    }

    [Fact]
    public void Constructor_WhenCalledMultipleTimes_ShouldAllowDifferentOccurredOnTimes()
    {
        // Arrange
        var time1 = DateTime.UtcNow;
        var time2 = DateTime.UtcNow.AddMinutes(1);

        // Act
        var event1 = new TestIntegrationEvent { Name = "Test1", Value = 1, OccurredOn = time1 };
        var event2 = new TestIntegrationEvent { Name = "Test2", Value = 2, OccurredOn = time2 };

        // Assert
        Assert.True(event2.OccurredOn > event1.OccurredOn);
        Assert.Equal(time1, event1.OccurredOn);
        Assert.Equal(time2, event2.OccurredOn);
    }

    [Fact]
    public void IntegrationEvent_WhenInitialized_ShouldHaveCorrectProperties()
    {
        // Arrange
        var name = "TestEvent";
        var value = 42;
        var id = Guid.NewGuid();
        var occurredOn = DateTime.UtcNow;

        // Act
        var integrationEvent = new TestIntegrationEvent
        {
            Name = name,
            Value = value,
            Id = id,
            OccurredOn = occurredOn
        };

        // Assert
        Assert.Equal(name, integrationEvent.Name);
        Assert.Equal(value, integrationEvent.Value);
        Assert.Equal(id, integrationEvent.Id);
        Assert.Equal(occurredOn, integrationEvent.OccurredOn);
    }

    [Fact]
    public void Equals_WhenComparingDifferentEvents_ShouldReturnFalse()
    {
        // Arrange
        var event1 = new TestIntegrationEvent { Name = "Test1", Value = 1 };
        var event2 = new TestIntegrationEvent { Name = "Test2", Value = 2 };

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
        var integrationEvent = new TestIntegrationEvent { Name = "Test", Value = 1 };

        // Act
        var result = integrationEvent.Equals(integrationEvent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GetHashCode_WhenCalledOnDifferentEvents_ShouldReturnDifferentHashCodes()
    {
        // Arrange
        var event1 = new TestIntegrationEvent { Name = "Test1", Value = 1 };
        var event2 = new TestIntegrationEvent { Name = "Test2", Value = 2 };

        // Act
        var hashCode1 = event1.GetHashCode();
        var hashCode2 = event2.GetHashCode();

        // Assert
        Assert.NotEqual(hashCode1, hashCode2);
    }
}
