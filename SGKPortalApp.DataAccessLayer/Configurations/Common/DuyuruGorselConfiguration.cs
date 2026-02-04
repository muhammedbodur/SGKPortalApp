using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class DuyuruGorselConfiguration : IEntityTypeConfiguration<DuyuruGorsel>
    {
        public void Configure(EntityTypeBuilder<DuyuruGorsel> builder)
        {
            // Table
            builder.ToTable("CMN_DuyuruGorseller", "dbo");

            // Primary Key
            builder.HasKey(dg => dg.DuyuruGorselId);

            // Properties
            builder.Property(dg => dg.DuyuruGorselId)
                .ValueGeneratedOnAdd();

            builder.Property(dg => dg.DuyuruId)
                .IsRequired()
                .HasComment("İlişkili duyuru ID");

            builder.Property(dg => dg.GorselUrl)
                .IsRequired()
                .HasMaxLength(500)
                .HasComment("Görsel URL");

            builder.Property(dg => dg.Sira)
                .IsRequired()
                .HasDefaultValue(1)
                .HasComment("Görsel sırası");

            builder.Property(dg => dg.VitrinFoto)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Vitrin fotoğrafı mı?");

            builder.Property(dg => dg.Aciklama)
                .HasMaxLength(200)
                .HasComment("Görsel açıklaması");

            // Auditing
            builder.Property(dg => dg.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Kayıt oluşturulma tarihi");

            builder.Property(dg => dg.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Son güncelleme tarihi");

            builder.Property(dg => dg.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Soft delete flag");

            // Relationships
            builder.HasOne(dg => dg.Duyuru)
                .WithMany(d => d.Gorseller)
                .HasForeignKey(dg => dg.DuyuruId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(dg => dg.DuyuruId)
                .HasDatabaseName("IX_CMN_DuyuruGorseller_DuyuruId");

            builder.HasIndex(dg => new { dg.DuyuruId, dg.Sira })
                .HasDatabaseName("IX_CMN_DuyuruGorseller_DuyuruId_Sira");

            builder.HasIndex(dg => dg.VitrinFoto)
                .HasDatabaseName("IX_CMN_DuyuruGorseller_VitrinFoto");

            // Soft Delete Query Filter
            builder.HasQueryFilter(dg => !dg.SilindiMi);
        }
    }
}
