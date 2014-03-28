using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RestApp.Helpers;

namespace RestApp.Models
{
    public class AuthorizeActivityAttribute : AuthorizeAttribute
    {
        int gPermissionCode;

        public AuthorizeActivityAttribute(int pPermissionCode)
        {
            gPermissionCode = pPermissionCode;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return SecurityProvider.CheckPermission(gPermissionCode);
            //return base.AuthorizeCore(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
     
            //base.HandleUnauthorizedRequest(filterContext);
        }
    }
}