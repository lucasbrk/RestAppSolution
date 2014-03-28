using System.Web.Mvc;
using System.Web.Routing;
using RestApp.Web.Framework.Localization;
using RestApp.Web.Framework.Mvc.Routes;

namespace RestApp.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //home page
            routes.MapLocalizedRoute("Home",
                            "",
                            new { controller = "Home", action = "Index"},
                            new[] { "RestApp.Web.Controllers" });

            //login, register
            routes.MapLocalizedRoute("Login",
                            "Login/",
                            new { controller = "User", action = "Login" },
                            new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("LoginCheckoutAsGuest",
            //                "login/checkoutasguest",
            //                new { controller = "User", action = "Login", checkoutAsGuest = true },
            //                new[] { "RestApp.Web.Controllers" });
            routes.MapLocalizedRoute("Register",
                            "register/",
                            new { controller = "User", action = "Register" },
                            new[] { "RestApp.Web.Controllers" });
            routes.MapLocalizedRoute("Logout",
                            "logout/",
                            new { controller = "User", action = "Logout" },
                            new[] { "RestApp.Web.Controllers" });
            routes.MapLocalizedRoute("RegisterResult",
                            "registerresult/{resultId}",
                            new { controller = "User", action = "RegisterResult" },
                            new { resultId = @"\d+" },
                            new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("CheckUsernameAvailability",
            //                "user/checkusernameavailability",
            //                new { controller = "User", action = "CheckUsernameAvailability" },
            //                new[] { "RestApp.Web.Controllers" });

            ////passwordrecovery
            //routes.MapLocalizedRoute("PasswordRecovery",
            //                "passwordrecovery",
            //                new { controller = "User", action = "PasswordRecovery" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("PasswordRecoveryConfirm",
            //                "passwordrecovery/confirm",
            //                new { controller = "User", action = "PasswordRecoveryConfirm" },                            
            //                new[] { "RestApp.Web.Controllers" });
            ////user
            //routes.MapLocalizedRoute("UserMyAccount",
            //                "user/myaccount",
            //                new { controller = "User", action = "MyAccount" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("UserInfo",
            //                "user/info",
            //                new { controller = "User", action = "Info" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("UserAddresses",
            //                "user/addresses",
            //                new { controller = "User", action = "Addresses" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("UserOrders",
            //                "user/orders",
            //                new { controller = "User", action = "Orders" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("UserReturnRequests",
            //                "user/returnrequests",
            //                new { controller = "User", action = "ReturnRequests" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("UserDownloadableProducts",
            //                "user/downloadableproducts",
            //                new { controller = "User", action = "DownloadableProducts" },
            //                new[] { "RestApp.Web.Controllers" });


            //routes.MapLocalizedRoute("UserBackInStockSubscriptions",
            //                "user/backinstocksubscriptions",
            //                new { controller = "User", action = "BackInStockSubscriptions" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("UserBackInStockSubscriptionsPaged",
            //                "user/backinstocksubscriptions/{page}",
            //                new { controller = "User", action = "BackInStockSubscriptions", page = UrlParameter.Optional },
            //                new { page = @"\d+" },
            //                new[] { "RestApp.Web.Controllers" });

            //routes.MapLocalizedRoute("UserRewardPoints",
            //                "user/rewardpoints",
            //                new { controller = "User", action = "RewardPoints" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("UserChangePassword",
            //                "user/changepassword",
            //                new { controller = "User", action = "ChangePassword" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("UserAvatar",
            //                "user/avatar",
            //                new { controller = "User", action = "Avatar" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("AccountActivation",
            //                "user/activation",
            //                new { controller = "User", action = "AccountActivation" },                            
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("UserProfile",
            //                "profile/{id}",
            //                new { controller = "Profile", action = "Index" },
            //                new { id = @"\d+"},
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("UserProfilePaged",
            //                "profile/{id}/page/{page}",
            //                new { controller = "Profile", action = "Index"},
            //                new {  id = @"\d+", page = @"\d+" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("UserForumSubscriptions",
            //                "user/forumsubscriptions",
            //                new { controller = "User", action = "ForumSubscriptions"},
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("UserForumSubscriptionsPaged",
            //                "user/forumsubscriptions/{page}",
            //                new { controller = "User", action = "ForumSubscriptions", page = UrlParameter.Optional },
            //                new { page = @"\d+" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("DeleteForumSubscription",
            //                "user/forumsubscriptions/delete/{subscriptionId}",
            //                new { controller = "User", action = "DeleteForumSubscription" },
            //                new { subscriptionId = @"\d+" },
            //                new[] { "RestApp.Web.Controllers" });
            ////config
            //routes.MapLocalizedRoute("Config",
            //                "config",
            //                new { controller = "Common", action = "Config" },
            //                new[] { "RestApp.Web.Controllers" });

            ////some AJAX links
            //routes.MapRoute("GetStatesByCountryId",
            //                "country/getstatesbycountryid/",
            //                new { controller = "Country", action = "GetStatesByCountryId" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("ChangeDevice",
            //                "changedevice/{dontusemobileversion}",
            //                new { controller = "Common", action = "ChangeDevice" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("ChangeCurrency",
            //                "changecurrency/{usercurrency}",
            //                new { controller = "Common", action = "CurrencySelected" },
            //                new { usercurrency = @"\d+" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("ChangeLanguage",
            //                "changelanguage/{langid}",
            //                new { controller = "Common", action = "SetLanguage" },
            //                new { langid = @"\d+" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("ChangeTaxType",
            //                "changetaxtype/{usertaxtype}",
            //                new { controller = "Common", action = "TaxTypeSelected" },
            //                new { usertaxtype = @"\d+" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapRoute("EuCookieLawAccept",
            //                "eucookielawaccept",
            //                new { controller = "Common", action = "EuCookieLawAccept" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("PollVote",
            //                "poll/vote",
            //                new { controller = "Poll", action = "Vote" },
            //                new[] { "RestApp.Web.Controllers" });
            //routes.MapLocalizedRoute("TopicAuthenticate",
            //                "topic/authenticate",
            //                new { controller = "Topic", action = "Authenticate" },
            //                new[] { "RestApp.Web.Controllers" });

        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
