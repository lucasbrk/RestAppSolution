using System.Data.Entity.ModelConfiguration;
using RestApp.Core.Domain.Tables;

namespace RestApp.Data.Mapping.Tables
{
    public class TableMap : EntityTypeConfiguration<Table>
    {
        public TableMap()
        {
            this.ToTable("Table");
            this.HasKey(t => t.Id);

            this.Property(t => t.Number).IsRequired();
            this.Property(t => t.Seat).IsRequired();
        }
    }
}
