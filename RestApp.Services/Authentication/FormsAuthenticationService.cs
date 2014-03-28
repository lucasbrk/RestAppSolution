using System;
using System.Web;
using System.Web.Security;
using RestApp.Core.Domain.Users;
using RestApp.Services.Users;

namespace RestApp.Services.Authentication
{
    /// <summary>
    /// Authentication service
    /// </summary>
    public partial class FormsAuthenticationService : IAuthenticationService
    {
        private readonly HttpContextBase gHttpContext;
        private readonly IUserService gUserService;
        private readonly TimeSpan gExpirationTimeSpan;

        private User gCachedUser;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="userService">User service</param>
        /// <param name="userSettings">User settings</param>
        public FormsAuthenticationService(HttpContextBase httpContext,
            IUserService userService)
        {
            this.gHttpContext = httpContext;
            this.gUserService = userService;
            this.gExpirationTimeSpan = FormsAuthentication.Timeout;
        }


        public virtual void SignIn(User user, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                user.LoginName,
                now,
                now.Add(gExpirationTimeSpan),
                createPersistentCookie,
                user.LoginName,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            gHttpContext.Response.Cookies.Add(cookie);
            gCachedUser = user;
        }

        public virtual void SignOut()
        {
            gCachedUser = null;
            FormsAuthentication.SignOut();
        }

        public virtual User GetAuthenticatedUser()
        {
            if (gCachedUser != null)
                return gCachedUser;

            if (gHttpContext == null ||
                gHttpContext.Request == null ||
                !gHttpContext.Request.IsAuthenticated ||
                !(gHttpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)gHttpContext.User.Identity;
            var user = GetAuthenticatedUserFromTicket(formsIdentity.Ticket);
            if (user != null && user.Enabled)
            {
                gCachedUser = user;
            }

            return gCachedUser;
        }

        public virtual User GetAuthenticatedUserFromTicket(FormsAuthenticationTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            var loginName = ticket.Name;

            if (String.IsNullOrWhiteSpace(loginName))
                return null;
            var user = gUserService.GetUserByLoginName(loginName);
            return user; //TODO: ver si conviene null
        }
    }
}