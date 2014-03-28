using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestApp.Models.Roles;

namespace RestApp.Helpers
{
    public static class SecurityProvider
    {
        public enum PermissionUser
        {
            // 100 - 199
            UserAdd = 100,
            UserEdit = 101,
            UserDelete = 102,
            UserChangePassword = 103
        }

        public enum PermissionMainMenu
        {
            // 200 - 299
            MainMenuViewCounties = 200,
            MainMenuViewZones = 201,
            MainMenuViewAdmin = 202,
            MainMenuViewUsers = 203,
            MainMenuViewRoles = 204,
            MainMenuViewAddUser = 205,
            MainMenuViewAddRole = 206
        }

        public static bool CheckPermission(int pPermissionCode)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return false;
            }

            string vUserName = HttpContext.Current.User.Identity.Name;

            RoleModel vRole = new RoleModel();

            if (vRole.PermissionRecordsCategory != null
                // && vRole.Permissions.Contains("[" + pPermissionCode.ToString() + "]")
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static MainMenuSecurityModel GetMainMenuSecurity()
        {
            MainMenuSecurityModel vMainMenuSecurityModel = new MainMenuSecurityModel();

            vMainMenuSecurityModel.ViewAddRole = CheckPermission((int)PermissionMainMenu.MainMenuViewAddRole);
            vMainMenuSecurityModel.ViewAddUser = CheckPermission((int)PermissionMainMenu.MainMenuViewAddUser);
            vMainMenuSecurityModel.ViewAdmin = CheckPermission((int)PermissionMainMenu.MainMenuViewAdmin);
            vMainMenuSecurityModel.ViewCounties = CheckPermission((int)PermissionMainMenu.MainMenuViewCounties);
            vMainMenuSecurityModel.ViewRoles = CheckPermission((int)PermissionMainMenu.MainMenuViewRoles);
            vMainMenuSecurityModel.ViewUsers = CheckPermission((int)PermissionMainMenu.MainMenuViewUsers);
            vMainMenuSecurityModel.ViewZones = CheckPermission((int)PermissionMainMenu.MainMenuViewZones);

            return vMainMenuSecurityModel;
        }
    }
}