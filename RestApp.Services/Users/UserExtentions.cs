using System;
using System.Linq;
using RestApp.Core.Domain.Users;
using RestApp.Core.Infrastructure;
using RestApp.Services.Localization;

namespace RestApp.Services.Users
{
    public static class UserExtentions
    {
        /// <summary>
        /// Gets a value indicating whether user is in a certain user role
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="RoleName">User role system name</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>Result</returns>
        public static bool IsInUserRole(this User user,
            string RoleName, bool onlyActiveUserRoles = true)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (String.IsNullOrEmpty(RoleName))
                throw new ArgumentNullException("RoleName");

            var result = user.Roles
                .Where(cr => !onlyActiveUserRoles || cr.Enabled)
                .Where(cr => cr.Name == RoleName)
                .FirstOrDefault() != null;
            return result;
        }

        ///// <summary>
        ///// Gets a value indicating whether user a search engine
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <returns>Result</returns>
        //public static bool IsSearchEngineAccount(this User user)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException("user");

        //    if (!user.IsSystemAccount || String.IsNullOrEmpty(user.Name))
        //        return false;

        //    var result = user.Name.Equals(SystemUserNames.SearchEngine, StringComparison.InvariantCultureIgnoreCase);
        //    return result;
        //}

        /// <summary>
        /// Gets a value indicating whether user is administrator
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>Result</returns>
        //public static bool IsAdmin(this User user, bool onlyActiveUserRoles = true)
        //{
        //    return IsInUserRole(user, SystemUserRoleNames.Administrators, onlyActiveUserRoles);
        //}

        ///// <summary>
        ///// Gets a value indicating whether user is a forum moderator
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        ///// <returns>Result</returns>
        //public static bool IsForumModerator(this User user, bool onlyActiveUserRoles = true)
        //{
        //    return IsInUserRole(user, SystemUserRoleNames.ForumModerators, onlyActiveUserRoles);
        //}

        ///// <summary>
        ///// Gets a value indicating whether user is registered
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        ///// <returns>Result</returns>
        //public static bool IsRegistered(this User user, bool onlyActiveUserRoles = true)
        //{
        //    return IsInUserRole(user, SystemUserRoleNames.Registered, onlyActiveUserRoles);
        //}

        ///// <summary>
        ///// Gets a value indicating whether user is guest
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        ///// <returns>Result</returns>
        //public static bool IsGuest(this User user, bool onlyActiveUserRoles = true)
        //{
        //    return IsInUserRole(user, SystemRoleNames.Guests, onlyActiveUserRoles);
        //}
        
        //public static string GetFullName(this User user)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException("user");
        //    var firstName = user.GetAttribute<string>(SystemUserAttributeNames.FirstName);
        //    var lastName = user.GetAttribute<string>(SystemUserAttributeNames.LastName);

        //    string fullName = "";
        //    if (!String.IsNullOrWhiteSpace(firstName) && !String.IsNullOrWhiteSpace(lastName))
        //        fullName = string.Format("{0} {1}", firstName, lastName);
        //    else
        //    {
        //        if (!String.IsNullOrWhiteSpace(firstName))
        //            fullName = firstName;

        //        if (!String.IsNullOrWhiteSpace(lastName))
        //            fullName = lastName;
        //    }
        //    return fullName;
        //}

        ///// <summary>
        ///// Formats the user name
        ///// </summary>
        ///// <param name="user">Source</param>
        ///// <returns>Formatted text</returns>
        //public static string FormatUserName(this User user)
        //{
        //    return FormatUserName(user, false);
        //}

        ///// <summary>
        ///// Formats the user name
        ///// </summary>
        ///// <param name="user">Source</param>
        ///// <param name="stripTooLong">Strip too long user name</param>
        ///// <returns>Formatted text</returns>
        //public static string FormatUserName(this User user, bool stripTooLong)
        //{
        //    if (user == null)
        //        return string.Empty;

        //    if (user.IsGuest())
        //    {
        //        return EngineContext.Current.Resolve<ILocalizationService>().GetResource("User.Guest");
        //    }

        //    string result = string.Empty;
        //    switch (EngineContext.Current.Resolve<UserSettings>().UserNameFormat)
        //    {
        //        case UserNameFormat.ShowEmails:
        //            result = user.Email;
        //            break;
        //        case UserNameFormat.ShowFullNames:
        //            result = user.GetFullName();
        //            break;
        //        case UserNameFormat.ShowUsernames:
        //            result = user.Username;
        //            break;
        //        default:
        //            break;
        //    }

        //    if (stripTooLong)
        //    {
        //        int maxLength = 0; // TODO make this setting configurable
        //        if (maxLength > 0 && result.Length > maxLength)
        //        {
        //            result = result.Substring(0, maxLength);
        //        }
        //    }

        //    return result;
        //}

    }
}
