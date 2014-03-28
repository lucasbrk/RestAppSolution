using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace RestApp.Web.Models.Localization
{
    public class LocaleStringResourceModel
    {
        public int Id { get; set; }

        public int LanguageId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Language { get; set; }
    }
}