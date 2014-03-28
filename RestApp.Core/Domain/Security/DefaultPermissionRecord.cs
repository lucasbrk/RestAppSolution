using System.Collections.Generic;

namespace RestApp.Core.Domain.Security
{
    /// <summary>
    /// Represents a default permission record
    /// </summary>
    public class DefaultPermissionRecord
    {
        public DefaultPermissionRecord() 
        {
            this.PermissionRecords = new List<PermissionRecord>();
        }

        /// <summary>
        /// Gets or sets the role system name
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Gets or sets the permissions
        /// </summary>
        public IEnumerable<PermissionRecord> PermissionRecords { get; set; }
    }
}
