using System;
using System.Collections.Generic;
using System.Linq;
using RestApp.Core.Caching;
using RestApp.Core.Data;
using RestApp.Core.Domain.Localization;
using RestApp.Services.Configuration;
using RestApp.Services.Users;
using RestApp.Services.Events;

namespace RestApp.Services.Localization
{
    /// <summary>
    /// Language service
    /// </summary>
    public partial class LanguageService : ILanguageService
    {
        #region Constants
        private const string LANGUAGES_ALL_KEY = "RestApp.language.all-{0}";
        private const string LANGUAGES_BY_ID_KEY = "RestApp.language.id-{0}";
        private const string LANGUAGES_PATTERN_KEY = "RestApp.language.";
        #endregion

        #region Fields

        private readonly IRepository<Language> gLanguageRepository;
        private readonly IUserService gUserService;
        private readonly ICacheManager gCacheManager;
        private readonly ISettingService gSettingService;
        private readonly IEventPublisher gEventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="languageRepository">Language repository</param>
        /// <param name="userService">User service</param>
        /// <param name="settingService">Setting service</param>
        /// <param name="eventPublisher">Event published</param>
        public LanguageService(ICacheManager cacheManager,
            IRepository<Language> languageRepository,
            IUserService userService,
            ISettingService settingService,
            IEventPublisher eventPublisher)
        {
            this.gCacheManager = cacheManager;
            this.gLanguageRepository = languageRepository;
            this.gUserService = userService;
            this.gSettingService = settingService;
            this.gEventPublisher = eventPublisher;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void DeleteLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language"); 
            
            //update appropriate users (their language)
            //it can take a lot of time if you have thousands of associated users
            var users = gUserService.GetUsersByLanguageId(language.Id);
            foreach (var user in users)
            {
                user.LanguageId = null;
                gUserService.UpdateUser(user);
            }

            gLanguageRepository.Delete(language);

            //cache
            gCacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            gEventPublisher.EntityDeleted(language);
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Language collection</returns>
        public virtual IList<Language> GetAllLanguages(bool showHidden = false)
        {
            string key = string.Format(LANGUAGES_ALL_KEY, showHidden);
            return gCacheManager.Get(key, () =>
            {
                var query = gLanguageRepository.Table;
                if (!showHidden)
                    query = query.Where(l => l.Enabled);
                var languages = query.ToList();
                return languages;
            });
        }

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language</returns>
        public virtual Language GetLanguageById(int languageId)
        {
            if (languageId == 0)
                return null;

            string key = string.Format(LANGUAGES_BY_ID_KEY, languageId);
            return gCacheManager.Get(key, () =>
                                              {
                                                  return gLanguageRepository.GetById(languageId);
                                              });
        }

        /// <summary>
        /// Get language filtered
        /// </summary>
        /// <param name="q">q</param>
        /// <returns>IList<Language></returns>
         public IList<Language> GetFilteredLanguages(string q)
        {
            var query = gLanguageRepository.Table;

            if (q != null)
            {
                if (q.Length == 1)
                {
                    query = query.Where(st => st.Name.StartsWith(q));
                }
                else if (q.Length > 1)
                {
                    query = query.Where(st => st.Name.IndexOf(q) > -1);
                }
            }

            return query.OrderBy(t => t.Name).ToList();
        }

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void InsertLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            gLanguageRepository.Insert(language);

            //cache
            gCacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            gEventPublisher.EntityInserted(language);
        }

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void UpdateLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");
            
            //update language
            gLanguageRepository.Update(language);

            //cache
            gCacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            gEventPublisher.EntityUpdated(language);
        }

        public Language GetLanguageByCultureCode(string CultureCode)
        {
            return gLanguageRepository.Table.SingleOrDefault(p => p.LanguageCulture.Equals(CultureCode, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Avaible Name
        /// </summary>
        /// <param name="name, id">LAnguage Name and Id</param>
        /// <returns>bool</returns>
        public bool IsNameAvailable(string name, int id)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new Exception("Invalid Name");

            var query = gLanguageRepository.Table
                        .Where(st => st.Name == name &&
                                     st.Id != id).FirstOrDefault();

            return query == null;
        }

        #endregion
    }
}
