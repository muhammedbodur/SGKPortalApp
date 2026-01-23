using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class KioskConfiguration : IEntityTypeConfiguration<Kiosk>
    {
        public void Configure(EntityTypeBuilder<Kiosk> builder)
        {
            builder.ToTable("SIR_KioskTanim", "dbo");

            builder.HasKey(k => k.KioskId);

            builder.Property(k => k.KioskId)
                .ValueGeneratedOnAdd();

            builder.Property(k => k.KioskIp)
                .HasMaxLength(16);

            builder.Property(kt => kt.Aktiflik)
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif);

            builder.Property(kt => kt.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(kt => kt.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(kt => kt.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(kt => new { kt.DepartmanHizmetBinasiId, kt.KioskAdi })
                .IsUnique()
                .HasDatabaseName("IX_SIR_KioskTanim_DepartmanHizmetBinasi_KioskAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(kt => !kt.SilindiMi);

            builder.HasOne(kt => kt.DepartmanHizmetBinasi)
                .WithMany(dhb => dhb.Kiosklar)
                .HasForeignKey(kt => kt.DepartmanHizmetBinasiId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KioskTanim_CMN_DepartmanHizmetBinalari");

            builder.HasMany(kt => kt.MenuAtamalari)
                .WithOne(kma => kma.Kiosk)
                .HasForeignKey(kma => kma.KioskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SIR_KioskMenuAtama_SIR_KioskTanim");
        }
    }
}
