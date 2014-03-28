using System.Collections.Generic;
using RestApp.Core.Domain.Localization;

namespace RestApp.Services.Localization
{
    /// <summary>
    /// Language service interface
    /// </summary>
    public partial interface ILanguageService
    {
        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="language">Language</param>
        void DeleteLanguage(Language language);

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Language collection</returns>
        IList<Language> GetAllLanguages(bool showHidden = false);

        Language GetLanguageByCultureCode(string CultureCode);

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language</returns>
        Language GetLanguageById(int languageId);

        /// <summary>
        /// Get language filtered
        /// </summary>
        /// <param name="q">q</param>
        /// <returns>IList<Language></returns>
        IList<Language> GetFilteredLanguages(string q);

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        void InsertLanguage(Language language);

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        void UpdateLanguage(Language language);
        
        /// <summary>
        /// Avaible Name
        /// </summary>
        /// <param name="name, id">Language Name and Id</param>
        /// <returns>bool</returns>
        bool IsNameAvailable(string name, int id);  
    }
}
