﻿using Microsoft.Extensions.Hosting;
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
