using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using RestApp.Web.Framework;
using RestApp.Web.Models.Roles;

namespace RestApp.Web.Models.Users
{
    public class NewUserModel
    {
        public NewUserModel()
        {
            LanguageList = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [RestAppResourceDisplayName("Model.User.Name")]
        public string Name { get; set; }

        [RestAppResourceDisplayName("Model.User.Login.LoginName")]
        public string LoginName { get; set; }

        [DataType(DataType.Password)]
        [RestAppResourceDisplayName("Model.User.Password")]
        public string Password { get; set; }

        [AllowHtml]
        [RestAppResourceDisplayName("Model.User.ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [RestAppResourceDisplayName("Model.User.DocumentNumber")]
        public long DocumentNumber { get; set; }

        [RestAppResourceDisplayName("Model.User.Email")]
        public string Email { get; set; }

        [RestAppResourceDisplayName("Model.User.RolesList")]
        public List<RoleCheckListModel> RolesList { get; set; }

        [RestAppResourceDisplayName("Model.User.Language")]
        public int? LanguageId { get; set; }

        [RestAppResourceDisplayName("Model.User.Enabled")]
        public bool Enabled { get; set; }

        public IList<SelectListItem> LanguageList { get; set; }
    }
}