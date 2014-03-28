using System.Collections.Generic;
using RestApp.Core.Domain.Roles;

namespace RestApp.Services.Roles
{
    /// <summary>
    /// Role service interface
    /// </summary>
    public partial interface IRoleService
    {
        /// <summary>
        /// Gets a Role
        /// </summary>
        /// <param name="taskId">Zone identifier</param>
        /// <returns>Zone</returns>
        Role GetRoleById(int roleId);

        /// <summary>
        /// Gets a Role by its type
        /// </summary>
        /// <param name="type">Role type</param>
        /// <returns>Role</returns>
        Role GetRoleByName(string name);

        /// <summary>
        /// Gets all Roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Roles</returns>
        IList<Role> GetAllRoles(bool showHidden = false);

        /// <summary>
        /// Deletes a Role
        /// </summary>
        /// <param name="role">Role</param>
        void DeleteRole(Role role);

        /// <summary>
        /// Inserts a Role
        /// </summary>
        /// <param name="role">Role</param>
        void InsertRole(Role role);

        /// <summary>
        /// Updates the Role
        /// </summary>
        /// <param name="role">Role</param>
        void UpdateRole(Role role);

        /// <summary>
        /// Get role filtered
        /// </summary>
        /// <param name="country">q</param>
        /// <returns>IList<Role></returns>
        IList<Role> GetFilteredRoles(string q);

        /// <summary>
        /// Get first letter Role
        /// </summary>
        /// <returns>string</returns>
        string GetFirstLetterFilter();
        
        /// <summary>
        /// Avaible Name
        /// </summary>
        /// <param name="name, id">Role Name and Id</param>
        /// <returns>bool</returns>
        bool IsNameAvailable(string name, int id);    
    }
}
