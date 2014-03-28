using System;
using RestApp.Core.Domain.Roles;
using RestApp.Core.Domain.Localization;
using System.Collections.Generic;

namespace RestApp.Core.Domain.Users
{
    public class User : BaseEntity
    {
        public User()
        {
            this.UserGuid = Guid.NewGuid();           
        }
        private ICollection<Role> _roles;

        public virtual Guid UserGuid { get; set; }

        public virtual string Name { get; set; }

        public virtual string LoginName { get; set; }

        public virtual long DocumentNumber { get; set; }

        public virtual string Password { get; set; }

        public virtual string Email { get; set; }        

        public virtual bool Enabled { get; set; }

        public virtual int? LanguageId { get; set; }

        public virtual Language Language { get; set; }

        /// <summary>
        /// Gets or sets the users roles
        /// </summary>
        public virtual ICollection<Role> Roles
        {
            get { return _roles ?? (_roles = new List<Role>()); }
            set { _roles = value; }
        }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public virtual DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public virtual DateTime LastActivityDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public virtual string LastIpAddress { get; set; }
    }
}