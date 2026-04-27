using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using Shared.Inbox.Abstractions.Consumers;
using Shared.Inbox.Abstractions.Interfaces;
using Shared.Inbox.Abstractions.Models;
using Shared.Messaging.Abstractions.Interfaces;
using Shared.Messaging.Abstractions.Models;

namespace Shared.Inbox.Abstractions.UnitTests.Consumers;

public class InboxConsumerDecoratorTests
{
    [Fact]
    public async Task ConsumeAsync_WhenMessageIdIsMissing_ReturnsNackWithoutRequeue()
    {
        var inner = Substitute.For<IMessageConsumer<TestMessage>>();
        var storage = Substitute.For<IInboxStorage>();
        await using var dbContext = CreateDbContext();
        var context = Substitute.For<IMessageContext>();
        context.MessageId.Returns(string.Empty);
        var decorator = CreateDecorator(inner, dbContext, storage);

        var result = await decorator.ConsumeAsync(new TestMessage(), context, CancellationToken.None);

        Assert.True(result.IsNack);
        Assert.False(result.Requeue);
        await inner.DidNotReceiveWithAnyArgs().ConsumeAsync(default!, default!, default);
        await storage.DidNotReceiveWithAnyArgs().TryRegisterAsync(default!, default);
    }

    [Fact]
    public async Task ConsumeAsync_WhenMessageIsDuplicate_ReturnsAckWithoutCallingInnerConsumer()
    {
        var inner = Substitute.For<IMessageConsumer<TestMessage>>();
        var storage = Substitute.For<IInboxStorage>();
        storage.TryRegisterAsync(Arg.Any<InboxMessage>(), Arg.Any<CancellationToken>())
            .Returns(InboxRegistrationResult.Duplicate());
        await using var dbContext = CreateDbContext();
        var context = CreateContext("message-1");
        var decorator = CreateDecorator(inner, dbContext, storage);

        var result = await decorator.ConsumeAsync(new TestMessage(), context, CancellationToken.None);

        Assert.True(result.IsAck);
        await inner.DidNotReceiveWithAnyArgs().ConsumeAsync(default!, default!, default);
    }

    [Fact]
    public async Task ConsumeAsync_WhenInnerConsumerAcks_UpdatesMessageAndReturnsAck()
    {
        var inner = Substitute.For<IMessageConsumer<TestMessage>>();
        inner.ConsumeAsync(Arg.Any<TestMessage>(), Arg.Any<IMessageContext>(), Arg.Any<CancellationToken>())
            .Returns(ConsumerResult.Ack());
        var storage = Substitute.For<IInboxStorage>();
        storage.TryRegisterAsync(Arg.Any<InboxMessage>(), Arg.Any<CancellationToken>())
            .Returns(InboxRegistrationResult.Registered());
        await using var dbContext = CreateDbContext();
        var context = CreateContext("message-1");
        var decorator = CreateDecorator(inner, dbContext, storage);

        var result = await decorator.ConsumeAsync(new TestMessage(), context, CancellationToken.None);

        Assert.True(result.IsAck);
        await storage.Received(1).UpdateAsync(
            Arg.Is<InboxMessage>(message => message.ProcessedOnUtc.HasValue && message.Error == null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConsumeAsync_WhenInnerConsumerNacksWithError_StoresErrorAndReturnsNack()
    {
        var inner = Substitute.For<IMessageConsumer<TestMessage>>();
        inner.ConsumeAsync(Arg.Any<TestMessage>(), Arg.Any<IMessageContext>(), Arg.Any<CancellationToken>())
            .Returns(ConsumerResult.Nack(requeue: false, error: "failed"));
        var storage = Substitute.For<IInboxStorage>();
        storage.TryRegisterAsync(Arg.Any<InboxMessage>(), Arg.Any<CancellationToken>())
            .Returns(InboxRegistrationResult.Registered());
        await using var dbContext = CreateDbContext();
        var context = CreateContext("message-1");
        var decorator = CreateDecorator(inner, dbContext, storage);

        var result = await decorator.ConsumeAsync(new TestMessage(), context, CancellationToken.None);

        Assert.True(result.IsNack);
        Assert.False(result.Requeue);
        await storage.Received(1).UpdateAsync(
            Arg.Is<InboxMessage>(message => message.Error == "failed"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConsumeAsync_WhenInnerConsumerThrows_StoresErrorAndRethrows()
    {
        var inner = Substitute.For<IMessageConsumer<TestMessage>>();
        inner.ConsumeAsync(Arg.Any<TestMessage>(), Arg.Any<IMessageContext>(), Arg.Any<CancellationToken>())
            .Returns<Task<ConsumerResult>>(_ => throw new InvalidOperationException("boom"));
        var storage = Substitute.For<IInboxStorage>();
        storage.TryRegisterAsync(Arg.Any<InboxMessage>(), Arg.Any<CancellationToken>())
            .Returns(InboxRegistrationResult.Registered());
        await using var dbContext = CreateDbContext();
        var context = CreateContext("message-1");
        var decorator = CreateDecorator(inner, dbContext, storage);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => decorator.ConsumeAsync(new TestMessage(), context, CancellationToken.None));

        await storage.Received(1).UpdateAsync(
            Arg.Is<InboxMessage>(message => message.Error == "boom"),
            Arg.Any<CancellationToken>());
    }

    private static InboxConsumerDecorator<TestMessage> CreateDecorator(
        IMessageConsumer<TestMessage> inner,
        DbContext dbContext,
        IInboxStorage storage) =>
        new(
            inner,
            dbContext,
            storage,
            "consumer",
            NullLogger<InboxConsumerDecorator<TestMessage>>.Instance);

    private static IMessageContext CreateContext(string messageId)
    {
        var context = Substitute.For<IMessageContext>();
        context.MessageId.Returns(messageId);
        return context;
    }

    private static TestDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new TestDbContext(options);
    }

    public sealed record TestMessage;

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options);
}
