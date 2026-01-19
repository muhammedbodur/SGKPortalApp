using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class ResmiTatilConfiguration : IEntityTypeConfiguration<ResmiTatil>
    {
        public void Configure(EntityTypeBuilder<ResmiTatil> builder)
        {
            // Table
            builder.ToTable("CMN_ResmiTatiller", "dbo");

            // Primary Key
            builder.HasKey(t => t.TatilId);

            // Properties
            builder.Property(t => t.TatilId)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.TatilAdi)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Tatil adı");

            builder.Property(t => t.Tarih)
                .IsRequired()
                .HasComment("Tatil tarihi");

            builder.Property(t => t.Yil)
                .IsRequired()
                .HasComment("Tatil yılı");

            builder.Property(t => t.TatilTipi)
                .IsRequired()
                .HasConversion<int>()
                .HasComment("Tatil tipi (0: Sabit, 1: Dini, 2: Özel)");

            builder.Property(t => t.YariGun)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Yarım gün tatil mi?");

            builder.Property(t => t.Aciklama)
                .HasMaxLength(500)
                .HasComment("Açıklama/Not");

            builder.Property(t => t.OtomatikSenkronize)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Nager.Date'ten otomatik senkronize edildi mi?");

            // Auditing
            builder.Property(t => t.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Kayıt oluşturulma tarihi");

            builder.Property(t => t.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Son güncelleme tarihi");

            builder.Property(t => t.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Soft delete flag");

            // Indexes
            builder.HasIndex(t => t.Tarih)
                .HasDatabaseName("IX_CMN_ResmiTatiller_Tarih");

            builder.HasIndex(t => t.Yil)
                .HasDatabaseName("IX_CMN_ResmiTatiller_Yil");

            builder.HasIndex(t => t.TatilTipi)
                .HasDatabaseName("IX_CMN_ResmiTatiller_TatilTipi");

            builder.HasIndex(t => new { t.Yil, t.Tarih })
                .HasDatabaseName("IX_CMN_ResmiTatiller_Yil_Tarih");

            builder.HasIndex(t => t.SilindiMi)
                .HasDatabaseName("IX_CMN_ResmiTatiller_SilindiMi");

            // Soft Delete Query Filter
            builder.HasQueryFilter(t => !t.SilindiMi);
        }
    }
}
