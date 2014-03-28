using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using RestApp.Core;
using RestApp.Core.Caching;
using RestApp.Core.Data;
using RestApp.Core.Domain.Localization;
using RestApp.Services;
using RestApp.Services.Events;
using RestApp.Services.Logging;

namespace RestApp.Services.Localization
{
    /// <summary>
    /// Provides information about localization
    /// </summary>
    public partial class LocalizationService : ILocalizationService
    {
        #region Constants
        private const string LOCALSTRINGRESOURCES_ALL_KEY = "RestApp.lsr.all-{0}";
        private const string LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY = "RestApp.lsr.{0}-{1}";
        private const string LOCALSTRINGRESOURCES_PATTERN_KEY = "RestApp.lsr.";
        #endregion

        #region Fields

        private readonly IRepository<LocaleStringResource> gLocalStringResourceRepository;
        private readonly IWorkContext gWorkContext;
        private readonly ILogger gLogger;
        private readonly ILanguageService gLanguageService;
        private readonly ICacheManager gCacheManager;
        private readonly IDataProvider gDataProvider;
        private readonly IDbContext gDbContext;
        private readonly IEventPublisher gEventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="logger">Logger</param>
        /// <param name="workContext">Work context</param>
        /// <param name="lsrRepository">Locale string resource repository</param>
        /// <param name="languageService">Language service</param>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="eventPublisher">Event published</param>
        public LocalizationService(ICacheManager cacheManager,
            ILogger logger, IWorkContext workContext,
            IRepository<LocaleStringResource> lsrRepository, 
            ILanguageService languageService,
            IDataProvider dataProvider, IDbContext dbContext, IEventPublisher eventPublisher)
        {
            this.gCacheManager = cacheManager;
            this.gLogger = logger;
            this.gWorkContext = workContext;
            this.gLocalStringResourceRepository = lsrRepository;
            this.gLanguageService = languageService;
            this.gDataProvider = dataProvider;
            this.gDbContext = dbContext;
            this.gDataProvider = dataProvider;
            this.gEventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public virtual void DeleteLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            gLocalStringResourceRepository.Delete(localeStringResource);

            //cache
            gCacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            //event notification
            gEventPublisher.EntityDeleted(localeStringResource);
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        /// <returns>Locale string resource</returns>
        public virtual LocaleStringResource GetLocaleStringResourceById(int localeStringResourceId)
        {
            if (localeStringResourceId == 0)
                return null;

            var localeStringResource = gLocalStringResourceRepository.GetById(localeStringResourceId);
            return localeStringResource;
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <returns>Locale string resource</returns>
        public virtual LocaleStringResource GetLocaleStringResourceByName(string resourceName)
        {
            if (gWorkContext.WorkingLanguage != null)
                return GetLocaleStringResourceByName(resourceName, gWorkContext.WorkingLanguage.Id);

            return null;
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <returns>Locale string resource</returns>
        public virtual LocaleStringResource GetLocaleStringResourceByName(string resourceName, int languageId,
            bool logIfNotFound = true)
        {
            var query = from lsr in gLocalStringResourceRepository.Table
                        orderby lsr.Name
                        where lsr.LanguageId == languageId && lsr.Name == resourceName
                        select lsr;
            var localeStringResource = query.FirstOrDefault();

            if (localeStringResource == null && logIfNotFound)
                gLogger.Warning(string.Format("Resource string ({0}) not found. Language ID = {1}", resourceName, languageId));
            return localeStringResource;
        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Locale string resources</returns>
        public virtual IList<LocaleStringResource> GetAllResources(int languageId)
        {
            var query = from l in gLocalStringResourceRepository.Table
                        orderby l.Name
                        where l.LanguageId == languageId
                        select l;
            var locales = query.ToList();
            return locales;
        }

        /// <summary>
        /// Inserts a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public virtual void InsertLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");
            
            gLocalStringResourceRepository.Insert(localeStringResource);

            //cache
            gCacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            //event notification
            gEventPublisher.EntityInserted(localeStringResource);
        }

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public virtual void UpdateLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            gLocalStringResourceRepository.Update(localeStringResource);

            //cache
            gCacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            //event notification
            gEventPublisher.EntityUpdated(localeStringResource);
        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Locale string resources</returns>
        public virtual Dictionary<string, KeyValuePair<int, string>> GetAllValues(int languageId)
        {
            string key = string.Format(LOCALSTRINGRESOURCES_ALL_KEY, languageId);
            return gCacheManager.Get(key, () =>
            {
                var query = from l in gLocalStringResourceRepository.Table
                            orderby l.Name
                            where l.LanguageId == languageId
                            select l;
                var locales = query.ToList();
                //format: <name, <id, value>>
                var dictionary = new Dictionary<string, KeyValuePair<int, string>>();
                foreach (var locale in locales)
                {
                    var resourceName = locale.Name.ToLowerInvariant();
                    if (!dictionary.ContainsKey(resourceName))
                        dictionary.Add(resourceName, new KeyValuePair<int, string>(locale.Id, locale.Value));
                }
                return dictionary;
            });
        }

        /// <summary>
        /// Gets all locale string resources by language identifier and filtered
        /// </summary>
        /// <param name="languageId, q">Language identifier and q</param>
        /// <returns>List Locale string resources</returns>
        public virtual IList<LocaleStringResource> GetAllResourceValuesFiltered(int languageId, string q)
        {
            var query = gLocalStringResourceRepository.Table
                            .Where(st => st.LanguageId==languageId);

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(st => st.Name.IndexOf(q) > -1 || st.Value.IndexOf(q) > -1);
            }

            return query.OrderBy(t => t.Name).ToList();
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <returns>A string representing the requested resource string.</returns>
        public virtual string GetResource(string resourceKey)
        {
            if (gWorkContext.WorkingLanguage != null)
                return GetResource(resourceKey, gWorkContext.WorkingLanguage.Id);
            
            return "";
        }
        
        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="returnEmptyIfNotFound">A value indicating whether to empty string will be returned if a resource is not found and default value is set to empty string</param>
        /// <returns>A string representing the requested resource string.</returns>
        public virtual string GetResource(string resourceKey, int languageId,
            bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false)
        {
            string result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();
            //if (_localizationSettings.LoadAllLocaleRecordsOnStartup)
            //{
                //load all records (we know they are cached)
                var resources = GetAllValues(languageId);
                if (resources.ContainsKey(resourceKey))
                {
                    result = resources[resourceKey].Value;
                }
            //}
            //else
            //{
            //    //gradual loading
            //    string key = string.Format(LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY, languageId, resourceKey);
            //    string lsr = _cacheManager.Get(key, () =>
            //    {
            //        var query = from l in _lsrRepository.Table
            //                    where l.Name == resourceKey
            //                    && l.LanguageId == languageId
            //                    select l.Value;
            //        return query.FirstOrDefault();
            //    });

            //    if (lsr != null) 
            //        result = lsr;
            //}
            if (String.IsNullOrEmpty(result))
            {
                if (logIfNotFound)
                    gLogger.Warning(string.Format("Resource string ({0}) is not found. Language ID = {1}", resourceKey, languageId));
                
                if (!String.IsNullOrEmpty(defaultValue))
                {
                    result = defaultValue;
                }
                else
                {
                    if (!returnEmptyIfNotFound)
                        result = resourceKey;
                }
            }
            return result;
        }

        /// <summary>
        /// Export language resources to xml
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportResourcesToXml(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");
            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Language");
            xmlWriter.WriteAttributeString("Name", language.Name);


            var resources = GetAllResources(language.Id);
            foreach (var resource in resources)
            {
                xmlWriter.WriteStartElement("LocaleResource");
                xmlWriter.WriteAttributeString("Name", resource.Name);
                xmlWriter.WriteElementString("Value", null, resource.Value);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        /// <summary>
        /// Import language resources from XML file
        /// </summary>
        /// <param name="language">Language</param>
        /// <param name="xml">XML</param>
        public virtual void ImportResourcesFromXml(Language language, string xml)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            if (String.IsNullOrEmpty(xml))
                return;
            //if (gDataProvider.StoredProceduredSupported)
            //{
            //    //SQL 2005 insists that your XML schema incoding be in UTF-16.
            //    //Otherwise, you'll get "XML parsing: line 1, character XXX, unable to switch the encoding"
            //    //so let's remove XML declaration
            //    var inDoc = new XmlDocument();
            //    inDoc.LoadXml(xml);
            //    var sb = new StringBuilder();
            //    using (var xWriter = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true }))
            //    {
            //        inDoc.Save(xWriter);
            //        xWriter.Close();
            //    }
            //    var outDoc = new XmlDocument();
            //    outDoc.LoadXml(sb.ToString());
            //    xml = outDoc.OuterXml;

            //    //stored procedures are enabled and supported by the database.
            //    var pLanguageId = gDataProvider.GetParameter();
            //    pLanguageId.ParameterName = "LanguageId";
            //    pLanguageId.Value = language.Id;
            //    pLanguageId.DbType = DbType.Int32;

            //    var pXmlPackage = gDataProvider.GetParameter();
            //    pXmlPackage.ParameterName = "XmlPackage";
            //    pXmlPackage.Value = xml;
            //    pXmlPackage.DbType = DbType.Xml;

            //    //long-running query. specify timeout (600 seconds)
            //    gDbContext.ExecuteSqlCommand("EXEC [LanguagePackImport] @LanguageId, @XmlPackage", 600, pLanguageId, pXmlPackage);
            //}
            //else
            //{
                //stored procedures aren't supported
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                var nodes = xmlDoc.SelectNodes(@"//Language/LocaleResource");
                foreach (XmlNode node in nodes)
                {
                    string name = node.Attributes["Name"].InnerText.Trim();
                    string value = "";
                    var valueNode = node.SelectSingleNode("Value");
                    if (valueNode != null)
                        value = valueNode.InnerText;

                    if (String.IsNullOrEmpty(name))
                        continue;

                    //do not use "Insert"/"Update" methods because they clear cache
                    //let's bulk insert
                    var resource = language.LocaleStringResources.Where(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (resource != null)
                        resource.Value = value;
                    else
                    {
                        language.LocaleStringResources.Add(
                            new LocaleStringResource()
                            {
                                Name = name,
                                Value = value
                            });
                    }
                }
                gLanguageService.UpdateLanguage(language);
            //}

            //clear cache
            gCacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
        }

        #endregion
    }
}
