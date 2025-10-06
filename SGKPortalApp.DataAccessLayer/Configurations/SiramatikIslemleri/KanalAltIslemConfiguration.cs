using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class KanalAltIslemConfiguration : IEntityTypeConfiguration<KanalAltIslem>
    {
        public void Configure(EntityTypeBuilder<KanalAltIslem> builder)
        {
            builder.ToTable("SIR_KanalAltIslemleri", "dbo");

            builder.HasKey(kai => kai.KanalAltIslemId);

            builder.Property(kai => kai.KanalAltIslemId)
                .ValueGeneratedOnAdd();

            builder.Property(kai => kai.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(kai => kai.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(kai => kai.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(kai => new { kai.KanalAltId, kai.KanalIslemId })
                .IsUnique()
                .HasDatabaseName("IX_SIR_KanalAltIslemleri_KanalAlt_KanalIslem")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(kai => !kai.SilindiMi);

            builder.HasMany(kai => kai.KanalPersonelleri)
                .WithOne(kp => kp.KanalAltIslem)
                .HasForeignKey(kp => kp.KanalAltIslemId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KanalPersonelleri_SIR_KanalAltIslemleri");

            builder.HasMany(kai => kai.Siralar)
                .WithOne(s => s.KanalAltIslem)
                .HasForeignKey(s => s.KanalAltIslemId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_Siralar_SIR_KanalAltIslemleri");
        }
    }
}