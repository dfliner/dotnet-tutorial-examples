using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace WpfAppDI.DependencyInjection;

/// <summary>
/// Discovers and registers all <see cref="Window"/> or <see cref="Page"/> based types to <see cref="IWindsorContainer"/>.
/// </summary>
public class WindowViewInstaller : IWindsorInstaller
{
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        // TODO: Windows are registered through to .NET DI container which is IServiceCollection.
        //       No need to do this.
        //container.Register(Classes.FromAssembly(Assembly.GetExecutingAssembly())
        //    .BasedOn<Window>()
        //    .OrBasedOn(typeof(Page))
        //    .LifestyleTransient());
    }
}
