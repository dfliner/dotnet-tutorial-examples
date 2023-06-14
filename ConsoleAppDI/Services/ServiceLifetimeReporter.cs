using Microsoft.Extensions.Logging;

namespace ConsoleAppDI.Services;

internal sealed class ServiceLifetimeReporter
{
    private readonly ITransientService transientService;
    private readonly IScopedService scopedService;
    private readonly ISingletonService singletonService;
    private readonly ILogger<ServiceLifetimeReporter> logger;

    public ServiceLifetimeReporter(
        ITransientService transientService,
        IScopedService scopedService,
        ISingletonService singletonService,
        ILogger<ServiceLifetimeReporter> logger)
    {
        this.transientService = transientService;
        this.scopedService = scopedService;
        this.singletonService = singletonService;
        this.logger = logger;
    }

    public void Report(string lifetimeDetails)
    {
        Console.WriteLine(lifetimeDetails);

        LogService(transientService, "Always different");
        LogService(scopedService, "Change only with lifetime");
        LogService(singletonService, "Always the same");
    }

    private void LogService<T>(T service, string message)
        where T : IReportServiceLifetime
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("Console:");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{typeof(T).Name}: {service.Id} ({message})");

        logger!.LogWarning($"\t{typeof(T).Name}: {service.Id} ({message})");
    }
}
