namespace ConsoleAppDI.Services;

internal sealed class ServiceLifetimeReporter
{
    private readonly ITransientService _transientService;
    private readonly IScopedService _scopedService;
    private readonly ISingletonService _singletonService;

    public ServiceLifetimeReporter(
        ITransientService transientService,
        IScopedService scopedService,
        ISingletonService singletonService)
    {
        _transientService = transientService;
        _scopedService = scopedService;
        _singletonService = singletonService;
    }

    public void Report(string lifetimeDetails)
    {
        Console.WriteLine(lifetimeDetails);

        LogService(_transientService, "Always different");
        LogService(_scopedService, "Change only with lifetime");
        LogService(_singletonService, "Always the same");
    }

    private void LogService<T>(T service, string message)
        where T : IReportServiceLifetime
    {
        Console.WriteLine($"\t{typeof(T).Name}: {service.Id} ({message})");
    }
}
