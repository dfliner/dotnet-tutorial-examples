using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAppDI.Services;

public interface IScopedService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Scoped;
}

internal sealed class ScopedService : IScopedService
{
    private readonly Guid _id;

    public ScopedService()
    {
        _id = Guid.NewGuid();
    }

    public Guid Id => _id;
}
