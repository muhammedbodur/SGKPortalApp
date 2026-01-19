using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class DuyuruConfiguration : IEntityTypeConfiguration<Duyuru>
    {
        public void Configure(EntityTypeBuilder<Duyuru> builder)
        {
            // Table
            builder.ToTable("CMN_Duyurular", "dbo");

            // Primary Key
            builder.HasKey(d => d.DuyuruId);

            // Properties
            builder.Property(d => d.DuyuruId)
                .ValueGeneratedOnAdd();

            builder.Property(d => d.Baslik)
                .IsRequired()
                .HasMaxLength(200)
                .HasComment("Duyuru başlığı");

            builder.Property(d => d.Icerik)
                .IsRequired()
                .HasComment("Duyuru içeriği");

            builder.Property(d => d.GorselUrl)
                .HasMaxLength(500)
                .HasComment("Duyuru görseli URL");

            builder.Property(d => d.Sira)
                .IsRequired()
                .HasComment("Duyuru sırası");

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
                .HasComment("Duyuru aktiflik durumu (0: Pasif, 1: Aktif)");

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
                .HasDatabaseName("IX_CMN_Duyurular_YayinTarihi");

            builder.HasIndex(d => d.Aktiflik)
                .HasDatabaseName("IX_CMN_Duyurular_Aktiflik");

            builder.HasIndex(d => new { d.Aktiflik, d.SilindiMi })
                .HasDatabaseName("IX_CMN_Duyurular_Aktiflik_SilindiMi");

            builder.HasIndex(d => d.Sira)
                .HasDatabaseName("IX_CMN_Duyurular_Sira");

            // Soft Delete Query Filter
            builder.HasQueryFilter(d => !d.SilindiMi);
        }
    }
}
