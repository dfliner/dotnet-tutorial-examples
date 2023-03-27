// See https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage for more information

using ConsoleAppDI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<ITransientService, TransientService>();
        services.AddScoped<IScopedService, ScopedService>();
        services.AddSingleton<ISingletonService, SingletonService>();
        services.AddTransient<ServiceLifetimeReporter>();
    })
    .Build();

// "await" to ensure completion of the task before printing out "Hello World"
await Task.Run(async () =>
{
    // mimic a long-running task
    await Task.Delay(20000);
    ExemplifyServiceLifetime(host.Services, "Lifetime 1");
    ExemplifyServiceLifetime(host.Services, "Lifetime 2");
});

Console.WriteLine("Hello World!");

await host.RunAsync();

Console.WriteLine("Hello World - Bye!");



static void ExemplifyServiceLifetime(IServiceProvider sp, string lifetime)
{
    using IServiceScope serviceScope = sp.CreateScope();
    IServiceProvider scopedSP = serviceScope.ServiceProvider;
    
    var reporter = scopedSP.GetRequiredService<ServiceLifetimeReporter>();
    reporter.Report($"{lifetime}: Call 1 to ScopedServiceProvider.GetRequiredService<ServiceLifetimeReporter>()");

    Console.WriteLine("...");

    reporter = scopedSP.GetRequiredService<ServiceLifetimeReporter>();
    reporter.Report($"{lifetime}: Call 2 to ScopedServiceProvider.GetRequiredService<ServiceLifetimeReporter>()");

    Console.WriteLine();
}