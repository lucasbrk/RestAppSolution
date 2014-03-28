using System;
using RestApp.Core.Domain.Users;

namespace RestApp.Core.Domain.Logging
{
    //TODO: ver si usar
    /// <summary>
    /// Represents an activity log record
    /// </summary>
    public partial class ActivityLog : BaseEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the activity log type identifier
        /// </summary>
        public virtual int ActivityLogTypeId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public virtual int UserId { get; set; }

        /// <summary>
        /// Gets or sets the activity comment
        /// </summary>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the activity log type
        /// </summary>
        public virtual ActivityLogType ActivityLogType { get; set; }

        /// <summary>
        /// Gets the user
        /// </summary>
        public virtual User User { get; set; }

        #endregion
    }
}
    