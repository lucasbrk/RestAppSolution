using System.Web;
using System.Web.Optimization;

namespace RestApp
{
    public class BundleConfig
    {
        // Para obtener más información acerca de Bundling, consulte http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //new
            #region Styles
            bundles.Add(new StyleBundle("~/Content/basic").Include(
                         "~/Content/css/bootstrap.css",
                         "~/Content/css/bootstrap-responsive.css",
                          "~/Content/css/font-awesome.css",
                         "~/Content/css/style.css"));

            bundles.Add(new StyleBundle("~/Content/signin").Include(
                        "~/Content/css/pages/signin.css"));

            bundles.Add(new StyleBundle("~/Content/shortcuts").Include(
                       "~/Content/css/pages/shortcuts.css")); 
            #endregion

            #region Scripts
            bundles.Add(new ScriptBundle("~/bundles/basic").Include(
                "~/Content/js/jquery-1.7.2.js",            
                "~/Content/js/bootstrap.js",
                "~/Content/js/base.js"));

            bundles.Add(new ScriptBundle("~/bundles/signin").Include(
               "~/Content/js/signin.js"));
            #endregion

            //old
            bundles.Add(new ScriptBundle("~/bundles/controlsToolKit").Include(
                           "~/Scripts/MVCControlToolkit.Controls-2.2.5.js"));

            bundles.Add(new ScriptBundle("~/bundles/extensions").Include(
                        "~/Scripts/jquery.blockUI.js"));

            bundles.Add(new ScriptBundle("~/bundles/listeditor").Include(
                        "~/Scripts/listEditor.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                         "~/Scripts/jquery.dataTables.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernui").Include(
                        //"~/Scripts/jquery.unobtrusive-ajax.min.js",
                        //"~/Scripts/jquery.unobtrusive-ajax.min.js",
                        //"~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/public.common.js"));
        }

    }
}