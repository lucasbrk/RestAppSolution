using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using RestApp.Models.Roles;
using RestApp.Web.Framework;
using System.Web.Mvc;

namespace RestApp.Web.Models.Users
{
    public class ListUserModel
    {
        public int Id { get; set; }

        [RestAppResourceDisplayName("Model.User.Name")]
        public string Name { get; set; }

        [RestAppResourceDisplayName("Model.User.Login.LoginName")]
        public string LoginName { get; set; }

        [RestAppResourceDisplayName("Model.User.DocumentNumber")]
        public long DocumentNumber { get; set; }        

        [RestAppResourceDisplayName("Model.User.Roles")]
        public string Roles { get; set; }

        [RestAppResourceDisplayName("Model.User.Email")]
        public string Email { get; set; }

        [RestAppResourceDisplayName("Model.User.Enabled")]
        public bool Enabled { get; set; }
    }
}