using System;
using System.Linq;
using System.Web;
using RestApp.Core;
using RestApp.Core.Domain.Users;
using RestApp.Core.Domain.Localization;
using RestApp.Services.Authentication;
using RestApp.Services.Users;
using RestApp.Services.Localization;
using RestApp.Web.Framework.Localization;

namespace RestApp.Web.Framework
{
    /// <summary>
    /// Working context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        private const string UserCookieName = "RestApp.user";

        private readonly HttpContextBase gHttpContext;
        private readonly IUserService gUserService;
        private readonly IAuthenticationService gAuthenticationService;
        private readonly ILanguageService gLanguageService;
        private readonly IWebHelper gWebHelper;

        private User gCachedUser;
        //private User gOriginalUserIfImpersonated;
        private bool gCachedIsAdmin;

        public WebWorkContext(HttpContextBase httpContext,
            IUserService userService,
            IAuthenticationService authenticationService,
            ILanguageService languageService,
            IWebHelper webHelper)
        {
            this.gHttpContext = httpContext;
            this.gUserService = userService;
            this.gAuthenticationService = authenticationService;
            this.gLanguageService = languageService;
            this.gWebHelper = webHelper;
        }

        protected User GetCurrentUser()
        {
            if (gCachedUser != null)
                return gCachedUser;

            User user = null;
            if (gHttpContext != null)
            {
                //check whether request is made by a search engine
                //in this case return built-in user record for search engines 
                //or comment the following two lines of code in order to disable this functionality
                //if (gWebHelper.IsSearchEngine(gHttpContext))
                //    user = gUserService.GetUserBySystemName(SystemUserNames.SearchEngine);

                //registered user
                if (user == null || !user.Enabled)
                {
                    user = gAuthenticationService.GetAuthenticatedUser();
                }

                //impersonate user if required (currently used for 'phone order' support)
                //if (user != null && user.Enabled)
                //{
                //        int? impersonatedUserId = user.GetAttribute<int?>(SystemUserAttributeNames.ImpersonatedUserId);
                //        if (impersonatedUserId.HasValue && impersonatedUserId.Value > 0)
                //        {
                //            var impersonatedUser = gUserService.GetUserById(impersonatedUserId.Value);
                //            if (impersonatedUser != null && !impersonatedUser.Deleted && impersonatedUser.Active)
                //            {
                //                //set impersonated user
                //                gOriginalUserIfImpersonated = user;
                //                user = impersonatedUser;
                //            }
                //        }
                //}

                //load guest user
                if (user == null || !user.Enabled)
                {
                    var userCookie = GetUserCookie();
                    if (userCookie != null && !String.IsNullOrEmpty(userCookie.Value))
                    {
                        Guid userGuid;
                        if (Guid.TryParse(userCookie.Value, out userGuid))
                        {
                            var userByCookie = gUserService.GetUserByGuid(userGuid);
                            if (userByCookie != null)
                                ////this user (from cookie) should not be registered
                                //!userByCookie.IsRegistered() &&
                                ////it should not be a built-in 'search engine' user account
                                //!userByCookie.IsSearchEngineAccount())
                                user = userByCookie;
                        }
                    }
                }

                //if not exists
                if (user == null || !user.Enabled)
                {
                    user = null;
                }
                else
                {
                    SetUserCookie(user.UserGuid);
                }
            }

            //validation
            if (user != null &&  user.Enabled)
            {
                //update last activity date
                if (user.LastActivityDateUtc.AddMinutes(1.0) < DateTime.UtcNow)
                {
                    user.LastActivityDateUtc = DateTime.UtcNow;
                    gUserService.UpdateUser(user);
                }

                //update IP address
                string currentIpAddress = gWebHelper.GetCurrentIpAddress();
                if (!String.IsNullOrEmpty(currentIpAddress))
                {
                    if (!currentIpAddress.Equals(user.LastIpAddress))
                    {
                        user.LastIpAddress = currentIpAddress;
                        gUserService.UpdateUser(user);
                    }
                }

                gCachedUser = user;
            }

            return gCachedUser;
        }

        protected HttpCookie GetUserCookie()
        {
            if (gHttpContext == null || gHttpContext.Request == null)
                return null;

            return gHttpContext.Request.Cookies[UserCookieName];
        }

        protected void SetUserCookie(Guid userGuid)
        {
            var cookie = new HttpCookie(UserCookieName);
            cookie.Value = userGuid.ToString();
            if (userGuid == Guid.Empty)
            {
                cookie.Expires = DateTime.Now.AddMonths(-1);
            }
            else
            {
                int cookieExpires = 24 * 365; //TODO make configurable
                cookie.Expires = DateTime.Now.AddHours(cookieExpires);
            }
            if (gHttpContext != null && gHttpContext.Response != null)
            {
                gHttpContext.Response.Cookies.Remove(UserCookieName);
                gHttpContext.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return GetCurrentUser();
            }
            set
            {
                SetUserCookie(value.UserGuid);
                gCachedUser = value;
            }
        }

        ///// <summary>
        ///// Gets or sets the original user (in case the current one is impersonated)
        ///// </summary>
        //public User OriginalUserIfImpersonated
        //{
        //    get
        //    {
        //        return gOriginalUserIfImpersonated;
        //    }
        //}

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public Language WorkingLanguage
        {
            get
            {
                if (this.CurrentUser != null &&
                    this.CurrentUser.Language != null &&
                    this.CurrentUser.Language.Enabled)
                    return this.CurrentUser.Language;

                var lang = gLanguageService.GetAllLanguages().FirstOrDefault();
                return lang;
            }
            set
            {
                if (this.CurrentUser == null)
                    return;

                this.CurrentUser.Language = value;
                gUserService.UpdateUser(this.CurrentUser);
            }
        }        

        public bool IsAdmin
        {
            get
            {
                return gCachedIsAdmin;
            }
            set
            {
                gCachedIsAdmin = value;
            }
        } 

        public User OriginalUserIfImpersonated
        {
            get { throw new NotImplementedException(); }
        }
    }
}
