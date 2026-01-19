using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class GununMenusuConfiguration : IEntityTypeConfiguration<GununMenusu>
    {
        public void Configure(EntityTypeBuilder<GununMenusu> builder)
        {
            // Table
            builder.ToTable("CMN_GununMenusu", "dbo");

            // Primary Key
            builder.HasKey(g => g.MenuId);

            // Properties
            builder.Property(g => g.MenuId)
                .ValueGeneratedOnAdd();

            builder.Property(g => g.Tarih)
                .IsRequired()
                .HasColumnType("date")
                .HasComment("Menü tarihi");

            builder.Property(g => g.Icerik)
                .IsRequired()
                .HasComment("Menü içeriği");

            builder.Property(g => g.Aktiflik)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif)
                .HasComment("Menü aktiflik durumu (0: Pasif, 1: Aktif)");

            // Auditing
            builder.Property(g => g.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Kayıt oluşturulma tarihi");

            builder.Property(g => g.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Son güncelleme tarihi");

            builder.Property(g => g.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Soft delete flag");

            // Indexes
            builder.HasIndex(g => g.Tarih)
                .HasDatabaseName("IX_CMN_GununMenusu_Tarih");

            builder.HasIndex(g => g.Aktiflik)
                .HasDatabaseName("IX_CMN_GununMenusu_Aktiflik");

            builder.HasIndex(g => new { g.Aktiflik, g.SilindiMi })
                .HasDatabaseName("IX_CMN_GununMenusu_Aktiflik_SilindiMi");

            // Soft Delete Query Filter
            builder.HasQueryFilter(g => !g.SilindiMi);
        }
    }
}
