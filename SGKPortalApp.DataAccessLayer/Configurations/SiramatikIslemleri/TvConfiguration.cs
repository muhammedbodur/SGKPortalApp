using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class TvConfiguration : IEntityTypeConfiguration<Tv>
    {
        public void Configure(EntityTypeBuilder<Tv> builder)
        {
            builder.ToTable("SIR_Tvler", "dbo");

            builder.HasKey(t => t.TvId);

            builder.Property(t => t.TvId)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.TvAdi)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("TV ekran adı");

            builder.Property(t => t.KatTipi)
                .HasConversion<int>()
                .HasComment("Kat bilgisi");

            builder.Property(t => t.TvAktiflik)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif)
                .HasComment("TV aktiflik durumu");

            builder.Property(t => t.TcKimlikNo)
                .HasMaxLength(11)
                .HasComment("TV için oluşturulan User'ın TcKimlikNo'su (FK)");

            builder.Property(t => t.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(t => t.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(t => t.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(t => t.TvAdi)
                .IsUnique()
                .HasDatabaseName("IX_SIR_Tvler_TvAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasIndex(t => t.TvAktiflik)
                .HasDatabaseName("IX_SIR_Tvler_Aktiflik");

            builder.HasIndex(t => t.TcKimlikNo)
                .IsUnique()
                .HasDatabaseName("IX_SIR_Tvler_TcKimlikNo")
                .HasFilter("[TcKimlikNo] IS NOT NULL AND [SilindiMi] = 0");

            builder.HasQueryFilter(t => !t.SilindiMi);

            builder.HasOne(t => t.HizmetBinasi)
                .WithMany()
                .HasForeignKey(t => t.HizmetBinasiId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_Tvler_CMN_HizmetBinalari");
        }
    }
}