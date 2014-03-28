namespace RestApp.Core.Domain.Localization
{
    /// <summary>
    /// Represents a locale string resource
    /// </summary>
    public partial class LocaleStringResource : BaseEntity
    {
        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public virtual int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the resource name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the resource value
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Gets or sets the language
        /// </summary>
        public virtual Language Language { get; set; }
    }

}
