using System.Data.Entity.ModelConfiguration;
using RestApp.Core.Domain.Localization;

namespace RestApp.Services.Mapping.Localization
{
    public partial class LanguageMap : EntityTypeConfiguration<Language>
    {
        public LanguageMap()
        {
            this.ToTable("Language");
            this.HasKey(l => l.Id);
            this.Property(l => l.Name).IsRequired().HasMaxLength(100);
            this.Property(l => l.LanguageCulture).IsRequired().HasMaxLength(20);

        }
    }
}
