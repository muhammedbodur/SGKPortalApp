using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class BankoIslemConfiguration : IEntityTypeConfiguration<BankoIslem>
    {
        public void Configure(EntityTypeBuilder<BankoIslem> builder)
        {
            builder.ToTable("SIR_BankoIslemleri", "dbo");

            builder.HasKey(bi => bi.BankoIslemId);

            builder.Property(bi => bi.BankoIslemId)
                .ValueGeneratedOnAdd();

            builder.Property(bi => bi.BankoIslemAdi)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(bi => bi.BankoGrup)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(bi => bi.BankoIslemAktiflik)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif);

            builder.Property(bi => bi.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(bi => bi.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(bi => bi.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(bi => bi.BankoIslemAdi)
                .IsUnique()
                .HasDatabaseName("IX_SIR_BankoIslemleri_BankoIslemAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(bi => !bi.SilindiMi);
        }
    }
}