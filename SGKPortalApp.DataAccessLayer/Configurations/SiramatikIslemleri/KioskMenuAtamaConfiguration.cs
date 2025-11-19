using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class KioskMenuAtamaConfiguration : IEntityTypeConfiguration<KioskMenuAtama>
    {
        public void Configure(EntityTypeBuilder<KioskMenuAtama> builder)
        {
            builder.ToTable("SIR_KioskMenuAtama");

            builder.HasKey(kma => kma.KioskMenuAtamaId);

            builder.Property(kma => kma.KioskMenuAtamaId)
                .ValueGeneratedOnAdd();

            builder.Property(kma => kma.AtamaTarihi)
                .IsRequired();

            builder.Property(kma => kma.Aktiflik)
                .IsRequired();

            // Relationships
            builder.HasOne(kma => kma.Kiosk)
                .WithMany(k => k.MenuAtamalari)
                .HasForeignKey(kma => kma.KioskId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KioskMenuAtama_SIR_Kiosk");

            builder.HasOne(kma => kma.KioskMenu)
                .WithMany(km => km.MenuAtamalari)
                .HasForeignKey(kma => kma.KioskMenuId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KioskMenuAtama_SIR_KioskMenu");

            // Indexes
            builder.HasIndex(kma => kma.KioskId)
                .HasDatabaseName("IX_SIR_KioskMenuAtama_KioskId");

            builder.HasIndex(kma => kma.KioskMenuId)
                .HasDatabaseName("IX_SIR_KioskMenuAtama_KioskMenuId");

            // Unique constraint: Bir kiosk'a aynı anda sadece bir aktif menü atanabilir
            builder.HasIndex(kma => new { kma.KioskId, kma.Aktiflik })
                .HasDatabaseName("IX_SIR_KioskMenuAtama_Kiosk_Aktif")
                .IsUnique()
                .HasFilter("[Aktiflik] = 1 AND [SilindiMi] = 0");

            // Soft Delete Filter
            builder.HasIndex(kma => kma.SilindiMi)
                .HasDatabaseName("IX_SIR_KioskMenuAtama_SilindiMi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(kma => !kma.SilindiMi);
        }
    }
}
