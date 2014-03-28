
using System.Data.Entity.ModelConfiguration;
using RestApp.Core.Domain.Users;

namespace RestApp.Services.Mapping.Users
{
    public partial class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            this.ToTable("User");
            this.HasKey(t => t.Id);
            this.Property(t => t.Name).IsRequired();
            this.Property(t => t.Email).HasMaxLength(1000);
            this.Property(t => t.Password);

            this.HasMany(t => t.Roles)
                .WithMany(t => t.Users)
                .Map(m => m.ToTable("User_Role_Mapping"));
        }
    }
}
