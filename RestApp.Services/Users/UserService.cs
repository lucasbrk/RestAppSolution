using System;
using System.Collections.Generic;
using System.Linq;
using RestApp.Core.Data;
using RestApp.Core.Domain.Users;
using RestApp.Services;
using RestApp.Core.Caching;
using RestApp.Services.Events;
using RestApp.Common.Utility;
using RestApp.Core.Domain.Roles;


namespace RestApp.Services.Users
{
    public partial class UserService : IUserService
    {
        #region Constants

        private const string USERROLES_ALL_KEY = "RestApp.userrole.all-{0}";
        private const string USERROLES_BY_ID_KEY = "RestApp.userrole.id-{0}";
        private const string USERROLES_BY_SYSTEMNAME_KEY = "RestApp.userrole.systemname-{0}";
        private const string USERROLES_PATTERN_KEY = "RestApp.userrole.";
        #endregion

        #region Fields

        private readonly IRepository<User> gUserRepository;
        private readonly IRepository<Role> gRoleRepository;
        private readonly ICacheManager gCacheManager;
        private readonly IEventPublisher gEventPublisher;

        #endregion

        #region Ctor

        public UserService(ICacheManager cacheManager,
            IRepository<User> userRepository,
            IRepository<Role> RoleRepository,
            IEventPublisher eventPublisher)
        {
            this.gCacheManager = cacheManager;
            this.gUserRepository = userRepository;
            this.gRoleRepository = RoleRepository;
            this.gEventPublisher = eventPublisher;
        }

        #endregion

        #region Methods        

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">USer identifier</param>
        /// <returns>User</returns>
        public virtual User GetUserById(int userId)
        {
            if (userId == 0)
                return null;

            return gUserRepository.GetById(userId);
        }

        /// <summary>
        /// Gets a user by its type
        /// </summary>
        /// <param name="name">user type</param>
        /// <returns>User</returns>
        public virtual User GetUserByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return null;

            var query = gUserRepository.Table;
            query = query.Where(st => st.Name == name);
            query = query.OrderByDescending(t => t.Id);

            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Gets a user by its type
        /// </summary>
        /// <param name="name">user type</param>
        /// <returns>User</returns>
        public virtual User GetUserByLoginName(string loginName)
        {
            if (String.IsNullOrWhiteSpace(loginName))
                return null;

            var query = gUserRepository.Table;
            query = query.Where(st => st.LoginName == loginName);
            query = query.OrderByDescending(t => t.Id);

            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Gets a user by its type
        /// </summary>
        /// <param name="name">user type</param>
        /// <returns>User</returns>
        public virtual User GetUserByDocumentNumber(long documentNumber)
        {
            var query = gUserRepository.Table;
            query = query.Where(st => st.DocumentNumber == documentNumber);
            query = query.OrderByDescending(t => t.Id);

            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Get a user by loginName return ID
        /// </summary>
        /// <param name="name">user type</param>
        /// <returns>int</returns>
        public virtual int GetUserIdByLoginName(string loginName)
        {
            if (String.IsNullOrWhiteSpace(loginName))
                throw new Exception("Invalid Login Name");

            var query = gUserRepository.Table;
            query = query.Where(st => st.LoginName == loginName);
            query = query.OrderByDescending(t => t.Id);

            var user = query.FirstOrDefault();
            return user.Id;
        }

        /// <summary>
        /// Get a user by email
        /// </summary>
        /// <param name="name">user type</param>
        /// <returns>User</returns>
        public User GetUserByEmail(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
                throw new Exception("Invalid e-mail");

            var query = gUserRepository.Table;
            query = query.Where(st => st.Email == email);
            query = query.OrderByDescending(t => t.Id);

            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Get roles by user
        /// </summary>
        /// <param name="name">user</param>
        /// <returns>User</returns>
        public IList<Role> GetAllRolesByUser(bool showHidden = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User</returns>
        public virtual IList<User> GetAllUsers(bool showHidden = false)
        {
            var query = gUserRepository.Table;
            if (!showHidden)
            {
                query = query.Where(t => t.Enabled);
            }
            query = query.OrderByDescending(t => t.Id);

            var users = query.ToList();
            return users;
        }

        /// <summary>
        /// Inserts a user
        /// </summary>
        /// <param name="user">User</param>
        public virtual void InsertUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            gUserRepository.Insert(user);

            //event notification
            gEventPublisher.EntityInserted(user);
        }

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">User</param>
        public virtual void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            gUserRepository.Update(user);

            //event notification
            gEventPublisher.EntityUpdated(user);
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="user">user</param>
        public virtual void DeleteUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            gUserRepository.Delete(user);

            //event notification
            gEventPublisher.EntityDeleted(user);
        }

        public virtual bool CheckPermission(int pPermissionCode, string loginName)
        {
            //User user = GetUserByLoginName(loginName);

            //if (user.Roles == null)
            //    return false;

            //string permission = user.Roles.Permissions;

            //if (permission != null && permission.Contains("[" + pPermissionCode.ToString() + "]"))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return true;
        }

        public virtual bool ValidateUser(string loginName, string password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(loginName))
                return false;

            string hash = EncryptionUtility.Encrypt(password.Trim());

            return this.GetAllUsers().Any(u => (u.LoginName == loginName.Trim()) && (u.Password == hash) && u.Enabled);
        }

        public bool IsLoginNameAvailable(string loginName, int id)
        {
            if (String.IsNullOrWhiteSpace(loginName))
                throw new Exception("Invalid LoginName");

            var query = gUserRepository.Table
                        .Where(st => st.LoginName == loginName && 
                                     st.Id != id).FirstOrDefault();

            return query == null;
        }

        public bool IsNameAvailable(string name, int id)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new Exception("Invalid Name");

            var query = gUserRepository.Table
                        .Where(st => st.Name == name &&
                                     st.Id != id).FirstOrDefault();

            return query == null;
        }

        public bool IsDocumentNumberAvailable(long documentNumber, int id)
        {
            //if (documentNumber == null)
            //    throw new Exception("Invalid documentNumber");

            var query = gUserRepository.Table
                        .Where(st => st.DocumentNumber == documentNumber &&
                                     st.Id != id).FirstOrDefault();

            return query == null;
        }

        public bool IsEmailAvailable(string email, int id)
        {           
            var query = gUserRepository.Table
                        .Where(st => st.Email == email &&
                                     st.Id != id).FirstOrDefault();

            return query == null;
        }

        public bool IsPasswordEqual(string password, int id)
        {
            User user = GetUserById(id);

            if (EncryptionUtility.Encrypt(password) == user.Password)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Get user filtered
        /// </summary>
        /// <param name="country">q</param>
        /// <returns>IList<USer></returns>
        public IList<User> GetFilteredUsers(string q)
        {
            var query = gUserRepository.Table;

            if (q != null)
            {
                if (q.Length == 1)
                {
                    query = query.Where(st => st.Name.StartsWith(q));
                }
                else if (q.Length > 1)
                {
                    query = query.Where(st => st.Name.IndexOf(q) > -1);
                }
            }

            return query.OrderBy(t => t.Name).ToList();
        }

        /// <summary>
        /// Get first letter User
        /// </summary>
        /// <returns>string</returns>
        public string GetFirstLetterFilter()
        {
            string letters = "";
            var query = gUserRepository.Table.OrderBy(u => u.Name).Select(u => u.Name.Substring(0, 1)).Distinct();

            foreach (var letter in query)
            {
                letters += letter.ToUpper();
            }

            return letters;
        }

        #endregion

        #region IUserService
        public Core.IPagedList<User> GetAllUsers(DateTime? registrationFrom, DateTime? registrationTo, int[] RoleIds, string email, string username, string firstName, string lastName, int dayOfBirth, int monthOfBirth, string company, string phone, string zipPostalCode, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Core.IPagedList<User> GetOnlineUsers(DateTime lastActivityFromUtc, int[] RoleIds, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetUsersByUserRoleId(int RoleId, bool showHidden = false)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetUsersByIds(int[] userIds)
        {
            throw new NotImplementedException();
        }

        public User GetUserByGuid(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                return null;

            var query = from c in gUserRepository.Table
                        where c.UserGuid == userGuid
                        orderby c.Id
                        select c;
            var user = query.FirstOrDefault();
            return user;
        }
       

        public User GetUserByUserName(string username)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetUsersByLanguageId(int languageId)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetUsersByCurrencyId(int currencyId)
        {
            throw new NotImplementedException();
        }

        //public User InsertGuestUser()
        //{
        //    throw new NotImplementedException();
        //}

        public void ResetCheckoutData(User user, bool clearCouponCodes = false, bool clearCheckoutAttributes = false, bool clearRewardPoints = true, bool clearShippingMethod = true, bool clearPaymentMethod = true)
        {
            throw new NotImplementedException();
        }

        //public int DeleteGuestUsers(DateTime? registrationFrom, DateTime? registrationTo, bool onlyWithoutShoppingCart)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        public Core.IPagedList<User> GetAllUsers(int[] RoleIds, string email, string name, string loginName)
        {
            throw new NotImplementedException();
        }

        public Role GetUserRoleByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return null;
            {
                var query = from cr in gRoleRepository.Table
                            orderby cr.Id
                            where cr.Name == name
                            select cr;
                var Role = query.FirstOrDefault();
                return Role;
            };
        }


        public void DeleteUserRole(Role Role)
        {
            throw new NotImplementedException();
        }

        public Role GetUserRoleById(int RoleId)
        {
            throw new NotImplementedException();
        }        

        public IList<Role> GetAllUserRoles(bool showHidden = false)
        {
            throw new NotImplementedException();
        }

        public void InsertUserRole(Role Role)
        {
            if (Role == null)
                throw new ArgumentNullException("Role");

            gRoleRepository.Insert(Role);

            gCacheManager.RemoveByPattern(USERROLES_PATTERN_KEY);

            //event notification
            gEventPublisher.EntityInserted(Role);
        }

        public void UpdateUserRole(Role Role)
        {
            throw new NotImplementedException();
        }


        public User GetUserByRoleName(string roleName)
        {
            throw new NotImplementedException();
        }

    }
}
