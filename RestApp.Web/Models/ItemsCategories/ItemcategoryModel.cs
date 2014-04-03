using System;
using System.Linq;
using RestApp.Web.Framework;

namespace RestApp.Web.Models.ItemsCategories
{
    public class ItemCategoryModel
    {
        public int Id { get; set; }

        [RestAppResourceDisplayName("Model.ItemCategory.Name")]
        public string Name { get; set; }

        [RestAppResourceDisplayName("Model.ItemCategory.Description")]
        public string Description { get; set; }

        [RestAppResourceDisplayName("Model.Common.Enabled")]
        public bool Enabled { get; set; }
    }
}