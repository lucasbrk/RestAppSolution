using System;
using System.Collections.Generic;
using System.Linq;
using RestApp.Core;
using RestApp.Core.Caching;
using RestApp.Core.Data;
using RestApp.Core.Domain.Users;
using RestApp.Core.Domain.Security;
using RestApp.Services.Users;
using RestApp.Core.Domain.Roles;

namespace RestApp.Services.Security
{
    /// <summary>
    /// Permission service
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        #region Constants
        /// <summary>
        /// Cache key for storing a valie indicating whether a certain user role has a permission
        /// </summary>
        /// <remarks>
        /// {0} : user role id
        /// {1} : permission system name
        /// </remarks>
        private const string PERMISSIONS_ALLOWED_KEY = "RestApp.permission.allowed-{0}-{1}";
        private const string PERMISSIONS_PATTERN_KEY = "RestApp.permission.";
        #endregion

        #region Fields

        private readonly IRepository<PermissionRecord> gPermissionRecordRepository;
        private readonly IUserService gUserService;
        private readonly IWorkContext gWorkContext;
        private readonly ICacheManager gCacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="permissionPecordRepository">Permission repository</param>
        /// <param name="userService">User service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="cacheManager">Cache manager</param>
        public PermissionService(IRepository<PermissionRecord> permissionPecordRepository,
            IUserService userService,
            IWorkContext workContext, ICacheManager cacheManager)
        {
            this.gPermissionRecordRepository = permissionPecordRepository;
            this.gUserService = userService;
            this.gWorkContext = workContext;
            this.gCacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordName">Permission record name</param>
        /// <param name="Role">User role</param>
        /// <returns>true - authorized; otherwise, false</returns>
        protected virtual bool Authorize(string permissionRecordName, Role Role)
        {
            if (String.IsNullOrEmpty(permissionRecordName))
                return false;
            
            string key = string.Format(PERMISSIONS_ALLOWED_KEY, Role.Id, permissionRecordName);
            return gCacheManager.Get(key, () =>
            {
                foreach (var permission1 in Role.PermissionRecords)
                    if (permission1.Name.Equals(permissionRecordName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void DeletePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            gPermissionRecordRepository.Delete(permission);

            gCacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetPermissionRecordById(int permissionId)
        {
            if (permissionId == 0)
                return null;

            return gPermissionRecordRepository.GetById(permissionId);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="name">Permission system name</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetPermissionRecordByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return null;

            var query = from pr in gPermissionRecordRepository.Table
                        where  pr.Name == name
                        orderby pr.Id
                        select pr;

            var permissionRecord = query.FirstOrDefault();
            return permissionRecord;
        }

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual IList<PermissionRecord> GetAllPermissionRecords()
        {
            var query = from pr in gPermissionRecordRepository.Table
                        orderby pr.Name
                        select pr;
            var permissions = query.ToList();
            return permissions;
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void InsertPermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            gPermissionRecordRepository.Insert(permission);

            gCacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void UpdatePermissionRecord(PermissionRecord permission)
        {
            if (permission == null)
                throw new ArgumentNullException("permission");

            gPermissionRecordRepository.Update(permission);

            gCacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void InstallPermissions(IPermissionProvider permissionProvider)
        {
            //install new permissions
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionRecordByName(permission.Name);
                if (permission1 == null)
                {
                    //new permission (install it)
                    permission1 = new PermissionRecord()
                    {
                        Name = permission.Name,
                        Category = permission.Category
                    };


                    //default user role mappings
                    var defaultPermissions = permissionProvider.GetDefaultPermissions();
                    foreach (var defaultPermission in defaultPermissions)
                    {
                        var Role = gUserService.GetUserRoleByName(defaultPermission.RoleName);
                        if (Role == null)
                        {
                            //new role (save it)
                            Role = new Role()
                            {
                                Name = defaultPermission.RoleName,
                                Enabled = true
                            };
                            gUserService.InsertUserRole(Role);
                        }


                        var defaultMappingProvided = (from p in defaultPermission.PermissionRecords
                                                      where p.Name == permission1.Name
                                                      select p).Any();
                        var mappingExists = (from p in Role.PermissionRecords
                                             where p.Name == permission1.Name
                                             select p).Any();
                        if (defaultMappingProvided && !mappingExists)
                        {
                            permission1.Roles.Add(Role);
                        }
                    }

                    //save new permission
                    InsertPermissionRecord(permission1);
                }
            }
        }

        /// <summary>
        /// Uninstall permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void UninstallPermissions(IPermissionProvider permissionProvider)
        {
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetPermissionRecordByName(permission.Name);
                if (permission1 != null)
                {
                    DeletePermissionRecord(permission1);
                }
            }
        }
        
        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(PermissionRecord permission)
        {
            return Authorize(permission, gWorkContext.CurrentUser);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(PermissionRecord permission, User user)
        {
            if (permission == null)
                return false;

            if (user == null)
                return false;

            //old implementation of Authorize method
            //var Roles = user.UserRoles.Where(cr => cr.Active);
            //foreach (var role in Roles)
            //    foreach (var permission1 in role.PermissionRecords)
            //        if (permission1.Name.Equals(permission.Name, StringComparison.InvariantCultureIgnoreCase))
            //            return true;

            //return false;

            return Authorize(permission.Name, user);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordName">Permission record name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(string permissionRecordName)
        {
            return Authorize(permissionRecordName, gWorkContext.CurrentUser);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordName">Permission record name</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(string permissionRecordName, User user)
        {
            if (String.IsNullOrEmpty(permissionRecordName))
                return false;

            var Roles = user.Roles.Where(cr => cr.Enabled);
            foreach (var role in Roles)
                if (Authorize(permissionRecordName, role))
                    //yes, we have such permission
                    return true;
            
            //no permission found
            return false;
        }

        #endregion

        public Dictionary<string, PermissionRecord> GetDictionaryPermissionRecord()
        {
            throw new NotImplementedException();
        }
    }
}