// This console app demonstrates generic hosting and dependency injection in console app.
// https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host
// https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage
// https://learn.microsoft.com/en-us/dotnet/core/extensions/logging
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging
// https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration


using ConsoleAppDI.Services;
using Demo.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;



// App host provides the following functionality:
// 1. IoC/DI
// 2. Logging
// 3. Config
// 4. App lifetime management
// 5. Hosting services (IHostedService implementations)
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((logging) =>
    {
        logging.AddLog4Net();
        logging.SetMinimumLevel(LogLevel.Debug);
    })
    .ConfigureServices(services =>
    {
        services.AddTransient<ITransientService, TransientService>();
        services.AddScoped<IScopedService, ScopedService>();
        services.AddSingleton<ISingletonService, SingletonService>();
        services.AddTransient<ServiceLifetimeReporter>();

        services.AddHostedService<BackgroundWorker>();

        // Requires Microsoft.Extensions.Http package
        services.AddHttpClient();
        services.AddSingleton<IWeatherService, WeatherService>();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<IHost>>();

var weatherService = host.Services.GetRequiredService<IWeatherService>();
var json = await weatherService.GetWeatherServiceMetadataAsync();

// Control over app startup and shutdown
// For demo purpose only
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
    await Task.Delay(10000);
});

await host.RunAsync();


Console.WriteLine();
Console.Write("Press Enter to Exit!");
Console.ReadLine();
Console.WriteLine("Hello World - Bye!");



static void ExemplifyServiceLifetime(IServiceProvider sp, string lifetime)
{
    var logger = sp.GetRequiredService<ILogger<Program>>();

    using IServiceScope serviceScope = sp.CreateScope();
    IServiceProvider scopedSP = serviceScope.ServiceProvider;
    
    var reporter = scopedSP.GetRequiredService<ServiceLifetimeReporter>();
    reporter.Report($"{lifetime}: Call 1 to ScopedServiceProvider.GetRequiredService<ServiceLifetimeReporter>()");
    logger!.LogDebug($"{lifetime}: Call 1 to ScopedServiceProvider.GetRequiredService<ServiceLifetimeReporter>()");

    Console.WriteLine("...");

    reporter = scopedSP.GetRequiredService<ServiceLifetimeReporter>();
    reporter.Report($"{lifetime}: Call 2 to ScopedServiceProvider.GetRequiredService<ServiceLifetimeReporter>()");
    logger!.LogDebug($"{lifetime}: Call 2 to ScopedServiceProvider.GetRequiredService<ServiceLifetimeReporter>()");

    Console.WriteLine();
}
