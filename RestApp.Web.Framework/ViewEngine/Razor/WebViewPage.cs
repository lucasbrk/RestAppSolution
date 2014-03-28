#region Using...

using System;
using System.IO;
using System.Web.Mvc;
using System.Web.WebPages;
using RestApp.Core;
using RestApp.Core.Data;
using RestApp.Core.Infrastructure;
using RestApp.Services.Localization;
using RestApp.Web.Framework.Localization;

#endregion

namespace RestApp.Web.Framework.ViewEngines.Razor
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {

        private ILocalizationService gLocalizationService;
        private Localizer gLocalizer;
        private IWorkContext gWorkContext;

        public Localizer T
        {
            get
            {
                if (gLocalizer == null)
                {
                    //null localizer
                    //_localizer = (format, args) => new LocalizedString((args == null || args.Length == 0) ? format : string.Format(format, args));

                    //default localizer
                    gLocalizer = (format, args) =>
                                     {
                                         var resFormat = gLocalizationService.GetResource(format);
                                         if (string.IsNullOrEmpty(resFormat))
                                         {
                                             return new LocalizedString(format);
                                         }
                                         return
                                             new LocalizedString((args == null || args.Length == 0)
                                                                     ? resFormat
                                                                     : string.Format(resFormat, args));
                                     };
                }
                return gLocalizer;
            }
        }

        public IWorkContext WorkContext
        {
            get
            {
                return gWorkContext;
            }
        }
        
        public override void InitHelpers()
        {
            base.InitHelpers();

            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                gLocalizationService = EngineContext.Current.Resolve<ILocalizationService>();
                gWorkContext = EngineContext.Current.Resolve<IWorkContext>();
            }
        }

        //public HelperResult RenderWrappedSection(string name, object wrapperHtmlAttributes)
        //{
        //    Action<TextWriter> action = delegate(TextWriter tw)
        //                        {
        //                            var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(wrapperHtmlAttributes);
        //                            var tagBuilder = new TagBuilder("div");
        //                            tagBuilder.MergeAttributes(htmlAttributes);

        //                            var section = RenderSection(name, false);
        //                            if (section != null)
        //                            {
        //                                tw.Write(tagBuilder.ToString(TagRenderMode.StartTag));
        //                                section.WriteTo(tw);
        //                                tw.Write(tagBuilder.ToString(TagRenderMode.EndTag));
        //                            }
        //                        };
        //    return new HelperResult(action);
        //}

        //public HelperResult RenderSection(string sectionName, Func<object, HelperResult> defaultContent)
        //{
        //    return IsSectionDefined(sectionName) ? RenderSection(sectionName) : defaultContent(new object());
        //}

        //public override string Layout
        //{
        //    get
        //    {
        //        var layout = base.Layout;

        //        if (!string.IsNullOrEmpty(layout))
        //        {
        //            var filename = System.IO.Path.GetFileNameWithoutExtension(layout);
        //            ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindView(ViewContext.Controller.ControllerContext, filename, "");

        //            if (viewResult.View != null && viewResult.View is RazorView)
        //            {
        //                layout = (viewResult.View as RazorView).ViewPath;
        //            }
        //        }

        //        return layout;
        //    }
        //    set
        //    {
        //        base.Layout = value;
        //    }
        //}
    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }
}