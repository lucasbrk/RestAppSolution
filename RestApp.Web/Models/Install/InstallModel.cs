using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using RestApp.Web.Framework.Mvc;

namespace RestApp.Web.Models.Install
{
    public partial class InstallModel : BaseApModel
    {
        public InstallModel()
        {
            this.AvailableLanguages = new List<SelectListItem>();
        }
        [AllowHtml]
        public string AdminLoginName { get; set; }
        [AllowHtml]
        [DataType(DataType.Password)]
        public string AdminPassword { get; set; }
        [AllowHtml]
        [Compare("AdminPassword", ErrorMessage = "Sus contraseñas no son las mismas.")]
        public string ConfirmPassword { get; set; }        

        //MySql properties
        public string MySqlConnectionInfo { get; set; }
        [AllowHtml]
        public string MySqlServerName { get; set; }
        [AllowHtml]
        public string MySqlDatabaseName { get; set; }
        [AllowHtml]
        public string MySqlUsername { get; set; }
        [AllowHtml]
        public string MySqlPassword { get; set; }
        public bool MySqlServerCreateDatabase { get; set; }
        [AllowHtml]
        public string MySqlDatabaseConnectionString { get; set; }
        [AllowHtml]
        public string DataProvider { get; set; }
        public bool UseCustomCollation { get; set; }
        [AllowHtml]
        public string Collation { get; set; }
        
        public bool InstallSampleData { get; set; }
        public bool InstallTestData { get; set; }

        public List<SelectListItem> AvailableLanguages { get; set; }
    }
}