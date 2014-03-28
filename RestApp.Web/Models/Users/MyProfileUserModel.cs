using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using RestApp.Web.Framework;
using System.Web.Mvc;

namespace RestApp.Web.Models.Users
{
    public class MyProfileUserModel
    {
        public MyProfileUserModel()
        {
            LanguagesList = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [RestAppResourceDisplayName("Model.User.Name")]
        public string Name { get; set; }

        [RestAppResourceDisplayName("Model.User.Login.LoginName")]
        public string LoginName { get; set; }

        [RestAppResourceDisplayName("Model.User.CurrentPassword")]
        public string CurrentPassword { get; set; }

        [RestAppResourceDisplayName("Model.User.NewPassword")]
        public string NewPassword { get; set; }

        [RestAppResourceDisplayName("Model.User.ConfirmPassword")]
        [AllowHtml]        
        public string ConfirmPassword { get; set; }

        [RestAppResourceDisplayName("Model.User.DocumentNumber")]
        public long DocumentNumber { get; set; }

        [RestAppResourceDisplayName("Model.User.Email")]
        public string Email { get; set; }

        [RestAppResourceDisplayName("Model.User.Roles")]
        public string RolesList { get; set; }

        [RestAppResourceDisplayName("Model.User.Language")]
        public int? LanguageId { get; set; }

        public IList<SelectListItem> LanguagesList { get; set; }
    }
}