using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestApp.Services.Localization;
using RestApp.Core.Infrastructure;

namespace RestApp.Web.Framework.Controllers
{
    public class HearderTextAttribute : Attribute
    {
        public string HeaderText { get; protected set; }
        public HearderTextAttribute(string header)
        {
            string headerStr = string.Empty;
            //localize header 
            ILocalizationService vLocalizationService = EngineContext.Current.Resolve<ILocalizationService>();
            headerStr = vLocalizationService.GetResource(header);
            //if not found in resource file, use the value that pass in  
            this.HeaderText = (string.Empty == headerStr) ? header : headerStr;
        }
    }
}
