using System;
using System.Collections.Generic;
using RestApp.Core.Domain.Users;
using RestApp.Core.Domain.Security;

namespace RestApp.Core.Domain.Roles
{
    public class Role : BaseEntity
    {
        private ICollection<User> _users;

        private ICollection<PermissionRecord> gPermissionRecords;

        public virtual string Name { get; set; }

        public string Description { get; set; }

        public virtual bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the roles users
        /// </summary>
        public virtual ICollection<User> Users
        {
            get { return _users ?? (_users = new List<User>()); }
            protected set { _users = value; }
        }

        /// <summary>
        /// Gets or sets the roles users
        /// </summary>
        public virtual ICollection<PermissionRecord> PermissionRecords
        {
            get { return gPermissionRecords ?? (gPermissionRecords = new List<PermissionRecord>()); }
            set { gPermissionRecords = value; }
        }
    }
}