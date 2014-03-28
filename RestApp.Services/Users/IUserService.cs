using System;
using System.Collections.Generic;
using RestApp.Core;
using RestApp.Core.Domain.Users;
using RestApp.Core.Domain.Roles;

namespace RestApp.Services.Users
{
    /// <summary>
    /// User service interface
    /// </summary>
    public partial interface IUserService
    {
        //TODO: revisar Sumary
        #region Users       

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User</returns>
        IList<User> GetAllUsers(bool showHidden = false);
       

        /// <summary>
        /// Gets online users
        /// </summary>
        /// <param name="lastActivityFromUtc">User last activity date (from)</param>
        /// <param name="RoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>User collection</returns>
        IPagedList<User> GetOnlineUsers(DateTime lastActivityFromUtc,
            int[] RoleIds, int pageIndex, int pageSize);

        /// <summary>
        /// Gets all users by user role id
        /// </summary>
        /// <param name="RoleId">User role identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User collection</returns>
        IList<User> GetUsersByUserRoleId(int RoleId, bool showHidden = false);

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">User</param>
        void DeleteUser(User user);

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>A user</returns>
        User GetUserById(int userId);

        /// <summary>
        /// Get users by identifiers
        /// </summary>
        /// <param name="userIds">User identifiers</param>
        /// <returns>Users</returns>
        IList<User> GetUsersByIds(int[] userIds);

        /// <summary>
        /// Gets a user by GUID
        /// </summary>
        /// <param name="userGuid">User GUID</param>
        /// <returns>A user</returns>
        User GetUserByGuid(Guid userGuid);

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>User</returns>
        User GetUserByEmail(string email);
        
        /// <summary>
        /// Get user by system role
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>User</returns>
        User GetUserByRoleName(string roleName);

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User</returns>
        User GetUserByUserName(string username);

        /// <summary>
        /// Get user by loginName
        /// </summary>
        /// <param name="username">LoginName</param>
        /// <returns>User</returns>
        User GetUserByLoginName(string loginName);

        /// <summary>
        /// Get user by document number
        /// </summary>
        /// <param name="username">DocumentNumber</param>
        /// <returns>User</returns>
        User GetUserByDocumentNumber(long documentNumber);

        /// <summary>
        /// Get users by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Users</returns>
        IList<User> GetUsersByLanguageId(int languageId);

        /// <summary>
        /// Get a user by loginName return ID
        /// </summary>
        /// <param name="name">user type</param>
        /// <returns>int</returns>
        int GetUserIdByLoginName(string loginName);

        ///// <summary>
        ///// Insert a guest user
        ///// </summary>
        ///// <returns>User</returns>
        //User InsertGuestUser();

        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">User</param>
        void InsertUser(User user);

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">User</param>
        void UpdateUser(User user);
        
        /// <summary>
        /// Reset data required for checkout
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
        /// <param name="clearCheckoutAttributes">A value indicating whether to clear selected checkout attributes</param>
        /// <param name="clearRewardPoints">A value indicating whether to clear "Use reward points" flag</param>
        /// <param name="clearShippingMethod">A value indicating whether to clear selected shipping method</param>
        /// <param name="clearPaymentMethod">A value indicating whether to clear selected payment method</param>
        void ResetCheckoutData(User user,
            bool clearCouponCodes = false, bool clearCheckoutAttributes = false,
            bool clearRewardPoints = true, bool clearShippingMethod = true,
            bool clearPaymentMethod = true);

        ///// <summary>
        ///// Delete guest user records
        ///// </summary>
        ///// <param name="registrationFrom">User registration from; null to load all users</param>
        ///// <param name="registrationTo">User registration to; null to load all users</param>
        ///// <param name="onlyWithoutShoppingCart">A value indicating whether to delete users only without shopping cart</param>
        ///// <returns>Number of deleted users</returns>
        //int DeleteGuestUsers(DateTime? registrationFrom,
        //   DateTime? registrationTo, bool onlyWithoutShoppingCart);

        bool ValidateUser(string username, string password);

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="systemName">User role name</param>
        /// <returns>User role</returns>
        Role GetUserRoleByName(string name);

        /// <summary>
        /// Avaible LoginName
        /// </summary>
        /// <param name="loginName, id">User LoginName and Id</param>
        /// <returns>bool</returns>
        bool IsLoginNameAvailable(string loginName, int id);

        /// <summary>
        /// Avaible Name
        /// </summary>
        /// <param name="loginName, id">User Name and Id</param>
        /// <returns>bool</returns>
        bool IsNameAvailable(string name, int id);

        /// <summary>
        /// Avaible document number
        /// </summary>
        /// <param name="loginName, id">User DocumentNumber and Id</param>
        /// <returns>bool</returns>
        bool IsDocumentNumberAvailable(long documentNumber, int id);

        /// <summary>
        /// Avaible email
        /// </summary>
        /// <param name="loginName, id">User Email and Id</param>
        /// <returns>bool</returns>
        bool IsEmailAvailable(string email, int id);

        /// <summary>
        /// Verify Password from "MyProfile" save
        /// </summary>
        /// <param name="password">current password</param>
        /// <param name="id">user identifier</param>
        /// <returns></returns>
        bool IsPasswordEqual(string password, int id);

        /// <summary>
        /// Get user filtered
        /// </summary>
        /// <param name="country">q</param>
        /// <returns>IList<User></returns>
        IList<User> GetFilteredUsers(string q);

        /// <summary>
        /// Get first letter User
        /// </summary>
        /// <returns>string</returns>
        string GetFirstLetterFilter();

        #endregion

        #region User roles

        /// <summary>
        /// Delete a user role
        /// </summary>
        /// <param name="Role">User role</param>
        void DeleteUserRole(Role Role);

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="RoleId">User role identifier</param>
        /// <returns>User role</returns>
        Role GetUserRoleById(int RoleId);

        /// <summary>
        /// Gets all user roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User role collection</returns>
        IList<Role> GetAllRolesByUser(bool showHidden = false);

        /// <summary>
        /// Inserts a user role
        /// </summary>
        /// <param name="Role">User role</param>
        void InsertUserRole(Role Role);

        /// <summary>
        /// Updates the user role
        /// </summary>
        /// <param name="Role">User role</param>
        void UpdateUserRole(Role Role);

        bool CheckPermission(int pPermissionCode, string loginName);
        
        #endregion
    }
}