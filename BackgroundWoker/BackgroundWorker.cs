using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demo.Services;

/// <summary>
/// Mimics a background service that keeps running until it is cancelled.
/// </summary>
public sealed class BackgroundWorker : BackgroundService
{
    private readonly ILogger<BackgroundWorker> logger;

    public BackgroundWorker(ILogger<BackgroundWorker> logger)
    {
        this.logger = logger;
    }

    // https://www.c-sharpcorner.com/article/implement-background-task-using-backgrounservice-class-in-asp-net-core/
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (true)
        {
            stoppingToken.ThrowIfCancellationRequested();

            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
