using RestApp.Core.Domain.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using RestApp.Core.Data;
using RestApp.Services.Events;


namespace RestApp.Services.Roles
{
    public partial class RoleService : IRoleService
    {
        #region Fields

        private readonly IRepository<Role> gRoleRepository;
        private readonly IEventPublisher gEventPublisher; 

        #endregion

        #region Ctor

        public RoleService(IRepository<Role> roleRepository, IEventPublisher eventPublisher)
        {
            this.gRoleRepository = roleRepository;
            this.gEventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a roles
        /// </summary>
        /// <param name="roleId">Role identifier</param>
        /// <returns>Role</returns>
        public virtual Role GetRoleById(int roleId)
        {
            if (roleId == 0)
                return null;

            return gRoleRepository.GetById(roleId);
        }

        /// <summary>
        /// Gets a role by name
        /// </summary>
        /// <param name="name">role name</param>
        /// <returns>Role</returns>
        public virtual Role GetRoleByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return null;

            var query = gRoleRepository.Table;
            query = query.Where(st => st.Name == name);
            query = query.OrderByDescending(t => t.Id);

            var role = query.FirstOrDefault();
            return role;
        }

        /// <summary>
        /// Gets all roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Role</returns>
        public virtual IList<Role> GetAllRoles(bool showHidden = false)
        {
            var query = gRoleRepository.Table;
            if (!showHidden)
            {
                query = query.Where(t => t.Enabled);
            }
            query = query.OrderByDescending(t => t.Id);

            var roles = query.ToList();
            return roles;
        }

        /// <summary>
        /// Inserts a role
        /// </summary>
        /// <param name="role">Role</param>
        public virtual void InsertRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("Role");

            gRoleRepository.Insert(role);

            //event notification
            gEventPublisher.EntityInserted(role);
        }

        /// <summary>
        /// Updates the role
        /// </summary>
        /// <param name="role">Role</param>
        public virtual void UpdateRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            gRoleRepository.Update(role);

            //event notification
            gEventPublisher.EntityUpdated(role);
        }

        /// <summary>
        /// Deletes a role
        /// </summary>
        /// <param name="role">Role</param>
        public virtual void DeleteRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            gRoleRepository.Delete(role);

            //event notification
            gEventPublisher.EntityDeleted(role);
        }

        /// <summary>
        /// Get role filtered
        /// </summary>
        /// <param name="country">q</param>
        /// <returns>IList<Role></returns>
        public IList<Role> GetFilteredRoles(string q)
        {
            var query = gRoleRepository.Table;

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
        /// Get first letter Role
        /// </summary>
        /// <returns>string</returns>
        public string GetFirstLetterFilter()
        {
            string letters = "";
            var query = gRoleRepository.Table.OrderBy(u => u.Name).Select(u => u.Name.Substring(0, 1)).Distinct();

            foreach (var letter in query)
            {
                letters += letter.ToUpper();
            }

            return letters;
        }
        
        /// <summary>
        /// Avaible Name
        /// </summary>
        /// <param name="name, id">Role Name and Id</param>
        /// <returns>bool</returns>
        public bool IsNameAvailable(string name, int id)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new Exception("Invalid Name");

            var query = gRoleRepository.Table
                        .Where(st => st.Name == name &&
                                     st.Id != id).FirstOrDefault();

            return query == null;
        }

        #endregion     
    }
}
