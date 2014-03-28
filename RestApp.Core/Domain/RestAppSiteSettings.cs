using RestApp.Core.Configuration;

namespace RestApp.Core.Domain
{
    public class RestAppSiteSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a site name
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a site URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether site is closed
        /// </summary>
        public bool Closed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether administrators can visit a closed site
        /// </summary>
        public bool ClosedAllowForAdmins { get; set; }

    }
}
