using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class KioskMenuIslemConfiguration : IEntityTypeConfiguration<KioskMenuIslem>
    {
        public void Configure(EntityTypeBuilder<KioskMenuIslem> builder)
        {
            builder.ToTable("SIR_KioskMenuIslem");

            builder.HasKey(kmi => kmi.KioskMenuIslemId);

            builder.Property(kmi => kmi.KioskMenuIslemId)
                .ValueGeneratedOnAdd();

            builder.Property(kmi => kmi.IslemAdi)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(kmi => kmi.MenuSira)
                .IsRequired();

            builder.Property(kmi => kmi.Aktiflik)
                .IsRequired();

            // Relationships
            builder.HasOne(kmi => kmi.KioskMenu)
                .WithMany(km => km.KioskMenuIslemler)
                .HasForeignKey(kmi => kmi.KioskMenuId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KioskMenuIslem_SIR_KioskMenu");

            builder.HasOne(kmi => kmi.KanalAlt)
                .WithMany(ka => ka.KioskMenuIslemleri)
                .HasForeignKey(kmi => kmi.KanalAltId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KioskMenuIslem_SIR_KanalAlt");

            // Indexes
            builder.HasIndex(kmi => kmi.KioskMenuId)
                .HasDatabaseName("IX_SIR_KioskMenuIslem_KioskMenuId");

            builder.HasIndex(kmi => kmi.KanalAltId)
                .HasDatabaseName("IX_SIR_KioskMenuIslem_KanalAltId");

            builder.HasIndex(kmi => new { kmi.KioskMenuId, kmi.MenuSira })
                .HasDatabaseName("IX_SIR_KioskMenuIslem_Menu_Sira");

            // Soft Delete Filter
            builder.HasIndex(kmi => kmi.SilindiMi)
                .HasDatabaseName("IX_SIR_KioskMenuIslem_SilindiMi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(kmi => !kmi.SilindiMi);
        }
    }
}
