using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.Events;
using Shared.Handlers.Extensions;
using Shared.Results;

namespace Shared.Handlers.Tests.Extensions;

public class HandlersDependencyInjectionTests
{
    private readonly IServiceCollection _services;
    private readonly Assembly _testAssembly;

    public HandlersDependencyInjectionTests()
    {
        _services = new ServiceCollection();
        _testAssembly = typeof(HandlersDependencyInjectionTests).Assembly;
    }

    [Fact]
    public void AddHandlersFromAssembly_WithValidAssembly_ShouldReturnServiceCollection()
    {
        // Act
        var result = _services.AddHandlersFromAssembly(_testAssembly);

        // Assert
        Assert.Same(_services, result);
    }

    [Fact]
    public void AddHandlersFromAssembly_WithEventHandler_ShouldRegisterHandler()
    {
        // Arrange
        var assembly = typeof(TestEventHandler).Assembly;

        // Act
        _services.AddHandlersFromAssembly(assembly);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var handler = serviceProvider.GetService<IEventHandler<TestEvent>>();
        Assert.NotNull(handler);
        Assert.IsType<TestEventHandler>(handler);
    }

    [Fact]
    public void AddHandlersFromAssembly_WithCommandHandler_ShouldRegisterHandler()
    {
        // Arrange
        var assembly = typeof(TestCommandHandler).Assembly;

        // Act
        _services.AddHandlersFromAssembly(assembly);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var handler = serviceProvider.GetService<ICommandHandler<TestCommand, TestResult>>();
        Assert.NotNull(handler);
        Assert.IsType<TestCommandHandler>(handler);
    }

    [Fact]
    public void AddHandlersFromAssembly_WithCommandHandlerWithoutResult_ShouldRegisterHandler()
    {
        // Arrange
        var assembly = typeof(TestCommandWithoutResultHandler).Assembly;

        // Act
        _services.AddHandlersFromAssembly(assembly);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var handler = serviceProvider.GetService<ICommandHandler<TestCommandWithoutResult>>();
        Assert.NotNull(handler);
        Assert.IsType<TestCommandWithoutResultHandler>(handler);
    }

    [Fact]
    public void AddHandlersFromAssembly_WithQueryHandler_ShouldRegisterHandler()
    {
        // Arrange
        var assembly = typeof(TestQueryHandler).Assembly;

        // Act
        _services.AddHandlersFromAssembly(assembly);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var handler = serviceProvider.GetService<IQueryHandler<TestQuery, TestQueryResult>>();
        Assert.NotNull(handler);
        Assert.IsType<TestQueryHandler>(handler);
    }

    [Fact]
    public void AddHandlersFromAssembly_WithAbstractClass_ShouldNotRegisterAbstractClass()
    {
        // Arrange
        var assembly = typeof(AbstractTestHandler).Assembly;

        // Act
        _services.AddHandlersFromAssembly(assembly);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var handlers = serviceProvider.GetServices<IEventHandler<TestEvent>>();
        // Should only include concrete implementations, not abstract classes
        Assert.DoesNotContain(handlers, h => h.GetType() == typeof(AbstractTestHandler));
    }

    [Fact]
    public void AddHandlersFromAssembly_WithInterface_ShouldNotRegisterInterface()
    {
        // Arrange
        var assembly = typeof(ITestService).Assembly;

        // Act
        _services.AddHandlersFromAssembly(assembly);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<ITestService>();
        Assert.Null(service); // Interfaces should not be registered
    }

    [Fact]
    public void AddHandlersFromAssembly_WithNullAssembly_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _services.AddHandlersFromAssembly(null!));
    }

    [Fact]
    public void AddHandlersFromAssembly_RegistersAsTransient()
    {
        // Arrange
        var assembly = typeof(TestEventHandler).Assembly;

        // Act
        _services.AddHandlersFromAssembly(assembly);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var handler1 = serviceProvider.GetService<IEventHandler<TestEvent>>();
        var handler2 = serviceProvider.GetService<IEventHandler<TestEvent>>();

        Assert.NotSame(handler1, handler2); // Should be different instances (transient)
    }

    [Fact]
    public void AddHandlersFromAssembly_SkipsNonHandlerTypes()
    {
        // Arrange
        var assembly = typeof(NonHandlerClass).Assembly;

        // Act
        _services.AddHandlersFromAssembly(assembly);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<NonHandlerClass>();
        Assert.Null(service); // Non-handler classes should not be registered
    }
}

// Test classes for the unit tests
public class TestEvent : IEventBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
}

public class TestCommand : ICommand<TestResult>
{
}

public class TestCommandWithoutResult : ICommand
{
}

public class TestQuery : IQuery<TestQueryResult>
{
}

public class TestResult
{
    public string Value { get; set; } = string.Empty;
}

public class TestQueryResult
{
    public string Data { get; set; } = string.Empty;
}

public class TestEventHandler : IEventHandler<TestEvent>
{
    public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

public class TestCommandHandler : ICommandHandler<TestCommand, TestResult>
{
    public Task<Result<TestResult>> Handle(TestCommand command, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result<TestResult>.Success(new TestResult()));
    }
}

public class TestCommandWithoutResultHandler : ICommandHandler<TestCommandWithoutResult>
{
    public Task<Result> Handle(TestCommandWithoutResult command, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success());
    }
}

public class TestQueryHandler : IQueryHandler<TestQuery, TestQueryResult>
{
    public Task<Result<TestQueryResult>> Handle(TestQuery query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result<TestQueryResult>.Success(new TestQueryResult()));
    }
}

public abstract class AbstractTestHandler : IEventHandler<TestEvent>
{
    public abstract Task HandleAsync(TestEvent @event, CancellationToken cancellationToken = default);
}

public interface ITestService
{
}

public class NonHandlerClass
{
    public string SomeProperty { get; set; } = string.Empty;
}
