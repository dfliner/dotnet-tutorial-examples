// See https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage for more information
// This console app demonstrates generic hosting and dependency injection in console app.

using ConsoleAppDI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<ITransientService, TransientService>();
        services.AddScoped<IScopedService, ScopedService>();
        services.AddSingleton<ISingletonService, SingletonService>();
        services.AddTransient<ServiceLifetimeReporter>();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<IHost>>();
var appLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

appLifetime.ApplicationStarted.Register(
    (logger) => ((ILogger<IHost>)logger!).LogInformation("App: Application started."),
    logger
);
appLifetime.ApplicationStopping.Register(
    (logger) => ((ILogger<IHost>)logger!).LogInformation("App: Application is stopping."),
    logger
);
appLifetime.ApplicationStopped.Register(
    (logger) => ((ILogger<IHost>)logger!).LogInformation("App: Application stopped"),
    logger
);

// "await" to ensure completion of the task before printing out "Hello World"
// Otherwise, Task.Run returns immediately while the task may still be running on the thread pool.
await Task.Run(async () =>
{
    ExemplifyServiceLifetime(host.Services, "Lifetime 1");
    ExemplifyServiceLifetime(host.Services, "Lifetime 2");

    // mimic a long-running task
    await Task.Delay(20000);
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
