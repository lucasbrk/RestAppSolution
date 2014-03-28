using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestApp.Web.Framework;
using RestApp.Web.Models.Roles;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RestApp.Web.Models.Users
{
    public class ChangePasswordAndPermissionModel
    {
        public int Id { get; set; }

        [RestAppResourceDisplayName("Model.User.RolesList")]
        public List<RoleCheckListModel> RolesList { get; set; }

        [DataType(DataType.Password)]
        [RestAppResourceDisplayName("Model.User.Password")]
        public string Password { get; set; }

        [AllowHtml]
        [RestAppResourceDisplayName("Model.User.ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [RestAppResourceDisplayName("Model.User.Name")]
        public string Name { get; set; }

        [RestAppResourceDisplayName("Model.User.Login.LoginName")]
        public string LoginName { get; set; }
    }
}