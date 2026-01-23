using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class KanalIslemConfiguration : IEntityTypeConfiguration<KanalIslem>
    {
        public void Configure(EntityTypeBuilder<KanalIslem> builder)
        {
            builder.ToTable("SIR_KanalIslemleri", "dbo");

            builder.HasKey(ki => ki.KanalIslemId);

            builder.Property(ki => ki.KanalIslemId)
                .ValueGeneratedOnAdd();

            builder.Property(ki => ki.BaslangicNumara)
                .IsRequired();

            builder.Property(ki => ki.BitisNumara)
                .IsRequired();

            builder.Property(ki => ki.Aktiflik)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(ki => ki.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(ki => ki.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(ki => ki.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(ki => new { ki.KanalId, ki.DepartmanHizmetBinasiId })
                .HasDatabaseName("IX_SIR_KanalIslemleri_Kanal_DepartmanHizmetBinasi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(ki => !ki.SilindiMi);

            builder.HasOne(ki => ki.Kanal)
                .WithMany(k => k.KanalIslemleri)
                .HasForeignKey(ki => ki.KanalId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KanalIslemleri_SIR_Kanallar");

            builder.HasOne(ki => ki.DepartmanHizmetBinasi)
                .WithMany(dhb => dhb.KanalIslemleri)
                .HasForeignKey(ki => ki.DepartmanHizmetBinasiId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KanalIslemleri_CMN_DepartmanHizmetBinalari");

            builder.HasMany(ki => ki.KanalAltIslemleri)
                .WithOne(kai => kai.KanalIslem)
                .HasForeignKey(kai => kai.KanalIslemId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_KanalAltIslemleri_SIR_KanalIslemleri");
        }
    }
}