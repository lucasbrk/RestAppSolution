using System.Collections.Generic;

namespace RestApp.Core.Domain.Localization
{
    /// <summary>
    /// Represents a language
    /// </summary>
    public partial class Language : BaseEntity
    {
        private ICollection<LocaleStringResource> gLocaleStringResources;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the language culture
        /// </summary>
        public virtual string LanguageCulture { get; set; } 

        /// <summary>
        /// Gets or sets a value indicating whether the language is Enabled
        /// </summary>
        public virtual bool Enabled { get; set; }

        //TODO: ver si usar, o como usar
        /// <summary>
        /// Gets or sets locale string resources
        /// </summary>
        public virtual ICollection<LocaleStringResource> LocaleStringResources
        {
            get { return gLocaleStringResources ?? (gLocaleStringResources = new List<LocaleStringResource>()); }
            protected set { gLocaleStringResources = value; }
        }
    }
}
