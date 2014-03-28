using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using RestApp.Core.Infrastructure;
using RestApp.Web.Framework.Mvc.Routes;
using RestApp.Core.Data;
using RestApp.Web.Framework.Mvc;
using RestApp.Core.Domain;
using StackExchange.Profiling.MVCHelpers;
using RestApp.Web.Framework;
using RestApp.Web.Framework.EmbeddedViews;
using System.Web.Hosting;
using StackExchange.Profiling;
using RestApp.Core;
using System.Globalization;
using System.Threading;
using RestApp.Services.Logging;

namespace RestApp
{
    // Nota: para obtener instrucciones sobre cómo habilitar el modo clásico de IIS6 o IIS7, 
    // visite http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //do not register HandleErrorAttribute. use classic error handling mode
            //filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //register custom routes (plugins, etc)
            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "RestApp.Web.Controllers" }
            );
        }

        protected void Application_Start()
        {  
            //initialize engine context
            EngineContext.Initialize(false);

            bool databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();

            //set dependency resolver
            var dependencyResolver = new ApDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            //model binders
            ModelBinders.Binders.Add(typeof(BaseApModel), new ApModelBinder());

            //Add some functionality on top of the default ModelMetadataProvider
            ModelMetadataProviders.Current = new ApMetadataProvider();

            //Registering some regular mvc stuf
            AreaRegistration.RegisterAllAreas();

            if (databaseInstalled)
            {
                GlobalFilters.Filters.Add(new ProfilingActionFilter());
            }
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            //register virtual path provider for embedded views
            var embeddedViewResolver = EngineContext.Current.Resolve<IEmbeddedViewResolver>();
            var embeddedProvider = new EmbeddedViewVirtualPathProvider(embeddedViewResolver.GetEmbeddedViews());
            HostingEnvironment.RegisterVirtualPathProvider(embeddedProvider);   

            WebApiConfig.Register(GlobalConfiguration.Configuration); 

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AuthConfig.RegisterAuth();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            EnsureDatabaseIsInstalled();

            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                MiniProfiler.Start();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                //stop as early as you can, even earlier with MvcMiniProfiler.MiniProfiler.Stop(discardResults: true);
                MiniProfiler.Stop();
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //we don't do it in Application_BeginRequest because a user is not authenticated yet
            SetWorkingCulture();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            //disable compression (if enabled). More info - http://stackoverflow.com/questions/3960707/asp-net-mvc-weird-characters-in-error-page
            //log error
            LogException(Server.GetLastError());
        }

        protected void EnsureDatabaseIsInstalled()
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            string installUrl = string.Format("{0}install", webHelper.GetSiteLocation());
            if (!webHelper.IsStaticResource(this.Request) &&
                !DataSettingsHelper.DatabaseIsInstalled() &&
                !webHelper.GetThisPageUrl(false).StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Response.Redirect(installUrl);
            }
        }

        protected void SetWorkingCulture()
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            if (webHelper.IsStaticResource(this.Request))
                return;

            //keep alive page requested (we ignore it to prevnt creating a guest user records)
            string keepAliveUrl = string.Format("{0}keepalive", webHelper.GetSiteLocation());
            if (webHelper.GetThisPageUrl(false).StartsWith(keepAliveUrl, StringComparison.InvariantCultureIgnoreCase))
                return;


            if (webHelper.GetThisPageUrl(false).StartsWith(string.Format("{0}admin", webHelper.GetSiteLocation()),
                StringComparison.InvariantCultureIgnoreCase))
            {
                //admin area


                //always set culture to 'en-US'
                //we set culture of admin area to 'en-US' because current implementation of Telerik grid 
                //doesn't work well in other cultures
                //e.g., editing decimal value in russian culture
                var culture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            else
            {
               
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                if (workContext.CurrentUser != null && workContext.WorkingLanguage != null)
                {
                    var culture = new CultureInfo(workContext.WorkingLanguage.LanguageCulture);
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
            }
        }

        protected void LogException(Exception exc)
        {
            if (exc == null)
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            try
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                logger.Error(exc.Message, exc, workContext.CurrentUser);
            }
            catch (Exception)
            {
                //don't throw new exception if occurs
            }
        }
    }
}