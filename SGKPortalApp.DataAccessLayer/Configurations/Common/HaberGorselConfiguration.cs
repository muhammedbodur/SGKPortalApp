using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class HaberGorselConfiguration : IEntityTypeConfiguration<HaberGorsel>
    {
        public void Configure(EntityTypeBuilder<HaberGorsel> builder)
        {
            // Table
            builder.ToTable("CMN_HaberGorseller", "dbo");

            // Primary Key
            builder.HasKey(hg => hg.HaberGorselId);

            // Properties
            builder.Property(hg => hg.HaberGorselId)
                .ValueGeneratedOnAdd();

            builder.Property(hg => hg.HaberId)
                .IsRequired()
                .HasComment("İlişkili haber ID");

            builder.Property(hg => hg.GorselUrl)
                .IsRequired()
                .HasMaxLength(500)
                .HasComment("Görsel URL");

            builder.Property(hg => hg.Sira)
                .IsRequired()
                .HasDefaultValue(1)
                .HasComment("Görsel sırası");

            builder.Property(hg => hg.VitrinFoto)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Vitrin fotoğrafı mı?");

            builder.Property(hg => hg.Aciklama)
                .HasMaxLength(200)
                .HasComment("Görsel açıklaması");

            // Auditing
            builder.Property(hg => hg.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Kayıt oluşturulma tarihi");

            builder.Property(hg => hg.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Son güncelleme tarihi");

            builder.Property(hg => hg.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Soft delete flag");

            // Relationships
            builder.HasOne(hg => hg.Haber)
                .WithMany(h => h.Gorseller)
                .HasForeignKey(hg => hg.HaberId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(hg => hg.HaberId)
                .HasDatabaseName("IX_CMN_HaberGorseller_HaberId");

            builder.HasIndex(hg => new { hg.HaberId, hg.Sira })
                .HasDatabaseName("IX_CMN_HaberGorseller_HaberId_Sira");

            builder.HasIndex(hg => hg.VitrinFoto)
                .HasDatabaseName("IX_CMN_HaberGorseller_VitrinFoto");

            // Soft Delete Query Filter
            builder.HasQueryFilter(hg => !hg.SilindiMi);
        }
    }
}
