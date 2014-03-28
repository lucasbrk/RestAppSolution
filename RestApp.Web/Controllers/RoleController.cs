using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestApp.Helpers;
using RestApp.Services.Roles;
using RestApp.Services.Localization;
using RestApp.Services.Security;
using RestApp.Models.Roles;
using RestApp.Core.Domain.Roles;
using RestApp.Core;
using RestApp.Web;
using RestApp.Core.Domain.Security;
using System.Text;
using RestApp.Web.Models.Security;
using RestApp.Core.Caching;
using RestApp.Common.Utility;

namespace RestApp.Web.Controllers
{
    [Authorize]
    public class RoleController : BaseApController
    {
        #region Fields
        
        private readonly IRoleService gRoleService;
        private readonly ILocalizationService gLocalizationService;
        private readonly IPermissionService gPermissionService;
        private readonly IWorkContext gWorkContext;

        #endregion

        #region Constructors

        public RoleController(IRoleService roleService, ILocalizationService localizationService, IPermissionService permissionService, IWorkContext WorkContext)
        {
            gRoleService = roleService;
            gLocalizationService = localizationService;
            gPermissionService = permissionService;
            gWorkContext = WorkContext;
        }

        #endregion

        #region Roles
        
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: /Role/
        public ActionResult List(string q)
        {            
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageRoles))
            {
                return AccessDeniedView();
            }
            
            List<ListRoleModel> listModel = new List<ListRoleModel>();

            foreach (var role in gRoleService.GetFilteredRoles(q))
            {
                listModel.Add(PrepareRoleModelForList(role));
            }

            if (q != null && q.Length == 1)
            {
                ViewBag.AlphabeticalLetter = q;
            }

            ViewBag.Letters = gRoleService.GetFirstLetterFilter();

            return View(listModel);
        }       

        // GET: /Role/Create
        public ActionResult Create()
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageRoles))
            {
                return AccessDeniedView();
            }

            var role = new RoleModel();

            role.PermissionRecordsCategory = ConvertListToModelPermissionRecords(null);

            return PartialView("_Create", role);
        }
       
        // POST: /Role/Create
        [HttpPost]
        public ActionResult Create(RoleModel model, FormCollection formData)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageRoles))
            {
                return AccessDeniedView();
            }
         
            if (ModelState.IsValid)
                {
                    var role = new Role()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Enabled = model.Enabled,

                        CreateBy = gWorkContext.CurrentUser.Id,
                        EditBy = gWorkContext.CurrentUser.Id,
                        DateCreateOn = DateTime.UtcNow,
                        DateEditOn = DateTime.UtcNow
                    };

                    role.PermissionRecords = ConvertToListPermissionRecords(formData.GetValues("Permissions"));
                    
                    gRoleService.InsertRole(role);

                    SuccessNotification(gLocalizationService.GetResource("Controller.Success.Role.Insert"));

                    return Json(new { success = "Valid" }, JsonRequestBehavior.AllowGet);
                }

            model.PermissionRecordsCategory = ConvertListToModelPermissionRecords(ConvertToListPermissionRecords(formData.GetValues("Permissions")));

            return Json(new { error = "Invalid" });
        }

        // GET: /Role/Edit/
        public ActionResult Edit(int id = 0)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageRoles))
            {
                return AccessDeniedView();
            }

            Role role = gRoleService.GetRoleById(id);
            if (role == null)
            {
                return HttpNotFound();
            }

            var roleModel = new RoleModel();

            roleModel.Id = role.Id;
            roleModel.Name = role.Name;
            roleModel.Description = role.Description;
            roleModel.Enabled = role.Enabled;
            roleModel.PermissionRecordsCategory = ConvertListToModelPermissionRecords(role.PermissionRecords);

            return PartialView("_Edit", roleModel);
        }

        // POST: /Role/Edit/
        [HttpPost]
        public ActionResult Edit(RoleModel model, FormCollection formData)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageRoles))
            {
                return AccessDeniedView();
            }

            var role = gRoleService.GetRoleById(model.Id);

            if (role == null)
            {
                 return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var PermissionRecordsList = ConvertToListPermissionRecords(formData.GetValues("Permissions")).Select(u => u.Id);

                    foreach (var permission in gPermissionService.GetAllPermissionRecords())
                    {
                        if (PermissionRecordsList.Contains(permission.Id))
                        {
                            if (role.PermissionRecords.Where(pr => pr.Id == permission.Id).Count() == 0)
                            {
                                role.PermissionRecords.Add(permission);
                            }
                        }
                        else
                        {
                            if (role.PermissionRecords.Where(pr => pr.Id == permission.Id).Count() != 0)
                            {
                                role.PermissionRecords.Remove(permission);
                            }
                        }
                    }

                    role.Name = model.Name;
                    role.Description = model.Description;
                    role.Enabled = model.Enabled;

                    role.CreateBy = gWorkContext.CurrentUser.Id;
                    role.EditBy = gWorkContext.CurrentUser.Id;
                    role.DateCreateOn = DateTime.UtcNow;
                    role.DateEditOn = DateTime.UtcNow;

                    gRoleService.UpdateRole(role);

                    SuccessNotification(gLocalizationService.GetResource("Controller.Success.Role.Update"));

                    // Cache
                    var cacheManager = new MemoryCacheManager();
                    cacheManager.Clear();

                    return RedirectToAction("Index");
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.ToString());
                }
               
            }

            model.PermissionRecordsCategory = ConvertListToModelPermissionRecords(ConvertToListPermissionRecords(formData.GetValues("Permissions")));

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageRoles))
            {
                return AccessDeniedView();
            }

            var custom = gRoleService.GetRoleById(id);
            if (custom == null)
            {
                return HttpNotFound();
            }

            gRoleService.DeleteRole(custom);

            SuccessNotification(gLocalizationService.GetResource("Controller.Success.Role.Delete"));

            return RedirectToAction("Index");
        }

        #endregion

        #region PermissionRecords

        public ActionResult PermissionRecordsAssigned(int id)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageRoles))
            {
                return AccessDeniedView();
            }

            Role role = gRoleService.GetRoleById(id);
            if (role == null)
            {
                return HttpNotFound();
            }

            var roleModel = new RoleModel();

            roleModel.Name = role.Name;
            roleModel.PermissionRecordsCategory = ConvertListToModelPermissionRecords(role.PermissionRecords);

            return PartialView("_PermissionRecordsAssigned", roleModel);
        }

        #endregion

        #region Utilities

        [NonAction]
        protected ICollection<PermissionRecord> ConvertToListPermissionRecords(string[] pData)
        {
            List<PermissionRecord> permissionRecordList = new List<PermissionRecord>();

            if (pData != null)
            {
                foreach (string permissionId in pData)
                {
                    permissionRecordList.Add(gPermissionService.GetPermissionRecordById(int.Parse(permissionId)));
                }
            }

            return permissionRecordList;
        }

        [NonAction]
        protected List<PermissionRecordCategoryModel> ConvertListToModelPermissionRecords(ICollection<PermissionRecord> pPermissionList)
        {
            if (pPermissionList == null)
            {
                pPermissionList = new List<PermissionRecord>();
            }

            var permissionCategoryList = new List<PermissionRecordCategoryModel>();

            foreach (var permission in gPermissionService.GetAllPermissionRecords())
            {
                PermissionRecordCategoryModel permissionRecordCategoryModel = permissionCategoryList.FirstOrDefault(u => u.Category == permission.Category);

                if (permissionRecordCategoryModel == null)
                {
                    permissionRecordCategoryModel = new PermissionRecordCategoryModel();
                    permissionRecordCategoryModel.Category = permission.Category;
                    permissionRecordCategoryModel.PermissionRecords = new List<PermissionRecordModel>();
                    permissionRecordCategoryModel.PermissionRecords.Add(new PermissionRecordModel()
                    {
                        Id = permission.Id,
                        Name = permission.Name,
                        Checked = pPermissionList.FirstOrDefault(u => u.Id == permission.Id) != null
                    });
                    permissionCategoryList.Add(permissionRecordCategoryModel);
                }
                else
                {
                    permissionRecordCategoryModel.PermissionRecords.Add(new PermissionRecordModel()
                    {
                        Id = permission.Id,
                        Name = permission.Name,
                        Checked = pPermissionList.FirstOrDefault(u => u.Id == permission.Id) != null
                    });
                }
            }

            return permissionCategoryList;
        }       

        [NonAction]
        protected ListRoleModel PrepareRoleModelForList(Role role)
        {
            return new ListRoleModel()
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Enabled = role.Enabled,
                PermissionRecords = GetPermissionRecords(role.PermissionRecords.ToList())
            };
        }

        [NonAction]
        protected string GetPermissionRecords(IList<PermissionRecord> permissionRecords, string separator = ",")
        {
            var sb = new StringBuilder();
            for (int i = 0; i < permissionRecords.Count; i++)
            {
                string permissionLocalizerName = gLocalizationService.GetResource("PermissionRecords." + permissionRecords[i].Category + "." + permissionRecords[i].Name);

                sb.Append(permissionLocalizerName);
                if (i != permissionRecords.Count - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        #endregion
    }
}