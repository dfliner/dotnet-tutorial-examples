using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using WpfAppDI.StartupHelper;
using WpfAppDI.StartupHelpers;
using WpfAppDI.ViewModel;

namespace WpfAppDI;

public partial class App : Application
{
    public IHost? AppHost { get; private set; }

    public App()
    {
        // Generic Host: https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host
        // The host provides a container (IServiceCollection) for dependency injection
        AppHost = Host.CreateDefaultBuilder()
            // https://github.com/castleproject/Windsor/blob/master/docs/net-dependency-extension.md
            // Uses IWindsorContainer as the DI container for the host.
            // No need to cross-wire since IWindsorContainer is the only IServiceProvider.
            // Later you can inject either IWindsorContainer or IServiceProvider to directly access the container.
            // Note, this clears all existing registerions in the container.
            .UseWindsorContainerServiceProvider(WindsorBootstrapper.Container)
            .ConfigureContainer<IWindsorContainer>((hostContext, container) => {
                // Now we can register services/components by convention
                container.Install(FromAssembly.InThisApplication(Assembly.GetExecutingAssembly()));
            })
            .ConfigureServices((hostContext, services) => 
            {
                // These services/components are registered to the IWindsorContainer
                services.AddSingleton<MainWindow>();
                services.AddSingleton<UserViewModel>();
                // factory service for creating child windows
                services.AddFormFactory<ChildWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var startupWindow = AppHost!.Services.GetRequiredService<MainWindow>();
        startupWindow.Show();

        // The same object
        var mainWnd = WindsorBootstrapper.Container.Resolve<MainWindow>();
        Debug.Assert(startupWindow == mainWnd);
        // mainWnd.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();

        WindsorBootstrapper.Container.Dispose();

        base.OnExit(e);
    }
}
