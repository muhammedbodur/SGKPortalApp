using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class IlceConfiguration : IEntityTypeConfiguration<Ilce>
    {
        public void Configure(EntityTypeBuilder<Ilce> builder)
        {
            builder.ToTable("CMN_Ilceler", "dbo");

            builder.HasKey(ic => ic.IlceId);

            builder.Property(ic => ic.IlceId)
                .ValueGeneratedOnAdd();

            builder.Property(ic => ic.IlceAdi)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ic => ic.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(ic => ic.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(ic => ic.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(ic => new { ic.IlId, ic.IlceAdi })
                .IsUnique()
                .HasDatabaseName("IX_CMN_Ilceler_Il_IlceAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(ic => !ic.SilindiMi);
        }
    }
}