using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RestApp.Web.Models.Users;
using RestApp.Web.Models.Security;
using RestApp.Core;
using RestApp.Services.Users;
using RestApp.Services.Localization;
using RestApp.Services.Logging;
using RestApp.Services.Security;
using RestApp.Web.Framework.Controllers;

namespace RestApp.Web.Controllers
{
    [Authorize]
    public partial class SecurityController : BaseApController
	{
		#region Fields

        private readonly ILogger gLogger;
        private readonly IWorkContext gWorkContext;
        private readonly IPermissionService gPermissionService;
        private readonly IUserService gUserService;
        private readonly ILocalizationService gLocalizationService;

		#endregion

		#region Constructors

        public SecurityController(ILogger logger, IWorkContext workContext,
            IPermissionService permissionService,
            IUserService userService, ILocalizationService localizationService)
		{
            this.gLogger = logger;
            this.gWorkContext = workContext;
            this.gPermissionService = permissionService;
            this.gUserService = userService;
            this.gLocalizationService = localizationService;
		}

		#endregion 

        #region Methods

        public ActionResult AccessDenied(string pageUrl)
        {
            var currentUser = gWorkContext.CurrentUser;
            if (currentUser == null)
            {
                gLogger.Information(string.Format("Access denied to anonymous request on {0}", pageUrl));
                return View();
            }

            gLogger.Information(string.Format("Access denied to user #{0} '{1}' on {2}", currentUser.LoginName, currentUser.LoginName, pageUrl));


            return View();
        }

        //public ActionResult Permissions()
        //{
        //    if (!gPermissionService.Authorize(StandardPermissionProvider.ManageAcl))
        //        return AccessDeniedView();

        //    var model = new PermissionMappingModel();

        //    var permissionRecords = gPermissionService.GetAllPermissionRecords();
        //    var userRoles = gUserService.GetAllUserRoles(true);
        //    foreach (var pr in permissionRecords)
        //    {
        //        model.AvailablePermissions.Add(new PermissionRecordModel()
        //        {
        //            Name = pr.Name,
        //        });
        //    }
        //    foreach (var cr in userRoles)
        //    {
        //        model.AvailableUserRoles.Add(new UserRoleModel()
        //        {
        //            Id = cr.Id,
        //            Name = cr.Name
        //        });
        //    }
        //    foreach (var pr in permissionRecords)
        //        foreach (var cr in userRoles)
        //        {
        //            bool allowed = pr.UserRoles.Where(x => x.Id == cr.Id).ToList().Count() > 0;
        //            if (!model.Allowed.ContainsKey(pr.Name))
        //                model.Allowed[pr.Name] = new Dictionary<int, bool>();
        //            model.Allowed[pr.Name][cr.Id] = allowed;
        //        }

        //    return View(model);
        //}

        //[HttpPost, ActionName("Permissions")]
        //public ActionResult PermissionsSave(FormCollection form)
        //{
        //    if (!gPermissionService.Authorize(StandardPermissionProvider.ManageAcl))
        //        return AccessDeniedView();

        //    var permissionRecords = gPermissionService.GetAllPermissionRecords();
        //    var userRoles = gUserService.GetAllUserRoles(true);


        //    foreach (var cr in userRoles)
        //    {
        //        string formKey = "allow_" + cr.Id;
        //        var permissionRecordNamesToRestrict = form[formKey] != null ? form[formKey].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

        //        foreach (var pr in permissionRecords)
        //        {

        //            bool allow = permissionRecordNamesToRestrict.Contains(pr.Name);
        //            if (allow)
        //            {
        //                if (pr.UserRoles.Where(x => x.Id == cr.Id).FirstOrDefault() == null)
        //                {
        //                    pr.UserRoles.Add(cr);
        //                    gPermissionService.UpdatePermissionRecord(pr);
        //                }
        //            }
        //            else
        //            {
        //                if (pr.UserRoles.Where(x => x.Id == cr.Id).FirstOrDefault() != null)
        //                {
        //                    pr.UserRoles.Remove(cr);
        //                    gPermissionService.UpdatePermissionRecord(pr);
        //                }
        //            }
        //        }
        //    }

        //    SuccessNotification(gLocalizationService.GetResource("Admin.Configuration.ACL.Updated"));
        //    return RedirectToAction("Permissions");
        //}
        #endregion
    }
}
