using RestApp.Web.Framework.Mvc;

namespace RestApp.Web.Models.Security
{
    public partial class PermissionRecordModel : BaseApModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }
    }
}