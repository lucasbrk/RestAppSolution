using System.Data.Entity.ModelConfiguration;
using RestApp.Core.Domain.ItemCategorys;

namespace RestApp.Data.Mapping.ItemCategorys
{
    public class ItemCategoryMap : EntityTypeConfiguration<ItemCategory>
    {
        public ItemCategoryMap()
        {
            this.ToTable("ItemCategory");
            this.HasKey(t => t.Id);

            this.Property(t => t.Name).IsRequired();
            this.Property(t => t.Description).IsOptional();
        }
    }
}
