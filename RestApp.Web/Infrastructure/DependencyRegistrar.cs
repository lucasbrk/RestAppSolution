using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using RestApp.Core.Caching;
using RestApp.Core.Infrastructure;
using RestApp.Core.Infrastructure.DependencyManagement;
using RestApp.Web.Infrastructure.Installation;

namespace RestApp.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //installation localization service
            builder.RegisterType<InstallationLocalizationService>().As<IInstallationLocalizationService>().InstancePerHttpRequest();
        }

        public int Order
        {
            get { return 2; }
        }
    }
}
