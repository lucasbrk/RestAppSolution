using System;

namespace RestApp.Core.Data
{
    // TODO: Infraestructura distinta, agregado

    public partial class DataSettingsHelper
    {        
        private static bool? gDatabaseIsInstalled;
        public static bool DatabaseIsInstalled()
        {
            if (!gDatabaseIsInstalled.HasValue)
            {
                var manager = new DataSettingsManager();
                var settings = manager.LoadSettings();
                gDatabaseIsInstalled = settings != null && !String.IsNullOrEmpty(settings.DataConnectionString);
            }

            return gDatabaseIsInstalled.Value;
        }

        public static void ResetCache()
        {
            gDatabaseIsInstalled = null;
        }
    }
}
