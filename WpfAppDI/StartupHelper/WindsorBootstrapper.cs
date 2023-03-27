using Castle.Windsor;
using Castle.Windsor.Installer;
using System;
using System.Reflection;

namespace WpfAppDI.StartupHelpers;

public static class WindsorBootstrapper
{
    /// <summary>
    /// When the container is instantiated, it automatically scans the application assemblies
    /// (with the same first part of the name as what is first part of the name of 
    /// the executing assembly - "WpfAppDI" in this demo app), and calls the installers to register
    /// all service/dependency components.
    /// </summary>
    private static Lazy<IWindsorContainer> lazy = new Lazy<IWindsorContainer>(
        () => new WindsorContainer()
    );

    public static IWindsorContainer Container
    {
        get
        {
            return lazy.Value;        
        }
    }
}
