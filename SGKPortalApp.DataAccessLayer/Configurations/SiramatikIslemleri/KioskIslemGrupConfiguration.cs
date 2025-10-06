using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class KioskIslemGrupConfiguration : IEntityTypeConfiguration<KioskIslemGrup>
    {
        public void Configure(EntityTypeBuilder<KioskIslemGrup> builder)
        {
            builder.ToTable("SIR_KioskIslemGruplari", "dbo");

            builder.HasKey(kig => kig.KioskIslemGrupId);

            builder.Property(kig => kig.KioskIslemGrupId)
                .ValueGeneratedOnAdd();

            builder.Property(kig => kig.KanalAltId)
                .IsRequired();

            builder.Property(kig => kig.KioskIslemGrupSira)
                .IsRequired();

            builder.Property(kig => kig.KioskIslemGrupAktiflik)
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif);

            builder.Property(kig => kig.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(kig => kig.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(kig => kig.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(kig => new { kig.KioskGrupId, kig.KanalAltId })
                .IsUnique()
                .HasDatabaseName("IX_SIR_KioskIslemGruplari_KioskGrup_KanalAlt")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(kig => !kig.SilindiMi);

            builder.HasOne(kig => kig.KioskGrup)
                .WithMany()
                .HasForeignKey(kig => kig.KioskGrupId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KioskIslemGruplari_SIR_KioskGruplari");

            builder.HasOne(kig => kig.HizmetBinasi)
                .WithMany()
                .HasForeignKey(kig => kig.HizmetBinasiId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KioskIslemGruplari_CMN_HizmetBinalari");

            builder.HasOne(kig => kig.KanalAlt)
                .WithMany()
                .HasForeignKey(kig => kig.KanalAltId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KioskIslemGruplari_SIR_KanallarAlt");

            builder.HasMany(kig => kig.KanalAltIslemleri)
                .WithOne(kai => kai.KioskIslemGrup)
                .HasForeignKey(kai => kai.KioskIslemGrupId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KanalAltIslemleri_SIR_KioskIslemGruplari");
        }
    }
}