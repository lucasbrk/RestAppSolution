using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestApp.Web.Models.Security
{
    public class PermissionRecordCategoryModel
    {
        public List<PermissionRecordModel> PermissionRecords { get; set; }
        public string Category { get; set; }
    }
}