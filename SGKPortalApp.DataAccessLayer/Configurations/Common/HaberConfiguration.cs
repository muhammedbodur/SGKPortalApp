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
            builder.HasKey(d => d.HaberId);

            // Properties
            builder.Property(d => d.HaberId)
                .ValueGeneratedOnAdd();

            builder.Property(d => d.Baslik)
                .IsRequired()
                .HasMaxLength(200)
                .HasComment("Haber başlığı");

            builder.Property(d => d.Icerik)
                .IsRequired()
                .HasComment("Haber içeriği");

            builder.Property(d => d.GorselUrl)
                .HasMaxLength(500)
                .HasComment("Haber görseli URL");

            builder.Property(d => d.Sira)
                .IsRequired()
                .HasComment("Haber sırası");

            builder.Property(d => d.YayinTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Yayın tarihi");

            builder.Property(d => d.BitisTarihi)
                .HasComment("Bitiş tarihi (Null ise süresiz)");

            builder.Property(d => d.Aktiflik)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif)
                .HasComment("Haber aktiflik durumu (0: Pasif, 1: Aktif)");

            // Auditing
            builder.Property(d => d.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Kayıt oluşturulma tarihi");

            builder.Property(d => d.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Son güncelleme tarihi");

            builder.Property(d => d.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Soft delete flag");

            // Indexes
            builder.HasIndex(d => d.YayinTarihi)
                .HasDatabaseName("IX_CMN_Haberler_YayinTarihi");

            builder.HasIndex(d => d.Aktiflik)
                .HasDatabaseName("IX_CMN_Haberler_Aktiflik");

            builder.HasIndex(d => new { d.Aktiflik, d.SilindiMi })
                .HasDatabaseName("IX_CMN_Haberler_Aktiflik_SilindiMi");

            builder.HasIndex(d => d.Sira)
                .HasDatabaseName("IX_CMN_Haberler_Sira");

            // Soft Delete Query Filter
            builder.HasQueryFilter(d => !d.SilindiMi);
        }
    }
}
