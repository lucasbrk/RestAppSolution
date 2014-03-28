using System.Collections.Generic;
using RestApp.Core.Domain.Users;
using RestApp.Core.Domain.Roles;

namespace RestApp.Core.Domain.Security
{
    /// <summary>
    /// Represents a permission record
    /// </summary>
    public class PermissionRecord : BaseEntity
    {
        private ICollection<Role> gRoles;

        /// <summary>
        /// Gets or sets the permission name
        /// </summary>
        public virtual string Name { get; set; }       
        
        /// <summary>
        /// Gets or sets the permission category
        /// </summary>
        public virtual string Category { get; set; }
        
        /// <summary>
        /// Gets or sets discount usage history
        /// </summary>
        public virtual ICollection<Role> Roles
        {
            get { return gRoles ?? (gRoles = new List<Role>()); }
            protected set { gRoles = value; }
        }   
    }
}
