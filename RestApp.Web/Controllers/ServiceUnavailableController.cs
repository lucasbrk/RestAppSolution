using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestApp.Services.Localization;
using RestApp.Services.Security;
using RestApp.Core;

namespace RestApp.Web.Controllers
{
    [Authorize]
    public class ServiceUnavailableController : BaseApController
    {
        //
        // GET: /ServiceUnavailable/

        public ActionResult ServiceUnavailable()
        {
            return View();
        }
    }
}
