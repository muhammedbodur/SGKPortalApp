using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class YetkiConfiguration : IEntityTypeConfiguration<Yetki>
    {
        public void Configure(EntityTypeBuilder<Yetki> builder)
        {
            builder.ToTable("PER_Yetkiler", "dbo");

            builder.HasKey(y => y.YetkiId);

            builder.Property(y => y.YetkiId)
                .ValueGeneratedOnAdd();

            builder.Property(y => y.YetkiAdi)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(y => y.ControllerAdi)
                .HasMaxLength(100);

            builder.Property(y => y.YetkiTuru)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(y => y.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(y => y.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(y => y.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(y => new { y.UstYetkiId, y.YetkiAdi })
                .IsUnique()
                .HasDatabaseName("IX_PER_Yetkiler_UstYetki_YetkiAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasIndex(y => y.YetkiAdi)
                .HasDatabaseName("IX_PER_Yetkiler_YetkiAdi");

            builder.HasIndex(y => y.ControllerAdi)
                .HasDatabaseName("IX_PER_Yetkiler_ControllerAdi");

            builder.HasQueryFilter(y => !y.SilindiMi);
        }
    }
}