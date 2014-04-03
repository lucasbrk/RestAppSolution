using System.Collections.Generic;
using RestApp.Core.Domain.Users;
using RestApp.Core.Domain.Security;

namespace RestApp.Services.Security
{
    public partial class StandardPermissionProvider : IPermissionProvider
    {
        //admin area permissions
        public static readonly PermissionRecord AccessPanelAdministration = new PermissionRecord { Name = "AccessPanelAdministration", Category = "Administration" };
        
        public static readonly PermissionRecord ManageRoles = new PermissionRecord { Name = "ManageRoles", Category = "Administration" };
        public static readonly PermissionRecord ManageUsers = new PermissionRecord { Name = "ManageUsers", Category = "Administration" };
        public static readonly PermissionRecord ManageChangePasswordsAndPermissions = new PermissionRecord { Name = "ManageChangePasswordsAndPermissions", Category = "Administration" };
        public static readonly PermissionRecord ManageTables = new PermissionRecord { Name = "ManageTables", Category = "Administration" };
        public static readonly PermissionRecord ManageItemsCategories = new PermissionRecord { Name = "ManageItemsCategories", Category = "Administration" };
        public static readonly PermissionRecord ManageSystemLogs = new PermissionRecord { Name = "ManageSystemLogs", Category = "Administration" };
        public static readonly PermissionRecord ManageLanguages = new PermissionRecord { Name = "ManageLanguages", Category = "Administration" };

        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[] 
            {
                AccessPanelAdministration,
                ManageRoles,
                ManageUsers,
                ManageChangePasswordsAndPermissions,
                ManageTables,
                ManageItemsCategories,
                ManageSystemLogs,
                ManageLanguages
            };
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[] 
            {
                new DefaultPermissionRecord 
                {
                    RoleName = "DefaultAdmin",
                    PermissionRecords = new[] 
                    {
                        AccessPanelAdministration,
                        ManageRoles,
                        ManageUsers,
                        ManageChangePasswordsAndPermissions,
                        ManageTables,
                        ManageItemsCategories,
                        ManageSystemLogs,
                        ManageLanguages
                    }
                },
            };
        }
    }
}