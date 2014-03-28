using RestApp.Core.Domain.Users;

namespace RestApp.Services.Authentication
{
    /// <summary>
    /// Authentication service interface
    /// </summary>
    public partial interface IAuthenticationService 
    {
        void SignIn(User user, bool createPersistentCookie);

        void SignOut();

        User GetAuthenticatedUser();
    }
}