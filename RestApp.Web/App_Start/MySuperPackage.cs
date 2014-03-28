using System;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof(RestApp.Web.App_Start.MySuperPackage), "PreStart")]

namespace RestApp.Web.App_Start {
    public static class MySuperPackage {
        public static void PreStart() {
            MVCControlsToolkit.Core.Extensions.Register();
        }
    }
}