using System;

namespace RestApp.Core.Domain.ItemCategorys
{
    public class ItemCategory : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }
    }
}
