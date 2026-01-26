using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class HizmetBinasiConfiguration : IEntityTypeConfiguration<HizmetBinasi>
    {
        public void Configure(EntityTypeBuilder<HizmetBinasi> builder)
        {
            builder.ToTable("CMN_HizmetBinalari", "dbo");

            builder.HasKey(hb => hb.HizmetBinasiId);

            builder.Property(hb => hb.HizmetBinasiId)
                .ValueGeneratedOnAdd();

            builder.Property(hb => hb.HizmetBinasiAdi)
                .IsRequired()
                .HasMaxLength(200);

            // MySQL legacy sistemdeki KOD değeri (senkronizasyon için)
            builder.Property(hb => hb.LegacyKod)
                .IsRequired(false);

            builder.Property(hb => hb.Adres)
                .HasMaxLength(500);

            builder.Property(hb => hb.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(hb => hb.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(hb => hb.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(hb => hb.HizmetBinasiAdi)
                .IsUnique()
                .HasDatabaseName("IX_CMN_HizmetBinalari_HizmetBinasiAdi")
                .HasFilter("[SilindiMi] = 0");

            // LegacyKod için unique index (MySQL sync için)
            builder.HasIndex(hb => hb.LegacyKod)
                .IsUnique()
                .HasDatabaseName("IX_CMN_HizmetBinalari_LegacyKod")
                .HasFilter("[LegacyKod] IS NOT NULL AND [SilindiMi] = 0");

            builder.HasQueryFilter(hb => !hb.SilindiMi);

            // Many-to-many ilişki DepartmanHizmetBinasi üzerinden
            builder.HasMany(hb => hb.DepartmanHizmetBinalari)
                .WithOne(dhb => dhb.HizmetBinasi)
                .HasForeignKey(dhb => dhb.HizmetBinasiId)
                .OnDelete(DeleteBehavior.Cascade);

            // Not: Personel artık doğrudan HizmetBinasi'na bağlı değil.
            // Personel -> DepartmanHizmetBinasi -> HizmetBinasi şeklinde dolaylı ilişki var.
        }
    }
}