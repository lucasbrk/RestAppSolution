using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestApp.Core;
using RestApp.Web.Models.Security;
using RestApp.Services.Security;

namespace RestApp.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseApController
    {
        #region Fields

        private readonly IPermissionService gPermissionService;

        #endregion

        #region Constructors

        public HomeController(IPermissionService permissionService)
        { 
            this.gPermissionService=permissionService;
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }
    }
}
