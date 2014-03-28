using System;
using System.Linq;
using RestApp.Web.Framework;
using System.Collections.Generic;
using System.Web.Mvc;

namespace RestApp.Web.Models.Localization
{
    public class LanguageModel
    {
        public LanguageModel()
        {
            LanguagesCultureList = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [RestAppResourceDisplayName("Model.Configuration.Language.Name")]
        public  string Name { get; set; }

        [RestAppResourceDisplayName("Model.Configuration.Language.LanguageCulture")]
        public string LanguageCulture { get; set; }       

        [RestAppResourceDisplayName("Model.Configuration.Language.Enabled")]
        public bool Enabled { get; set; }

        public IList<SelectListItem> LanguagesCultureList { get; set; }
    }
}