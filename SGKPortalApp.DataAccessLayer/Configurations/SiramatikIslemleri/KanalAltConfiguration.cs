using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class KanalAltConfiguration : IEntityTypeConfiguration<KanalAlt>
    {
        public void Configure(EntityTypeBuilder<KanalAlt> builder)
        {
            builder.ToTable("SIR_KanallarAlt", "dbo");

            builder.HasKey(ka => ka.KanalAltId);

            builder.Property(ka => ka.KanalAltId)
                .ValueGeneratedOnAdd();

            builder.Property(ka => ka.KanalAltAdi)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(ka => ka.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(ka => ka.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(ka => ka.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(ka => new { ka.KanalId, ka.KanalAltAdi })
                .IsUnique()
                .HasDatabaseName("IX_SIR_KanallarAlt_Kanal_KanalAltAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(ka => !ka.SilindiMi);

            builder.HasOne(ka => ka.Kanal)
                .WithMany(k => k.KanalAltlari)
                .HasForeignKey(ka => ka.KanalId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KanallarAlt_SIR_Kanallar");

            builder.HasMany(ka => ka.KanalAltIslemleri)
                .WithOne(kai => kai.KanalAlt)
                .HasForeignKey(kai => kai.KanalAltId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KanalAltIslemleri_SIR_KanallarAlt");
        }
    }
}