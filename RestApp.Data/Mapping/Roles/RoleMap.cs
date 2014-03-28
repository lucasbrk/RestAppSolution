using System.Data.Entity.ModelConfiguration;
using RestApp.Core.Domain.Roles;

namespace RestApp.Services.Mapping.Roles
{
    public partial class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            this.ToTable("Role");
            this.HasKey(t => t.Id);
            this.Property(t => t.Name).IsRequired();
        }
    }
}
