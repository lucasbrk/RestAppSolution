using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestApp.Services.Localization;
using RestApp.Services.Security;
using RestApp.Core;
using RestApp.Core.Domain.Localization;
using RestApp.Web.Models.Localization;
using MvcPaging;
using RestApp.Common.Utility;
using System.Data;
using System.IO;

namespace RestApp.Web.Controllers
{
    [Authorize]
    public class LanguageController : BaseApController
    {
		#region Fields

        private readonly ILocalizationService gLocalizationService;
        private readonly IPermissionService gPermissionService;
        private readonly IWorkContext gWorkContext;
        private readonly ILanguageService gLanguageService;

        #endregion

		#region Constructors

        public LanguageController(ILocalizationService localizationService, IPermissionService permissionService, IWorkContext WorkContext, ILanguageService languageService)
        { 
            this.gLocalizationService = localizationService;
            this.gPermissionService = permissionService;
            this.gWorkContext = WorkContext;
            this.gLanguageService = languageService;
        }

        #endregion

		#region Language

		public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		// GET: /Language/
        public ActionResult List(string q = "")
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageLanguages))
            {
                return AccessDeniedView();
            }

            List<ListLanguageModel> listModel = new List<ListLanguageModel>();

            foreach (var language in gLanguageService.GetFilteredLanguages(q))
            {
                listModel.Add(PrepareLanguageModelForList(language));
            }

            if (q.Length == 1)
            {
                ViewBag.AlphabeticalLetter = q;
            }

            return View(listModel);
        }
       

		// GET: /Language/Create
        public ActionResult Create()
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageLanguages))
            {
                return AccessDeniedView();
            }

            var language = new LanguageModel();

            foreach (var cultureInfo in System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures))
            {
                language.LanguagesCultureList.Add(new SelectListItem() { Text = string.Format("{0} ({1})", cultureInfo.Name, cultureInfo.DisplayName), Value = cultureInfo.Name });
            }

            return View(language);
        }

        // POST: /Language/Create
        [HttpPost]
        public ActionResult Create(LanguageModel model)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageLanguages))
            {
                return AccessDeniedView();
            }
           
            if (ModelState.IsValid)
            {
                var language = new Language()
                {
                    Name = model.Name,
                    LanguageCulture = model.LanguageCulture,
                    Enabled = model.Enabled,

                    CreateBy = gWorkContext.CurrentUser.Id,
                    EditBy = gWorkContext.CurrentUser.Id,
                    DateCreateOn = DateTime.UtcNow,
                    DateEditOn = DateTime.UtcNow
                };

                gLanguageService.InsertLanguage(language);

                SuccessNotification(gLocalizationService.GetResource("Controller.Success.Languaage.Insert"));

                return RedirectToAction("Index");
            }

            foreach (var cultureInfo in System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures))
            {
                model.LanguagesCultureList.Add(new SelectListItem() { Text = string.Format("{0} ({1})", cultureInfo.Name, cultureInfo.DisplayName), Value = cultureInfo.NativeName });
            }

            return View(model);
        }

        // GET: /Language/Edit/
        public ActionResult Edit(int id = 0)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageLanguages))
            {
                return AccessDeniedView();
            }

            Language language = gLanguageService.GetLanguageById(id);
            if (language == null)
            {
                return HttpNotFound();
            }

            
            var languageModel = new LanguageModel();
            
            foreach (var cultureInfo in System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures))
            {
                languageModel.LanguagesCultureList.Add(new SelectListItem() { Text = string.Format("{0} ({1})", cultureInfo.Name, cultureInfo.DisplayName), Value = cultureInfo.Name, Selected = cultureInfo.Name == language.LanguageCulture });
            }


            languageModel.Id = language.Id;
            languageModel.Name = language.Name;
            languageModel.LanguageCulture = language.LanguageCulture;
            languageModel.Enabled = language.Enabled;            

            return View(languageModel);
        }

        // POST: /Language/Edit/
        [HttpPost]
        public ActionResult Edit(LanguageModel model)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageLanguages))
            {
                return AccessDeniedView();
            }

            var language = gLanguageService.GetLanguageById(model.Id);

            if (language == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    model.Name = language.Name;
                    model.LanguageCulture = language.LanguageCulture;
                    model.Enabled = language.Enabled;

                    gLanguageService.UpdateLanguage(language);

                    SuccessNotification(gLocalizationService.GetResource("Controller.Success.Language.Update"));

                    return RedirectToAction("Index");
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.ToString());
                }

            }

            foreach (var cultureInfo in System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures))
            {
                model.LanguagesCultureList.Add(new SelectListItem() { Text = string.Format("{0} ({1})", cultureInfo.Name, cultureInfo.DisplayName), Value = cultureInfo.Name, Selected = cultureInfo.Name == model.Name });
            }

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageLanguages))
            {
                return AccessDeniedView();
            }

            var custom = gLanguageService.GetLanguageById(id);
            if (custom == null)
            {
                return HttpNotFound();
            }

            gLanguageService.DeleteLanguage(custom);

            SuccessNotification(gLocalizationService.GetResource("Controller.Success.Language.Delete"));

            return RedirectToAction("Index");
        }

        public ActionResult ExportToExcel(string q)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageLanguages))
            {
                return AccessDeniedView();
            }

            // Create new DataTable to hold the data
            DataTable data = new DataTable();

            // Create the columns to export
            data.Columns.Add(gLocalizationService.GetResource("View.Language.Name"), Type.GetType("System.String"));
            data.Columns.Add(gLocalizationService.GetResource("View.Language.LanguageCulture"), Type.GetType("System.String"));

            // Get the data
            foreach (var localeString in gLanguageService.GetFilteredLanguages(q))
            {
                // Create new row with the table schema
                DataRow row = data.NewRow();

                // Add the data to each cell in row
                row[0] = localeString.Name;
                row[1] = localeString.LanguageCulture;

                // Add the new row to the rows collection
                data.Rows.Add(row);
            }

            // return the file to download
            return File(ExportUtility.ExportToExcel(data).ToArray(), // The binary data of the XLS file
                "application/vnd.ms-excel",                          // MIME type of Excel files
                "Export.xls");                                       // Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

		#endregion

        #region Resource

        public ActionResult Resources(int Id, string q)
        {
            List<ListLocaleStringResourceModel> listModel = new List<ListLocaleStringResourceModel>();

            Language languague = gLanguageService.GetLanguageById(Id);
            if (languague == null)
            {
                RedirectToAction("Index");
            }

            foreach (var localeString in gLocalizationService.GetAllResourceValuesFiltered(Id, q))
            {
                listModel.Add(PrepareLocaleStringResourceModelForList(localeString));
            }

            ViewBag.LanguageName = languague.Name;
            ViewBag.LanguageId = languague.Id;

            return View(listModel);
        }

        public ActionResult ResourcesEdit(int id = 0)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageLanguages))
            {
                return AccessDeniedView();
            }

            LocaleStringResource localeStringResource = gLocalizationService.GetLocaleStringResourceById(id);
            if (localeStringResource == null)
            {
                return HttpNotFound();
            }

            var localeStringResourceModel = new LocaleStringResourceModel();

            localeStringResourceModel.Id = localeStringResource.Id;
            localeStringResourceModel.Name = localeStringResource.Name;
            localeStringResourceModel.Value = localeStringResource.Value;

            return PartialView("ResourcesEdit", localeStringResourceModel);
        }

        [HttpPost]
        public ActionResult ResourcesEdit(LocaleStringResourceModel model)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageLanguages))
            {
                return AccessDeniedView();
            }

            var resource = gLocalizationService.GetLocaleStringResourceById(model.Id);

            if (resource == null)
            {
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    resource.Value = model.Value;

                    resource.EditBy = gWorkContext.CurrentUser.Id;
                    resource.DateEditOn = DateTime.UtcNow;

                    gLocalizationService.UpdateLocaleStringResource(resource);

                    SuccessNotification(gLocalizationService.GetResource("Controller.Success.Language.LocaleStringResource.Update"));

                    return RedirectToAction("Resources", new { id = resource.LanguageId });

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.ToString());
                }

            }

            this.Response.StatusCode = 400;
            return PartialView("ResourcesEdit", model);
        }


        public ActionResult ExportToExcel(int id, string q)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageLanguages))
            {
                return AccessDeniedView();
            }

            Language languague = gLanguageService.GetLanguageById(id);
            if (languague == null)
            {
                return HttpNotFound();
            }
            
            // Create new DataTable to hold the data
            DataTable data = new DataTable();

            // Create the columns to export
            data.Columns.Add("Language", Type.GetType("System.String"));
            data.Columns.Add("Value", Type.GetType("System.String"));
            data.Columns.Add("Name", Type.GetType("System.String"));

            // Get the data
            foreach (var localeString in gLocalizationService.GetAllResourceValuesFiltered(id, q))
            {
                // Create new row with the table schema
                DataRow row = data.NewRow();

                // Add the data to each cell in row
                row[0] = localeString.Language.Name;
                row[1] = localeString.Value;
                row[2] = localeString.Name;

                // Add the new row to the rows collection
                data.Rows.Add(row);
            }

            // return the file to download
            return File(ExportUtility.ExportToExcel(data).ToArray(), // The binary data of the XLS file
                "application/vnd.ms-excel",                          // MIME type of Excel files
                "Export.xls");                                       // Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public ActionResult ResourceDelete(int id)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageLanguages))
            {
                return AccessDeniedView();
            }

            var custom = gLocalizationService.GetLocaleStringResourceById(id);
            if (custom == null)
            {
                return HttpNotFound();
            }

            gLocalizationService.DeleteLocaleStringResource(custom);

            SuccessNotification(gLocalizationService.GetResource("Controller.Success.Resourse.Delete"));

            return RedirectToAction("Index");
        }

        #endregion

        #region Utilities

        [NonAction]
        protected ListLanguageModel PrepareLanguageModelForList(Language language)
        {
            return new ListLanguageModel()
            {
                Id = language.Id,
                LanguageCulture = language.LanguageCulture,
                Name = language.Name,
                Enabled = language.Enabled
            };
        }

        [NonAction]
        protected ListLocaleStringResourceModel PrepareLocaleStringResourceModelForList(LocaleStringResource localeStringResource)
        {
            return new ListLocaleStringResourceModel()
            {
                Id = localeStringResource.Id,
                Language = localeStringResource.Language.Name,
                Value = localeStringResource.Value,
                Name = localeStringResource.Name
            };
        }
        
        #endregion
    }
}
