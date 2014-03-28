using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestApp.Web.Controllers;
using RestApp.Services.Localization;
using RestApp.Services.Security;
using RestApp.Core;

namespace RestApp.Web.Controllers
{
    public class AdministrationController : BaseApController
    {
		#region Fields

        private readonly ILocalizationService gLocalizationService;
        private readonly IPermissionService gPermissionService;
        private readonly IWorkContext gWorkContext;

        #endregion

		#region Constructors

        public AdministrationController(ILocalizationService localizationService, IPermissionService permissionService, IWorkContext workContext)
        { 
            this.gLocalizationService = localizationService;
            this.gPermissionService = permissionService;
            this.gWorkContext = workContext;
        }

        #endregion

		#region Administration

		public ActionResult Index()
        {
            ViewBag.SubNavActive = "Administration";
            return View();
        }

		#endregion

		#region Utilities
        
        #endregion
    }
}
