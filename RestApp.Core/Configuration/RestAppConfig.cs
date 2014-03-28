using System;
using System.Configuration;
using System.Xml;
using RestApp.Core.Infrastructure;

namespace RestApp.Core.Configuration
{
    /// <summary>
    /// Represents a RestAppConfig
    /// </summary>
    public partial class RestAppConfig : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new RestAppConfig();
            var dynamicDiscoveryNode = section.SelectSingleNode("DynamicDiscovery");
            if (dynamicDiscoveryNode != null && dynamicDiscoveryNode.Attributes != null)
            {
                var attribute = dynamicDiscoveryNode.Attributes["Enabled"];
                if (attribute != null)
                    config.DynamicDiscovery = Convert.ToBoolean(attribute.Value);
            }

            var engineNode = section.SelectSingleNode("Engine");
            if (engineNode != null && engineNode.Attributes != null)
            {
                var attribute = engineNode.Attributes["Type"];
                if (attribute != null)
                    config.EngineType = attribute.Value;
            }

            return config;
        }
        
        /// <summary>
        /// In addition to configured assemblies examine and load assemblies in the bin directory.
        /// </summary>
        public bool DynamicDiscovery { get; set; }

        /// <summary>
        /// A custom <see cref="IEngine"/> to manage the application instead of the default.
        /// </summary>
        public string EngineType { get; set; }
       
    }
}
