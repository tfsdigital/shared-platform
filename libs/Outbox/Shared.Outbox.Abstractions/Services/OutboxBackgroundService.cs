using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Shared.Outbox.Abstractions.Database;
using Shared.Outbox.Abstractions.Settings;

namespace Shared.Outbox.Abstractions.Services;

internal sealed class OutboxBackgroundService<TContext>(
    OutboxProcessor<TContext> processor,
    IOptions<OutboxProcessorOptions> processorOptions)
    : BackgroundService where TContext : DbContext, IOutboxDbContext
{
    private readonly OutboxProcessorOptions _processor = processorOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!IsCancellationRequested(stoppingToken))
        {
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = _processor.MaxParallelism,
                CancellationToken = stoppingToken
            };

            var processedAny = 0;

            await Parallel.ForEachAsync(
                Enumerable.Range(0, _processor.MaxParallelism),
                parallelOptions,
                async (_, token) =>
                {
                    var hadMessages = await processor.ProcessMessagesAsync(token);
                    if (hadMessages) Interlocked.Increment(ref processedAny);
                });

            if (processedAny == 0)
                await Task.Delay(TimeSpan.FromSeconds(_processor.IntervalInSeconds), stoppingToken);
        }
    }

    [SuppressMessage("Minor Code Smell", "S2589:Boolean expressions should not be gratuitous", Justification = "BackgroundService loops until the host cancels the token; async operations also observe the same token.")]
    private static bool IsCancellationRequested(CancellationToken stoppingToken)
    {
        return stoppingToken.IsCancellationRequested;
    }
}
