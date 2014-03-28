using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestApp.Services.Localization;
using RestApp.Services.Security;
using RestApp.Core;
using RestApp.Services.Tables;
using RestApp.Web.Models.Tables;
using RestApp.Core.Domain.Tables;
using System.Data;
using RestApp.Common.Utility;

namespace RestApp.Web.Controllers
{
    [Authorize]
    public class TableController : BaseApController
    {
		#region Fields

        private readonly ILocalizationService gLocalizationService;
        private readonly IPermissionService gPermissionService;
        private readonly IWorkContext gWorkContext;
        private readonly ITableService gTableService;

        #endregion

		#region Constructors

        public TableController(ILocalizationService localizationService, IPermissionService permissionService, IWorkContext WorkContext, ITableService tableService)
        { 
            this.gLocalizationService = localizationService;
            this.gPermissionService = permissionService;
            this.gWorkContext = WorkContext;
            this.gTableService = tableService;
        }

        #endregion

		#region Table

		public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		// GET: /Table/
        public ActionResult List()
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageTables))
            {
                return AccessDeniedView();
            }

            List<TableModel> listModel = new List<TableModel>();

            foreach (var table in gTableService.GetAllTables())
            {
                listModel.Add(new TableModel()
                {
                    Id = table.Id,
                    Number = table.Number,
                    Seat = table.Seat,
                    Enabled = table.Enabled
                });
            }

            ViewBag.SubNavActive = "Administration";

            return View(listModel);
        }

		// GET: /Table/Create
        public ActionResult Create()
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageTables))
            {
                return AccessDeniedView();
            }

            var table = new TableModel();

            return PartialView("_Create", table);
        }

        // POST: /Table/Create
        [HttpPost]
        public ActionResult Create(TableModel model)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageTables))
            {
                return AccessDeniedView();
            }

            if (ModelState.IsValid)
            {
                var table = new Table()
                {
                    Number = model.Number,
                    Seat = model.Seat,
                    Enabled = model.Enabled,

                    CreateBy = gWorkContext.CurrentUser.Id,
                    EditBy = gWorkContext.CurrentUser.Id,
                    DateCreateOn = DateTime.UtcNow,
                    DateEditOn = DateTime.UtcNow
                };

                gTableService.InsertTable(table);

                SuccessNotification(gLocalizationService.GetResource("Controller.Success.Common.Insert"));

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: /Table/Edit/
        public ActionResult Edit(int id = 0)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageTables))
            {
                return AccessDeniedView();
            }

            Table table = gTableService.GetTableById(id);
            if (table == null)
            {
                return HttpNotFound();
            }

            var tableModel = new TableModel();

            tableModel.Id = table.Id;
            tableModel.Number = table.Number;
            tableModel.Seat = table.Seat;
            tableModel.Enabled = table.Enabled;

            return PartialView("_Edit", tableModel);
        }

        // POST: /Table/Edit/
        [HttpPost]
        public ActionResult Edit(TableModel model)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageTables))
            {
                return AccessDeniedView();
            }

            var table = gTableService.GetTableById(model.Id);

            if (table == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    table.Number = model.Number;
                    table.Seat = model.Seat;
                    table.Enabled = model.Enabled;

                    table.CreateBy = gWorkContext.CurrentUser.Id;
                    table.EditBy = gWorkContext.CurrentUser.Id;
                    table.DateCreateOn = DateTime.UtcNow;
                    table.DateEditOn = DateTime.UtcNow;

                    gTableService.UpdateTable(table);

                    SuccessNotification(gLocalizationService.GetResource("Controller.Success.Common.Update"));

                    return RedirectToAction("Index");

                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.ToString());
                }

            }

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageTables))
            {
                return AccessDeniedView();
            }

            var table = gTableService.GetTableById(id);
            if (table == null)
            {
                return HttpNotFound();
            }

            gTableService.DeleteTable(table);

            SuccessNotification(gLocalizationService.GetResource("Controller.Success.Common.Delete"));

            return RedirectToAction("Index");
        }

		#endregion

		#region Utilities

        #endregion
    }
}
