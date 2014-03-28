using RestApp.Core;
using RestApp.Core.Infrastructure;
using RestApp.Services.Localization;
using RestApp.Web.Framework.Mvc;

namespace RestApp.Web.Framework
{
    public class RestAppResourceDisplayName : System.ComponentModel.DisplayNameAttribute, IModelAttribute
    {
        private string gResourceValue = string.Empty;
        //private bool _resourceValueRetrived;

        public RestAppResourceDisplayName(string resourceKey)
            : base(resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public string ResourceKey { get; set; }

        public override string DisplayName
        {
            get
            {
                //do not cache resources because it causes issues when you have multiple languages
                //if (!_resourceValueRetrived)
                //{
                var langId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
                    gResourceValue = EngineContext.Current
                        .Resolve<ILocalizationService>()
                        .GetResource(ResourceKey, langId, true, ResourceKey);
                //    _resourceValueRetrived = true;
                //}
                return gResourceValue;
            }
        }

        public string Name
        {
            get { return "ApLigaWebResourceDisplayName"; }
        }
    }
}
