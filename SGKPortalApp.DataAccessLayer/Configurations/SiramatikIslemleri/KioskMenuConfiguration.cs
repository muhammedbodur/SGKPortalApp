using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class KioskMenuConfiguration : IEntityTypeConfiguration<KioskMenu>
    {
        public void Configure(EntityTypeBuilder<KioskMenu> builder)
        {
            builder.ToTable("SIR_KioskMenuTanim", "dbo");

            builder.HasKey(kmt => kmt.KioskMenuId);

            builder.Property(kmt => kmt.KioskMenuId)
                .ValueGeneratedOnAdd();

            builder.Property(kmt => kmt.MenuAdi)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(kmt => kmt.Aciklama)
                .HasMaxLength(500);

            builder.Property(kmt => kmt.Aktiflik)
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif);

            builder.Property(kmt => kmt.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(kmt => kmt.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(kmt => kmt.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(kmt => kmt.MenuAdi)
                .IsUnique()
                .HasDatabaseName("IX_SIR_KioskMenuTanim_MenuAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(kmt => !kmt.SilindiMi);
        }
    }
}
