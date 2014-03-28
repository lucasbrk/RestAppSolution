using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestApp.Core.Domain.Logging
{
    public enum LogType
    {
        /// <summary>
        /// Common error log item type
        /// </summary>
        CommonError = 1,
        /// <summary>
        /// Mail error log item type
        /// </summary>
        MailError = 2,
        /// <summary>
        /// Administration area log item type
        /// </summary>
        AdministrationArea = 3,
        /// <summary>
        /// Application level err
        /// </summary>
        Application = 4,
        /// <summary>
        /// Language resource key err
        /// </summary>
        LanguageResource = 5,
        /// <summary>
        /// Language resource key err
        /// </summary>
        ConnectRegistration = 6,
        /// <summary>
        /// Web service err
        /// </summary>
        WebService = 7,
        /// <summary>
        /// Unknown log item type
        /// </summary>
        Unknown = 20,
    }

}
