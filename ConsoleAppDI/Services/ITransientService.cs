using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAppDI.Services;

public interface ITransientService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Transient;
}

internal sealed class TransientService : ITransientService
{
    private readonly Guid _id;

    public TransientService()
    {
        _id = Guid.NewGuid();
    }
    public Guid Id => _id;
}
