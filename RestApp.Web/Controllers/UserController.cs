using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RestApp.Helpers;
using RestApp.Core.Domain.Users;
using RestApp.Services.Users;
using RestApp.Web.Models.Users;
using RestApp.Services.Security;
using RestApp.Core;
using RestApp.Core.Domain.Roles;
using System.Text;
using RestApp.Services.Localization;
using RestApp.Web.Models.Roles;
using RestApp.Services.Roles;
using RestApp.Common.Utility;
using RestApp.Services.Authentication;

namespace RestApp.Web.Controllers
{
    [Authorize]
    public class UserController : BaseApController
    {
        #region Fields

        private readonly IUserService gUserService;
        private readonly UserSettings gUserSettings;
        private readonly IPermissionService gPermissionService;
        private readonly IWorkContext gWorkContext;
        private readonly ILocalizationService gLocalizationService;
        private readonly IRoleService gRoleService;
        private readonly ILanguageService gLanguageService;
        private readonly IAuthenticationService gAuthenticationService;

        #endregion

        #region Constructors

        public UserController(IUserService userService,UserSettings userSettings, IPermissionService permissionService,
        IWorkContext workContext, ILocalizationService localizationService, IRoleService roleService,
            ILanguageService languageService, IAuthenticationService authenticationService)
        {
            this.gUserService = userService;
            this.gUserSettings = userSettings;
            this.gPermissionService = permissionService;
            this.gWorkContext = workContext;
            this.gLocalizationService = localizationService;
            this.gRoleService = roleService;
            this.gLanguageService = languageService;
            this.gAuthenticationService = authenticationService;
        }

        #endregion

        #region Users
        
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: /User/
        public ActionResult List()
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageUsers))
            {
                return AccessDeniedView();
            }

            List<ListUserModel> listModel = new List<ListUserModel>();

            foreach (var user in gUserService.GetAllUsers())
            {
                listModel.Add(new ListUserModel()
                {
                    Id = user.Id,
                    Name = user.Name,
                    LoginName = user.LoginName,
                    DocumentNumber = user.DocumentNumber,
                    Roles = GetRoles(user.Roles.ToList()),
                    Email = user.Email,
                    Enabled = user.Enabled
                });
            }

            ViewBag.SubNavActive = "Administration";

            return View(listModel);
        }

        // GET: /User/Create
        public ActionResult Create()
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageUsers))
            {
                return AccessDeniedView();
            }

            var user = new NewUserModel();

            user.RolesList = ConvertToRolesChechListModel(null);

            user.LanguageList = LoadSelectListsItems();

            ViewBag.Action = this.ControllerContext.RouteData.Values["action"];

            return PartialView("_Create", user);
        }

        // POST: /Role/Create
        [HttpPost]
        public ActionResult Create(NewUserModel model, FormCollection formData)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageUsers))
            {
                return AccessDeniedView();
            }

            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    UserGuid = Guid.NewGuid(),
                    Name = model.Name,
                    LoginName = model.LoginName,
                    DocumentNumber = model.DocumentNumber,
                    Email = model.Email,
                    LanguageId = model.LanguageId,
                    Password = EncryptionUtility.Encrypt(model.Password),
                    Enabled = model.Enabled,

                    CreateBy = gWorkContext.CurrentUser.Id,
                    EditBy = gWorkContext.CurrentUser.Id,
                    DateCreateOn = DateTime.UtcNow,
                    DateEditOn = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow
                };

                user.Roles = ConvertToListRoles(formData.GetValues("Roles"));

                gUserService.InsertUser(user);

                SuccessNotification(gLocalizationService.GetResource("Controller.Success.Common.Insert"));

                return RedirectToAction("Index");
            }

            model.LanguageList = LoadSelectListsItems();

            model.RolesList = ConvertToRolesChechListModel(ConvertToListRoles(formData.GetValues("Permissions")));


            return View(model);
        }

        // GET: /User/Edit/5
        public ActionResult Edit(int id = 0)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageUsers))
            {
                return AccessDeniedView();
            }

            User user = gUserService.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userModel = new EditUserModel();

            userModel.Name = user.Name;
            userModel.LoginName = user.LoginName;
            userModel.DocumentNumber = user.DocumentNumber;
            userModel.Email = user.Email;
            userModel.LanguageId = user.LanguageId;
            userModel.RolesList = ConvertToRolesChechListModel(user.Roles);
            userModel.Enabled = user.Enabled;

            userModel.LanguageList = LoadSelectListsItems(user);

            ViewBag.Action = this.ControllerContext.RouteData.Values["action"];

            return PartialView("_Edit", userModel);
        }

        // POST: /User/Edit/5
        [HttpPost]
        public ActionResult Edit(EditUserModel model, FormCollection formData)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageUsers))
            {
                return AccessDeniedView();
            }

            var user = gUserService.GetUserById(model.Id);

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var RolesList = ConvertToListRoles(formData.GetValues("Roles")).Select(u => u.Id);

                    foreach (var role in gRoleService.GetAllRoles())
                    {
                        if (RolesList.Contains(role.Id))
                        {
                            if (user.Roles.Where(pr => pr.Id == role.Id).Count() == 0)
                            {
                                user.Roles.Add(role);
                            }
                        }
                        else
                        {
                            if (user.Roles.Where(pr => pr.Id == role.Id).Count() != 0)
                            {
                                user.Roles.Remove(role);
                            }
                        }
                    }

                    user.Name = model.Name;
                    user.LoginName = model.LoginName;
                    user.DocumentNumber = model.DocumentNumber;
                    user.Email = model.Email;
                    user.LanguageId = model.LanguageId;
                    user.Enabled = model.Enabled;

                    user.CreateBy = gWorkContext.CurrentUser.Id;
                    user.EditBy = gWorkContext.CurrentUser.Id;
                    user.DateCreateOn = DateTime.UtcNow;
                    user.DateEditOn = DateTime.UtcNow;

                    gUserService.UpdateUser(user);

                    SuccessNotification(gLocalizationService.GetResource("Controller.Success.Common.Update"));

                    return RedirectToAction("Index");
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.ToString());
                }
            }

            model.RolesList = ConvertToRolesChechListModel(ConvertToListRoles(formData.GetValues("Roles")));

            model.LanguageList = LoadSelectListsItems(user);

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageUsers))
            {
                return AccessDeniedView();
            }

            var custom = gUserService.GetUserById(id);
            if (custom == null)
            {
                return HttpNotFound();
            }

            gUserService.DeleteUser(custom);

            SuccessNotification(gLocalizationService.GetResource("Controller.Success.Common.Delete"));

            return RedirectToAction("Index");
        }

        public ActionResult ExportToExcel(string q)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageUsers))
            {
                return AccessDeniedView();
            }

            // Create new DataTable to hold the data
            DataTable data = new DataTable();

            // Create the columns to export
            data.Columns.Add(gLocalizationService.GetResource("View.User.Name"), Type.GetType("System.String"));
            data.Columns.Add(gLocalizationService.GetResource("View.User.LoginName"), Type.GetType("System.String"));
            data.Columns.Add(gLocalizationService.GetResource("View.User.DocumentNumber"), Type.GetType("System.Int64"));
            data.Columns.Add(gLocalizationService.GetResource("View.User.Email"), Type.GetType("System.String"));

            // Get the data
            foreach (var localeString in gUserService.GetFilteredUsers(q))
            {
                // Create new row with the table schema
                DataRow row = data.NewRow();

                // Add the data to each cell in row
                row[0] = localeString.Name;
                row[1] = localeString.LoginName;
                row[2] = localeString.DocumentNumber;
                row[3] = localeString.Email;

                // Add the new row to the rows collection
                data.Rows.Add(row);
            }

            // return the file to download
            return File(ExportUtility.ExportToExcel(data).ToArray(), // The binary data of the XLS file
                "application/vnd.ms-excel",                          // MIME type of Excel files
                "Export.xls");                                       // Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //// POST: /User/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                model.LoginName = model.LoginName.Trim();

                if (gUserService.ValidateUser(model.LoginName, model.Password))
                {
                    var user = gUserService.GetUserByLoginName(model.LoginName);
                    gAuthenticationService.SignIn(user, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            ModelState.AddModelError("", gLocalizationService.GetResource("Model.User.Login.LoginError"));
            return View(model);
        }

        //// GET: /User/LogOff
        public ActionResult LogOff()
        {
            gAuthenticationService.SignOut();

            return RedirectToAction("Index", "Home");
        }       

        // GET: /User/MyProfile/
        public ActionResult MyProfile(string returnUrl)
        {
            User user = gUserService.GetUserById(gWorkContext.CurrentUser.Id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userModel = new MyProfileUserModel();

            userModel.Id = user.Id;
            userModel.Name = user.Name;
            userModel.LoginName = user.LoginName;
            // string.Empty por seguridad para no mostrar el password en html
            userModel.CurrentPassword = string.Empty;
            userModel.NewPassword = string.Empty;
            userModel.ConfirmPassword = string.Empty;
            userModel.DocumentNumber = user.DocumentNumber;
            userModel.Email = user.Email;
            userModel.LanguageId = user.LanguageId;
            userModel.RolesList = GetRoles(user.Roles.ToList());
            userModel.LanguageId = user.LanguageId;

            userModel.LanguagesList = LoadSelectListsItems(user);

            ViewBag.ReturnUrl = returnUrl;

            return View(userModel);
        }

        [HttpPost]
        public ActionResult MyProfile(MyProfileUserModel model, string returnUrl)
        {
            var user = gUserService.GetUserById(model.Id);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Name = model.Name;
                    user.LoginName = model.LoginName;
                    if (!string.IsNullOrWhiteSpace(model.NewPassword))
                    {
                        user.Password = EncryptionUtility.Encrypt(model.NewPassword);
                    }
                    user.DocumentNumber = model.DocumentNumber;
                    user.Email = model.Email;
                    user.LanguageId = model.LanguageId;
                    user.EditBy = gWorkContext.CurrentUser.Id;
                    user.DateEditOn = DateTime.UtcNow;

                    gUserService.UpdateUser(user);

                    SuccessNotification(gLocalizationService.GetResource("Controller.Success.User.Update"));

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.ToString());
                }
            }

            model.RolesList = GetRoles(user.Roles.ToList());

            model.LanguagesList = LoadSelectListsItems(user);

            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }

        public ActionResult ChangePasswordAndPermission(int id)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageChangePasswordsAndPermissions))
            {
                return AccessDeniedView();
            }

            User user = gUserService.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userModel = new ChangePasswordAndPermissionModel()
            {
                Id = user.Id,
                Name = user.Name,
                LoginName = user.LoginName,
                RolesList = ConvertToRolesChechListModel(user.Roles)
            };

            return View(userModel);
        }

        [HttpPost]
        public ActionResult ChangePasswordAndPermission(ChangePasswordAndPermissionModel model, FormCollection formData)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageChangePasswordsAndPermissions))
            {
                return AccessDeniedView();
            }

            User user = gUserService.GetUserById(model.Id);
            if (user == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var RolesList = ConvertToListRoles(formData.GetValues("Roles")).Select(u => u.Id);

                    foreach (var role in gRoleService.GetAllRoles())
                    {
                        if (RolesList.Contains(role.Id))
                        {
                            if (user.Roles.Where(pr => pr.Id == role.Id).Count() == 0)
                            {
                                user.Roles.Add(role);
                            }
                        }
                        else
                        {
                            if (user.Roles.Where(pr => pr.Id == role.Id).Count() != 0)
                            {
                                user.Roles.Remove(role);
                            }
                        }
                    }

                    user.Password = EncryptionUtility.Encrypt(model.Password);                   

                    user.EditBy = gWorkContext.CurrentUser.Id;
                    user.DateEditOn = DateTime.UtcNow;

                    gUserService.UpdateUser(user);

                    SuccessNotification(gLocalizationService.GetResource("Controller.Success.User.Update"));

                    return RedirectToAction("Index");
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.ToString());
                }
            }

            model.RolesList = ConvertToRolesChechListModel(ConvertToListRoles(formData.GetValues("Roles")));

            return View(model);
        }


        #endregion

        #region Utilities
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [NonAction]
        protected string GetRoles(IList<Role> role, string separator = ",")
        {
            var sb = new StringBuilder();
            for (int i = 0; i < role.Count; i++)
            {
                sb.Append(role[i].Name);
                if (i != role.Count - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        [NonAction]
        protected List<RoleCheckListModel> ConvertToRolesChechListModel(ICollection<Role> pRoleList)
        {
            if (pRoleList == null)
            {
                pRoleList = new List<Role>();
            }

            var rolesList = new List<RoleCheckListModel>();

            foreach (var role in gRoleService.GetAllRoles(true))
            {

                rolesList.Add(new RoleCheckListModel()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Checked = pRoleList.FirstOrDefault(u => u.Id == role.Id) != null

                });
                
            }

            return rolesList;
        }

        [NonAction]
        protected ICollection<Role> ConvertToListRoles(string[] pData)
        {
            List<Role> rolesList = new List<Role>();

            if (pData != null)
            {
                foreach (string roleId in pData)
                {
                    rolesList.Add(gRoleService.GetRoleById(int.Parse(roleId)));
                }
            }

            return rolesList;
        }

        [NonAction]
        protected List<SelectListItem> LoadSelectListsItems(User user = null)
        {
            List<SelectListItem> genericList = new List<SelectListItem>();
            foreach (var l in gLanguageService.GetAllLanguages())
            {
                genericList.Add(new SelectListItem()
                {
                    Text = l.Name,
                    Value = l.Id.ToString(),
                    Selected = user != null ? l.Id == user.LanguageId : false
                });
            }

            return genericList;
        }

        #endregion
    }
}