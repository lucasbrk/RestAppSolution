using System.Web.Mvc;
using System;

namespace RestApp.Web.Framework.Mvc
{
    public partial class BaseApModel
    {
        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }
    }
    public partial class BaseApEntityModel : BaseApModel
    {
        public virtual int Id { get; set; }
    }
}
