using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class KanalConfiguration : IEntityTypeConfiguration<Kanal>
    {
        public void Configure(EntityTypeBuilder<Kanal> builder)
        {
            builder.ToTable("SIR_Kanallar", "dbo");

            builder.HasKey(k => k.KanalId);

            builder.Property(k => k.KanalId)
                .ValueGeneratedOnAdd();

            builder.Property(k => k.KanalAdi)
                .IsRequired()
                .HasMaxLength(150)
                .HasComment("Kanal adı");

            builder.Property(k => k.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(k => k.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(k => k.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(k => k.KanalAdi)
                .IsUnique()
                .HasDatabaseName("IX_SIR_Kanallar_KanalAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(k => !k.SilindiMi);

            builder.HasMany(k => k.KanalAltlari)
                .WithOne(ka => ka.Kanal)
                .HasForeignKey(ka => ka.KanalId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KanallarAlt_SIR_Kanallar");

            builder.HasMany(k => k.KanalIslemleri)
                .WithOne(ki => ki.Kanal)
                .HasForeignKey(ki => ki.KanalId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KanalIslemleri_SIR_Kanallar");
        }
    }
}