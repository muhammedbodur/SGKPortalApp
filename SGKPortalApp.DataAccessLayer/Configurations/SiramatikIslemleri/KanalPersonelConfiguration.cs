using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class KanalPersonelConfiguration : IEntityTypeConfiguration<KanalPersonel>
    {
        public void Configure(EntityTypeBuilder<KanalPersonel> builder)
        {
            builder.ToTable("SIR_KanalPersonelleri", "dbo");

            builder.HasKey(kp => kp.KanalPersonelId);

            builder.Property(kp => kp.KanalPersonelId)
                .ValueGeneratedOnAdd();

            builder.Property(kp => kp.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(kp => kp.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(kp => kp.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(kp => kp.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(kp => new { kp.KanalAltIslemId, kp.TcKimlikNo })
                .IsUnique()
                .HasDatabaseName("IX_SIR_KanalPersonelleri_KanalAltIslem_Tc")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(kp => !kp.SilindiMi);

            builder.HasOne(kp => kp.Personel)
                .WithMany()
                .HasForeignKey(kp => kp.TcKimlikNo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KanalPersonelleri_PER_Personeller");

            builder.HasOne(kp => kp.KanalAltIslem)
                .WithMany(kai => kai.KanalPersonelleri)
                .HasForeignKey(kp => kp.KanalAltIslemId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KanalPersonelleri_SIR_KanalAltIslemleri");
        }
    }
}