using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MahApps.Metro.Controls.Dialogs;

namespace WpfAppDI.DependencyInjection;

/// <summary>
/// Discovers and registers services to <see cref="IWindsorContainer"/>.
/// </summary>
public class ServiceInstaller : IWindsorInstaller
{
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        // Register IDialogCoordinator so that it can be injected into view models
        container.Register(Component.For<IDialogCoordinator>()
            .Instance(DialogCoordinator.Instance)
            .LifestyleSingleton());
    }
}
