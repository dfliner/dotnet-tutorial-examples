using Autofac;
using WestPacificUniversity.EFCore.Repositories;

namespace WestPacificUniversity.DependencyInjection
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(BaseEntityRepository<>))
                .As(typeof(IRepository<>))
                .InstancePerLifetimeScope();
        }


    }
}
