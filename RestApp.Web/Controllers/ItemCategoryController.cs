using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestApp.Services.Localization;
using RestApp.Services.Security;
using RestApp.Core;
using RestApp.Services.ItemCategorys;
using RestApp.Web.Models.ItemsCategories;
using RestApp.Core.Domain.ItemCategorys;
using System.Data;
using RestApp.Common.Utility;

namespace RestApp.Web.Controllers
{
    [Authorize]
    public class ItemCategoryController : BaseApController
    {
		#region Fields

        private readonly ILocalizationService gLocalizationService;
        private readonly IPermissionService gPermissionService;
        private readonly IWorkContext gWorkContext;
        private readonly IItemCategoryService gItemCategoryService;

        #endregion

		#region Constructors

        public ItemCategoryController(ILocalizationService localizationService, IPermissionService permissionService, IWorkContext WorkContext, IItemCategoryService itemCategoryService)
        { 
            this.gLocalizationService = localizationService;
            this.gPermissionService = permissionService;
            this.gWorkContext = WorkContext;
            this.gItemCategoryService = itemCategoryService;
        }

        #endregion

		#region ItemCategory

		public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		// GET: /ItemCategory/
        public ActionResult List()
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageItemsCategories))
            {
                return AccessDeniedView();
            }

            List<ItemCategoryModel> listModel = new List<ItemCategoryModel>();

            foreach (var itemCategory in gItemCategoryService.GetAllItemCategorys())
            {
                listModel.Add(new ItemCategoryModel()
                {
                    Id = itemCategory.Id,
                    Name = itemCategory.Name,
                    Description = itemCategory.Description,
                    Enabled = itemCategory.Enabled
                });
            }

            ViewBag.SubNavActive = "Administration";

            return View(listModel);
        }

		// GET: /ItemCategory/Create
        public ActionResult Create()
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageItemsCategories))
            {
                return AccessDeniedView();
            }

            var itemCategory = new ItemCategoryModel();

            return PartialView("_Create", itemCategory);
        }

        // POST: /ItemCategory/Create
        [HttpPost]
        public ActionResult Create(ItemCategoryModel model)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageItemsCategories))
            {
                return AccessDeniedView();
            }

            if (ModelState.IsValid)
            {
                var itemCategory = new ItemCategory()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Enabled = model.Enabled,

                    CreateBy = gWorkContext.CurrentUser.Id,
                    EditBy = gWorkContext.CurrentUser.Id,
                    DateCreateOn = DateTime.UtcNow,
                    DateEditOn = DateTime.UtcNow
                };

                gItemCategoryService.InsertItemCategory(itemCategory);

                SuccessNotification(gLocalizationService.GetResource("Controller.Success.Common.Insert"));

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: /ItemCategory/Edit/
        public ActionResult Edit(int id = 0)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageItemsCategories))
            {
                return AccessDeniedView();
            }

            ItemCategory itemCategory = gItemCategoryService.GetItemCategoryById(id);
            if (itemCategory == null)
            {
                return HttpNotFound();
            }

            var itemCategoryModel = new ItemCategoryModel();

            itemCategoryModel.Id = itemCategory.Id;
            itemCategoryModel.Name = itemCategory.Name;
            itemCategoryModel.Description = itemCategory.Description;
            itemCategoryModel.Enabled = itemCategory.Enabled;

            return PartialView("_Edit", itemCategoryModel);
        }

        // POST: /ItemCategory/Edit/
        [HttpPost]
        public ActionResult Edit(ItemCategoryModel model)
        {
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageItemsCategories))
            {
                return AccessDeniedView();
            }

            var itemCategory = gItemCategoryService.GetItemCategoryById(model.Id);

            if (itemCategory == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    itemCategory.Name = model.Name;
                    itemCategory.Description = model.Description;
                    itemCategory.Enabled = model.Enabled;

                    itemCategory.CreateBy = gWorkContext.CurrentUser.Id;
                    itemCategory.EditBy = gWorkContext.CurrentUser.Id;
                    itemCategory.DateCreateOn = DateTime.UtcNow;
                    itemCategory.DateEditOn = DateTime.UtcNow;

                    gItemCategoryService.UpdateItemCategory(itemCategory);

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
            if (!gPermissionService.Authorize(StandardPermissionProvider.ManageItemsCategories))
            {
                return AccessDeniedView();
            }

            var itemCategory = gItemCategoryService.GetItemCategoryById(id);
            if (itemCategory == null)
            {
                return HttpNotFound();
            }

            gItemCategoryService.DeleteItemCategory(itemCategory);

            SuccessNotification(gLocalizationService.GetResource("Controller.Success.Common.Delete"));

            return RedirectToAction("Index");
        }

		#endregion

		#region Utilities

        #endregion
    }
}
