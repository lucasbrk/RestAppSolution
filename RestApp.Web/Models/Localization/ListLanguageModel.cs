using RestApp.Web.Framework;

namespace RestApp.Web.Models.Localization
{
    public partial class ListLanguageModel
    {
        public int Id { get; set; }

        [RestAppResourceDisplayName("Model.Configuration.Language.Name")]
        public string Name { get; set; }

        [RestAppResourceDisplayName("Model.Configuration.Language.LanguageCulture")]
        public string LanguageCulture { get; set; }

        [RestAppResourceDisplayName("Model.Configuration.Language.Enabled")]
        public bool Enabled { get; set; }       
    }
}