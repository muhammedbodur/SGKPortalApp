using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class DuyuruResimConfiguration : IEntityTypeConfiguration<DuyuruResim>
    {
        public void Configure(EntityTypeBuilder<DuyuruResim> builder)
        {
            builder.ToTable("CMN_DuyuruResimler", "dbo");

            builder.HasKey(r => r.DuyuruResimId);

            builder.Property(r => r.DuyuruResimId)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.DuyuruId)
                .IsRequired()
                .HasComment("Bağlı duyuru (haber) ID");

            builder.Property(r => r.ResimUrl)
                .IsRequired()
                .HasMaxLength(500)
                .HasComment("Resim dosya yolu");

            builder.Property(r => r.IsVitrin)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Vitrin (slider) fotoğrafı mı");

            builder.Property(r => r.Sira)
                .IsRequired()
                .HasDefaultValue(1)
                .HasComment("Resim sırası");

            builder.Property(r => r.Aktiflik)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif);

            // Auditing
            builder.Property(r => r.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(r => r.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(r => r.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // FK
            builder.HasOne(r => r.Duyuru)
                .WithMany()
                .HasForeignKey(r => r.DuyuruId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(r => r.DuyuruId)
                .HasDatabaseName("IX_CMN_DuyuruResimler_DuyuruId");

            builder.HasIndex(r => new { r.DuyuruId, r.Sira })
                .HasDatabaseName("IX_CMN_DuyuruResimler_DuyuruId_Sira");

            // Soft delete filter
            builder.HasQueryFilter(r => !r.SilindiMi);
        }
    }
}
