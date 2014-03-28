namespace RestApp.Core.Domain.Configuration
{
    /// <summary>
    /// Represents a setting
    /// </summary>
    public partial class Setting : BaseEntity
    {
        public Setting() { }


        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Returns the setting value as the specified type
        /// </summary>
        public virtual T As<T>()
        {
            return CommonHelper.To<T>(this.Value);
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
