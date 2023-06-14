using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAppDI.Services;

public interface IScopedService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Scoped;
}

internal sealed class ScopedService : IScopedService
{
    private readonly Guid id;

    public ScopedService()
    {
        id = Guid.NewGuid();
    }

    public Guid Id => id;
}
