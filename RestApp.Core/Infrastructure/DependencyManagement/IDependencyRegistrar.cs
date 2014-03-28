using Autofac;

namespace RestApp.Core.Infrastructure.DependencyManagement
{
    //TODO: ver typeFinder
    public interface IDependencyRegistrar
    {
        void Register(ContainerBuilder builder, ITypeFinder typeFinder);

        int Order { get; }
    }
}
