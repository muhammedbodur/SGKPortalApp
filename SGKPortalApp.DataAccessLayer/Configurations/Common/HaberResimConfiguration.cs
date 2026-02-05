using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class HaberResimConfiguration : IEntityTypeConfiguration<HaberResim>
    {
        public void Configure(EntityTypeBuilder<HaberResim> builder)
        {
            builder.ToTable("CMN_HaberResimler", "dbo");

            builder.HasKey(r => r.HaberResimId);

            builder.Property(r => r.HaberResimId)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.HaberId)
                .IsRequired()
                .HasComment("Bağlı haber ID");

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
            builder.HasOne(r => r.Haber)
                .WithMany()
                .HasForeignKey(r => r.HaberId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(r => r.HaberId)
                .HasDatabaseName("IX_CMN_HaberResimler_HaberId");

            builder.HasIndex(r => new { r.HaberId, r.Sira })
                .HasDatabaseName("IX_CMN_HaberResimler_HaberId_Sira");

            // Soft delete filter
            builder.HasQueryFilter(r => !r.SilindiMi);
        }
    }
}
