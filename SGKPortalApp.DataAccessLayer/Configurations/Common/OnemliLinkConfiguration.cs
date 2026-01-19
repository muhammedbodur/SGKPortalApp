using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class OnemliLinkConfiguration : IEntityTypeConfiguration<OnemliLink>
    {
        public void Configure(EntityTypeBuilder<OnemliLink> builder)
        {
            // Table
            builder.ToTable("CMN_OnemliLinkler", "dbo");

            // Primary Key
            builder.HasKey(o => o.LinkId);

            // Properties
            builder.Property(o => o.LinkId)
                .ValueGeneratedOnAdd();

            builder.Property(o => o.LinkAdi)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Link adı");

            builder.Property(o => o.Url)
                .IsRequired()
                .HasMaxLength(500)
                .HasComment("Link URL");

            builder.Property(o => o.Sira)
                .IsRequired()
                .HasComment("Link sırası");

            builder.Property(o => o.Aktiflik)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif)
                .HasComment("Link aktiflik durumu (0: Pasif, 1: Aktif)");

            // Auditing
            builder.Property(o => o.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Kayıt oluşturulma tarihi");

            builder.Property(o => o.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Son güncelleme tarihi");

            builder.Property(o => o.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Soft delete flag");

            // Indexes
            builder.HasIndex(o => o.Sira)
                .HasDatabaseName("IX_CMN_OnemliLinkler_Sira");

            builder.HasIndex(o => o.Aktiflik)
                .HasDatabaseName("IX_CMN_OnemliLinkler_Aktiflik");

            builder.HasIndex(o => new { o.Aktiflik, o.SilindiMi })
                .HasDatabaseName("IX_CMN_OnemliLinkler_Aktiflik_SilindiMi");

            // Soft Delete Query Filter
            builder.HasQueryFilter(o => !o.SilindiMi);
        }
    }
}
