using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAppDI.Services;

public interface ISingletonService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Singleton;
}

internal sealed class SingletonService : ISingletonService
{
    private readonly Guid id;

    public SingletonService()
    {
        id = Guid.NewGuid();
    }
    public Guid Id => id;
}
