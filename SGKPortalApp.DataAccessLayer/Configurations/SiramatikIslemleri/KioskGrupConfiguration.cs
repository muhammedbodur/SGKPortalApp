using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class KioskGrupConfiguration : IEntityTypeConfiguration<KioskGrup>
    {
        public void Configure(EntityTypeBuilder<KioskGrup> builder)
        {
            builder.ToTable("SIR_KioskGruplari", "dbo");

            builder.HasKey(kg => kg.KioskGrupId);

            builder.Property(kg => kg.KioskGrupId)
                .ValueGeneratedOnAdd();

            builder.Property(kg => kg.KioskGrupAdi)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(kg => kg.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(kg => kg.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(kg => kg.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(kg => kg.KioskGrupAdi)
                .IsUnique()
                .HasDatabaseName("IX_SIR_KioskGruplari_KioskGrupAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(kg => !kg.SilindiMi);
        }
    }
}