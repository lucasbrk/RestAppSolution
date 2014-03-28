using System.Web.Mvc;
using RestApp.Web.Framework;
using RestApp.Web.Framework.Mvc;

namespace RestApp.Web.Models.Localization
{
    public partial class ListLocaleStringResourceModel
    {
        public int Id { get; set; }

        [RestAppResourceDisplayName("Model.Configuration.Language.Resource.Name")]
        public string Name { get; set; }

        [RestAppResourceDisplayName("Model.Configuration.Language.Resource.Value")]
        public string Value { get; set; }

        [RestAppResourceDisplayName("Model.Configuration.Language.Resource.Language")]
        public string Language { get; set; }
    }
}