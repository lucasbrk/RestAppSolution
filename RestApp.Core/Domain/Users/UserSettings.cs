
using RestApp.Core.Configuration;
using RestApp.Core.Domain.Users;

namespace RestApp.Core.Domain.Users
{
    public class UserSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a minimum password length
        /// </summary>
        public int PasswordMinLength { get; set; }
    }
}