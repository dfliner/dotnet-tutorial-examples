using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAppDI.Services;

public interface ISingletonService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Singleton;
}

internal sealed class SingletonService : ISingletonService
{
    private readonly Guid _id;

    public SingletonService()
    {
        _id = Guid.NewGuid();
    }
    public Guid Id => _id;
}
