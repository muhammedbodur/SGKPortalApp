using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class HaberConfiguration : IEntityTypeConfiguration<Haber>
    {
        public void Configure(EntityTypeBuilder<Haber> builder)
        {
            // Table
            builder.ToTable("CMN_Haberler", "dbo");

            // Primary Key
            builder.HasKey(h => h.HaberId);

            // Properties
            builder.Property(h => h.HaberId)
                .ValueGeneratedOnAdd();

            builder.Property(h => h.Baslik)
                .IsRequired()
                .HasMaxLength(200)
                .HasComment("Haber başlığı");

            builder.Property(h => h.Icerik)
                .IsRequired()
                .HasComment("Haber içeriği");

            builder.Property(h => h.GorselUrl)
                .HasMaxLength(500)
                .HasComment("Vitrin görseli URL");

            builder.Property(h => h.Sira)
                .IsRequired()
                .HasComment("Haber sırası");

            builder.Property(h => h.YayinTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Yayın tarihi");

            builder.Property(h => h.BitisTarihi)
                .HasComment("Bitiş tarihi (Null ise süresiz)");

            builder.Property(h => h.Aktiflik)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif)
                .HasComment("Haber aktiflik durumu (0: Pasif, 1: Aktif)");

            // Auditing
            builder.Property(h => h.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Kayıt oluşturulma tarihi");

            builder.Property(h => h.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Son güncelleme tarihi");

            builder.Property(h => h.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Soft delete flag");

            // Indexes
            builder.HasIndex(h => h.YayinTarihi)
                .HasDatabaseName("IX_CMN_Haberler_YayinTarihi");

            builder.HasIndex(h => h.Aktiflik)
                .HasDatabaseName("IX_CMN_Haberler_Aktiflik");

            builder.HasIndex(h => new { h.Aktiflik, h.SilindiMi })
                .HasDatabaseName("IX_CMN_Haberler_Aktiflik_SilindiMi");

            builder.HasIndex(h => h.Sira)
                .HasDatabaseName("IX_CMN_Haberler_Sira");

            // Soft Delete Query Filter
            builder.HasQueryFilter(h => !h.SilindiMi);
        }
    }
}
