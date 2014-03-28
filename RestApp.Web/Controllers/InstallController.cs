using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web.Hosting;
using System.Web.Mvc;
using RestApp.Core;
using RestApp.Core.Data;
using RestApp.Core.Infrastructure;
using RestApp.Services.Installation;
using RestApp.Web.Framework.Security;
using RestApp.Web.Infrastructure.Installation;
using RestApp.Web.Models.Install;
using RestApp.Services.Security;
using System.Web.Security;
using RestApp.Core.Caching;
using MySql.Data.MySqlClient;

namespace RestApp.Web.Controllers
{
    public partial class InstallController : BaseApController
    {
        #region Fields

        private readonly IInstallationLocalizationService gLocService;

        #endregion

        #region Ctor

        public InstallController(IInstallationLocalizationService locService)
        {
            this.gLocService = locService;
        }

        #endregion

        #region Utilities
        // MySQL
        private bool mySqlDatabaseExists(string connectionString)
        {
            try
            {
                //just try to connect
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string CreateMySqlDatabase(string connectionString)
        {
            try
            {
                //parse database name
                var builder = new MySqlConnectionStringBuilder(connectionString);
                var databaseName = builder.Database;
                //now create connection string to 'master' dabatase. It always exists.
                builder.Database = string.Empty; // = "master";
                var masterCatalogConnectionString = builder.ToString();
                string query = string.Format("CREATE DATABASE {0} COLLATE utf8_unicode_ci", databaseName);

                using (var conn = new MySqlConnection(masterCatalogConnectionString))
                {
                    conn.Open();
                    using (var command = new MySqlCommand(query, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Format("An error occured when creating database: {0}", ex.Message);
            }
        }

        private string CreateMySqlConnectionString(string serverName, string databaseName, string userName, string password, UInt32 timeout = 0)
        {
            var builder = new MySqlConnectionStringBuilder();
            builder.Server = serverName;
            builder.Database = databaseName.ToLower();
            builder.UserID = userName;
            builder.Password = password;
            builder.PersistSecurityInfo = false;
            builder.AllowUserVariables = true;
            builder.DefaultCommandTimeout = 30000;

            builder.ConnectionTimeout = int.MaxValue;
            return builder.ConnectionString;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            // Clear Cache
            var cacheManager = new MemoryCacheManager();
            cacheManager.Clear();

            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                return RedirectToRoute("HomePage");
            }

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var model = new InstallModel()
            {
                AdminLoginName = "admin",
                AdminPassword = "pass",
                ConfirmPassword = "pass",
                InstallSampleData = true,
                InstallTestData = false,
                MySqlDatabaseConnectionString = @"Server=127.0.0.1;Database=RestAppDB;Uid=root;Pwd=root;",
                DataProvider = "mysql",
                MySqlConnectionInfo = "sqlconnectioninfo_raw",
                MySqlServerCreateDatabase = false,
                UseCustomCollation = false,
                Collation = "SQL_Latin1_General_CP1_CI_AS",
            };
            foreach (var lang in gLocService.GetAvailableLanguages())
            {
                model.AvailableLanguages.Add(new SelectListItem()
                {
                    Value = Url.Action("ChangeLanguage", "Install", new { language = lang.Code}),
                    Text = lang.Name,
                    Selected = gLocService.GetCurrentLanguage().Code == lang.Code,
                });
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(InstallModel model)
        {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToRoute("HomePage");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            if (model.MySqlDatabaseConnectionString != null)
                model.MySqlDatabaseConnectionString = model.MySqlDatabaseConnectionString.Trim();

            //prepare language list
            foreach (var lang in gLocService.GetAvailableLanguages())
            {
                model.AvailableLanguages.Add(new SelectListItem()
                {
                    Value = Url.Action("ChangeLanguage", "Install", new { language = lang.Code }),
                    Text = lang.Name,
                    Selected = gLocService.GetCurrentLanguage().Code == lang.Code,
                });
            }
        
            if (model.MySqlConnectionInfo.Equals("sqlconnectioninfo_raw", StringComparison.InvariantCultureIgnoreCase))
            {
                //raw connection string
                if (string.IsNullOrEmpty(model.MySqlDatabaseConnectionString))
                    ModelState.AddModelError("", gLocService.GetResource("ConnectionStringRequired"));

                try
                {
                    //try to create connection string
                    new SqlConnectionStringBuilder(model.MySqlDatabaseConnectionString);
                }
                catch
                {
                    ModelState.AddModelError("", gLocService.GetResource("ConnectionStringWrongFormat"));
                }
            }
            else
            {
                //values
                if (string.IsNullOrEmpty(model.MySqlServerName))
                    ModelState.AddModelError("", gLocService.GetResource("SqlServerNameRequired"));
                if (string.IsNullOrEmpty(model.MySqlDatabaseName))
                    ModelState.AddModelError("", gLocService.GetResource("DatabaseNameRequired"));

                ////authentication type
                //if (model.SqlAuthenticationType.Equals("sqlauthentication", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    //SQL authentication
                //    if (string.IsNullOrEmpty(model.MySqlUsername))
                //        ModelState.AddModelError("", gLocService.GetResource("SqlServerUsernameRequired"));
                //    if (string.IsNullOrEmpty(model.MySqlPassword))
                //        ModelState.AddModelError("", gLocService.GetResource("SqlServerPasswordRequired"));
                //}
            }
            


            //Consider granting access rights to the resource to the ASP.NET request identity. 
            //ASP.NET has a base process identity 
            //(typically {MACHINE}\ASPNET on IIS 5 or Network Service on IIS 6 and IIS 7, 
            //and the configured application pool identity on IIS 7.5) that is used if the application is not impersonating.
            //If the application is impersonating via <identity impersonate="true"/>, 
            //the identity will be the anonymous user (typically IUSR_MACHINENAME) or the authenticated request user.
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            //validate permissions
            var dirsToCheck = FilePermissionHelper.GetDirectoriesWrite(webHelper);
            foreach (string dir in dirsToCheck)
                if (!FilePermissionHelper.CheckPermissions(dir, false, true, true, false))
                    ModelState.AddModelError("", string.Format(gLocService.GetResource("ConfigureDirectoryPermissions"), WindowsIdentity.GetCurrent().Name, dir));

            var filesToCheck = FilePermissionHelper.GetFilesWrite(webHelper);
            foreach (string file in filesToCheck)
                if (!FilePermissionHelper.CheckPermissions(file, false, true, true, true))
                    ModelState.AddModelError("", string.Format(gLocService.GetResource("ConfigureFilePermissions"), WindowsIdentity.GetCurrent().Name, file));
            
            if (ModelState.IsValid)
            {
                model.MySqlConnectionInfo = "sqlconnectioninfo_raw";

                var settingsManager = new DataSettingsManager();
                try
                {
                    string connectionString = null;

                        if (model.MySqlConnectionInfo.Equals("sqlconnectioninfo_raw", StringComparison.InvariantCultureIgnoreCase))
                        {
                            //raw connection string

                            //we know that MARS option is required when using Entity Framework
                            //let's ensure that it's specified
                            var sqlCsb = new SqlConnectionStringBuilder(model.MySqlDatabaseConnectionString);
                            connectionString = sqlCsb.ToString();
                        }
                        else
                        {
                            //values
                            connectionString = CreateMySqlConnectionString(model.MySqlServerName, model.MySqlDatabaseName, model.MySqlUsername, model.MySqlPassword);
                        }

                        if (model.MySqlServerCreateDatabase)
                        {
                            if (!mySqlDatabaseExists(connectionString))
                            {
                                //create database
                                var collation = model.UseCustomCollation ? model.Collation : "";
                                var errorCreatingDatabase = CreateMySqlDatabase(connectionString);
                                if (!String.IsNullOrEmpty(errorCreatingDatabase))
                                    throw new Exception(errorCreatingDatabase);
                                else
                                {
                                    //Database cannot be created sometimes. Weird! Seems to be Entity Framework issue
                                    //that's just wait 3 seconds
                                    Thread.Sleep(3000);
                                }
                            }
                        }
                        else
                        {
                            //check whether database exists
                            if (!mySqlDatabaseExists(connectionString))
                                throw new Exception(gLocService.GetResource("DatabaseNotExists"));
                        }
                                       

                    //save settings
                        //var dataProvider = model.DataProvider;
                    var settings = new DataSettings()
                    {
                        DataProvider = "mysql",
                        DataConnectionString = connectionString
                    };
                    settingsManager.SaveSettings(settings);

                    //init data provider
                    var dataProviderInstance = EngineContext.Current.Resolve<BaseDataProviderManager>().LoadDataProvider();
                    dataProviderInstance.InitDatabase();
                    
                    
                    //now resolve installation service
                    var installationService = EngineContext.Current.Resolve<IInstallationService>();
                    installationService.InstallData(model.AdminLoginName, model.AdminPassword, model.InstallTestData);

                    //reset cache
                    DataSettingsHelper.ResetCache();

                    //register default permissions
                    //var permissionProviders = EngineContext.Current.Resolve<ITypeFinder>().FindClassesOfType<IPermissionProvider>();
                    var permissionProviders = new List<Type>();
                    permissionProviders.Add(typeof(StandardPermissionProvider));
                    foreach (var providerType in permissionProviders)
                    {
                        dynamic provider = Activator.CreateInstance(providerType);
                        EngineContext.Current.Resolve<IPermissionService>().InstallPermissions(provider);
                    }

                    //restart application
                    webHelper.RestartAppDomain();

                    //Redirect to home page
                    return RedirectToRoute(new { contoller = "Home", action = "Index" });
                }
                catch (Exception exception)
                {
                    //reset cache
                    DataSettingsHelper.ResetCache();

                    //clear provider settings if something got wrong
                    settingsManager.SaveSettings(new DataSettings
                    {
                        DataProvider = null,
                        DataConnectionString = null
                    });

                    ModelState.AddModelError("", string.Format(gLocService.GetResource("SetupFailed"), exception.ToString()));
                }
            }
            return View(model);
        }

        public ActionResult ChangeLanguage(string language)
        {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToRoute("HomePage");

            gLocService.SaveCurrentLanguage(language);

            //Reload the page);
            return RedirectToAction("Index", "Install");
        }

        public ActionResult RestartInstall()
        {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToRoute("HomePage");
            
            //restart application
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            webHelper.RestartAppDomain();

            //Redirect to home page
            return RedirectToRoute("HomePage");
        }

        #endregion
    }
}
