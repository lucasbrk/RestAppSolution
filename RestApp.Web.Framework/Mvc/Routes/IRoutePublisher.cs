using System.Web.Routing;

namespace RestApp.Web.Framework.Mvc.Routes
{
    public interface IRoutePublisher
    {
        void RegisterRoutes(RouteCollection routeCollection);
    }
}
