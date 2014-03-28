using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using RestApp.Web.Models.Security;
using RestApp.Web.Framework.Mvc;
using RestApp.Web.Framework;

namespace RestApp.Models.Roles
{
    public class RoleModel : BaseApModel
    {
        public int Id { get; set; }

        [RestAppResourceDisplayName("Model.Role.Name")]
        public string Name { get; set; }

        [RestAppResourceDisplayName("Model.Role.Description")]
        public string Description { get; set; }

        [RestAppResourceDisplayName("Model.Role.PermissionRecordsCategory")]
        public List<PermissionRecordCategoryModel> PermissionRecordsCategory { get; set; }

        [RestAppResourceDisplayName("Model.Role.Enabled")]
        public bool Enabled { get; set; }
    }
}