using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestApp.Services.Users;
using RestApp.Core.Caching;

namespace RestApp.Web.Controllers
{
    [Authorize]
    public class CommonController : BaseApController
    {
        #region Fields     


        #endregion

        #region Constructors       

        #endregion

        #region Methods       

        public ActionResult ClearCache()
        {
            var cacheManager = new MemoryCacheManager();
            cacheManager.Clear();

            return RedirectToAction("Index", "Home");
        }
       
        #endregion
    }
}
