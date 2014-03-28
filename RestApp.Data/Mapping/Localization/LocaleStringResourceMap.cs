using System.Data.Entity.ModelConfiguration;
using RestApp.Core.Domain.Localization;

namespace RestApp.Services.Mapping.Localization
{
    public partial class LocaleStringResourceMap : EntityTypeConfiguration<LocaleStringResource>
    {
        public LocaleStringResourceMap()
        {
            this.ToTable("LocaleStringResource");
            this.HasKey(lsr => lsr.Id);
            this.Property(lsr => lsr.Name).IsRequired().HasMaxLength(200);
            this.Property(lsr => lsr.Value).IsRequired().IsMaxLength();


            this.HasRequired(lsr => lsr.Language)
                .WithMany(l => l.LocaleStringResources)
                .HasForeignKey(lsr => lsr.LanguageId);
        }
    }
}