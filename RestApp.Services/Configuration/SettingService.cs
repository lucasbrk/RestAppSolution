using System;
using System.Collections.Generic;
using System.Linq;
using RestApp.Core;
using RestApp.Core.Caching;
using RestApp.Core.Configuration;
using RestApp.Core.Data;
using RestApp.Core.Domain.Configuration;
using RestApp.Core.Infrastructure;
using RestApp.Services.Events;
using RestApp.Common.Utility;

namespace RestApp.Services.Configuration
{
    /// <summary>
    /// Setting manager
    /// </summary>
    public partial class SettingService : ISettingService
    {
        #region Constants
        private const string SETTINGS_ALL_KEY = "RestApp.setting.all";
        #endregion

        #region Fields

        private readonly IRepository<Setting> gSettingRepository;
        private readonly IEventPublisher gEventPublisher;
        private readonly ICacheManager gCacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="settingRepository">Setting repository</param>
        public SettingService(ICacheManager cacheManager, IEventPublisher eventPublisher,
            IRepository<Setting> settingRepository)
        {
            this.gCacheManager = cacheManager;
            this.gEventPublisher = eventPublisher;
            this.gSettingRepository = settingRepository;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void InsertSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            gSettingRepository.Insert(setting);

            //cache
            if (clearCache)
                gCacheManager.RemoveByPattern(SETTINGS_ALL_KEY);

            //event notification
            gEventPublisher.EntityInserted(setting);
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            gSettingRepository.Update(setting);

            //cache
            if (clearCache)
                gCacheManager.RemoveByPattern(SETTINGS_ALL_KEY);

            //event notification
            gEventPublisher.EntityUpdated(setting);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Setting</returns>
        public virtual Setting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            var setting = gSettingRepository.GetById(settingId);
            return setting;
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Setting object</returns>
        public virtual Setting GetSettingByKey(string key)
        {
            if (String.IsNullOrEmpty(key))
                return null;

            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings();
            if (settings.ContainsKey(key))
            {
                var id = settings[key].Key;
                return GetSettingById(id);
            }

            return null;
        }

        public int GetSettingValueInteger(string name)
        {
            var setting = GetSettingByKey(name, "");
            return StringUtility.GetInt(setting);
        }

        public int GetSettingValueInteger(string name, int defaultValue)
        {
            var setting = GetSettingByKey(name, defaultValue.ToString());
            return StringUtility.GetInt(setting);
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T))
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings();
            if (settings.ContainsKey(key))
                return CommonHelper.To<T>(settings[key].Value);

            return defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void SetSetting<T>(string key, T value, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim().ToLowerInvariant();
            
            var settings = GetAllSettings();
            
            Setting setting = null;
            string valueStr = CommonHelper.GetApCustomTypeConverter(typeof(T)).ConvertToInvariantString(value);
            if (settings.ContainsKey(key))
            {
                //update
                var settingId = settings[key].Key;
                setting = GetSettingById(settingId);
                setting.Value = valueStr;
                UpdateSetting(setting, clearCache);
            }
            else
            {
                //insert
                setting = new Setting()
                              {
                                  Name = key,
                                  Value = valueStr,
                              };
                InsertSetting(setting, clearCache);
            }
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            gSettingRepository.Delete(setting);

            //cache
            gCacheManager.RemoveByPattern(SETTINGS_ALL_KEY);

            //event notification
            gEventPublisher.EntityDeleted(setting);
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        public virtual IDictionary<string, KeyValuePair<int, string>> GetAllSettings()
        {
            //cache
            string key = string.Format(SETTINGS_ALL_KEY);
            return gCacheManager.Get(key, () =>
            {
                var query = from s in gSettingRepository.Table
                            orderby s.Name
                            select s;
                var settings = query.ToList();
                //format: <name, <id, value>>
                var dictionary = new Dictionary<string, KeyValuePair<int, string>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    if (!dictionary.ContainsKey(resourceName))
                        dictionary.Add(resourceName, new KeyValuePair<int, string>(s.Id, s.Value));
                }
                return dictionary;
            });
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="settingInstance">Setting instance</param>
        public virtual void SaveSetting<T>(T settingInstance) where T : ISettings, new()
        {
            //We should be sure that an appropriate ISettings object will not be cached in IoC tool after updating (by default cached per HTTP request)
            EngineContext.Current.Resolve<IConfigurationProvider<T>>().SaveSettings(settingInstance);
        }

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public virtual void DeleteSetting<T>() where T : ISettings, new()
        {
            EngineContext.Current.Resolve<IConfigurationProvider<T>>().DeleteSettings();
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            gCacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }
        #endregion
    }
}