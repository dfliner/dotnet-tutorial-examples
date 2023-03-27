using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAppDI.Services;

public interface IReportServiceLifetime
{
    Guid Id { get; }

    ServiceLifetime Lifetime { get; }
}
