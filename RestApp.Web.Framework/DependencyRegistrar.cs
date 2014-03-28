using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mvc;
using RestApp.Core;
using RestApp.Core.Caching;
using RestApp.Core.Configuration;
using RestApp.Core.Data;
using RestApp.Core.Fakes;
using RestApp.Core.Infrastructure;
using RestApp.Core.Infrastructure.DependencyManagement;
using RestApp.Services;
using RestApp.Services.Users;
using RestApp.Services.Configuration;
using RestApp.Services.Localization;
using RestApp.Web.Framework.EmbeddedViews;
using RestApp.Web.Framework.Mvc.Routes;
using RestApp.Services.Events;
using RestApp.Services.Security;
using RestApp.Services.Authentication;
using RestApp.Services.Logging;
using RestApp.Common.Utility;
using RestApp.Services.Installation;
using RestApp.Services.Roles;
using RestApp.Services.Tables;

namespace RestApp.Web.Framework
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerHttpRequest();

            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerHttpRequest();

            //controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();
            builder.Register(c => dataSettingsManager.LoadSettings()).As<DataSettings>();
            builder.Register(x => new EfDataProviderManager(x.Resolve<DataSettings>())).As<BaseDataProviderManager>().InstancePerDependency();


            builder.Register(x => (IEfDataProvider)x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();
            builder.Register(x => (IEfDataProvider)x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IEfDataProvider>().InstancePerDependency();

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                var efDataProviderManager = new EfDataProviderManager(dataSettingsManager.LoadSettings());
                var dataProvider = (IEfDataProvider)efDataProviderManager.LoadDataProvider();
                dataProvider.InitConnectionFactory();

                builder.Register<IDbContext>(c => new ApObjectContext(dataProviderSettings.DataConnectionString)).InstancePerHttpRequest();
            }
            else
            {
                builder.Register<IDbContext>(c => new ApObjectContext(dataSettingsManager.LoadSettings().DataConnectionString)).InstancePerHttpRequest();
            }

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();

            //cache manager
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("apsalud_cache_static").SingleInstance();
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("apsalud_cache_per_request").InstancePerHttpRequest();


            //work context
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerHttpRequest();

            builder.RegisterGeneric(typeof(ConfigurationProvider<>)).As(typeof(IConfigurationProvider<>));
            builder.RegisterSource(new SettingsSource());

            ////services  
            builder.RegisterType<UserService>().As<IUserService>().InstancePerHttpRequest();
            builder.RegisterType<RoleService>().As<IRoleService>().InstancePerHttpRequest();        
            builder.RegisterType<TableService>().As<ITableService>().InstancePerHttpRequest();

            //pass MemoryCacheManager to SettingService as cacheManager (cache settngs between requests)
            builder.RegisterType<PermissionService>().As<IPermissionService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("apsalud_cache_static"))
                .InstancePerHttpRequest();

            //pass MemoryCacheManager to SettingService as cacheManager (cache settngs between requests)
            builder.RegisterType<SettingService>().As<ISettingService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("apsalud_cache_static"))
                .InstancePerHttpRequest();
            //pass MemoryCacheManager to LocalizationService as cacheManager (cache locales between requests)
            builder.RegisterType<LocalizationService>().As<ILocalizationService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("apsalud_cache_static"))
                .InstancePerHttpRequest();

            //pass MemoryCacheManager to LocalizedEntityService as cacheManager (cache locales between requests)
            ////builder.RegisterType<LocalizedEntityService>().As<ILocalizedEntityService>()
            //    .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("apsalud_cache_static"))
            //    .InstancePerHttpRequest();
            builder.RegisterType<LanguageService>().As<ILanguageService>().InstancePerHttpRequest();

            builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerHttpRequest();         

            builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerHttpRequest();
            builder.RegisterType<UserActivityService>().As<IUserActivityService>().InstancePerHttpRequest();

            builder.RegisterType<InstallationService>().As<IInstallationService>().InstancePerHttpRequest();

            builder.RegisterType<EmbeddedViewResolver>().As<IEmbeddedViewResolver>().SingleInstance();
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();

            //Register event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            {
                builder.RegisterType(consumer)
                    .As(consumer.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IConsumer<>)))
                    .InstancePerHttpRequest();
            }
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();
            
        }

        public int Order
        {
            get { return 0; }
        }
    }


    public class SettingsSource : IRegistrationSource
    {
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);

        public IEnumerable<IComponentRegistration> RegistrationsFor(
                Service service,
                Func<Service, IEnumerable<IComponentRegistration>> registrations)
        {
            var ts = service as TypedService;
            if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration)buildMethod.Invoke(null, null);
            }
        }

        static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder
                .ForDelegate((c, p) => c.Resolve<IConfigurationProvider<TSettings>>().Settings)
                .InstancePerHttpRequest()
                .CreateRegistration();
        }

        public bool IsAdapterForIndividualComponents { get { return false; } }

    }

}
