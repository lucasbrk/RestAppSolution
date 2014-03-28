using System.ComponentModel.DataAnnotations;
using RestApp.Web.Framework;
using RestApp.Web.Framework.Mvc;

namespace RestApp.Web.Models.Users
{
    public class LoginModel : BaseApModel
    {
        [RestAppResourceDisplayName("Model.User.Login.LoginName")]
        public string LoginName { get; set; }

        public bool Enabled { get; set; }

        [RestAppResourceDisplayName("Model.User.Login.Password")]
        public string Password { get; set; }

        [RestAppResourceDisplayName("Model.User.Login.RememberMe")]
        public bool RememberMe { get; set; }
    }
}