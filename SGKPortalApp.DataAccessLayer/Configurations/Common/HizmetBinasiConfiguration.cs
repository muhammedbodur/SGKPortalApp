using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class HizmetBinasiConfiguration : IEntityTypeConfiguration<HizmetBinasi>
    {
        public void Configure(EntityTypeBuilder<HizmetBinasi> builder)
        {
            builder.ToTable("CMN_HizmetBinalari", "dbo");

            builder.HasKey(hb => hb.HizmetBinasiId);

            builder.Property(hb => hb.HizmetBinasiId)
                .ValueGeneratedOnAdd();

            builder.Property(hb => hb.HizmetBinasiAdi)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(hb => hb.Adres)
                .HasMaxLength(500);

            builder.Property(hb => hb.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(hb => hb.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(hb => hb.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(hb => hb.HizmetBinasiAdi)
                .IsUnique()
                .HasDatabaseName("IX_CMN_HizmetBinalari_HizmetBinasiAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(hb => !hb.SilindiMi);

            builder.HasOne(hb => hb.Departman)
                .WithMany(d => d.HizmetBinalari)
                .HasForeignKey(hb => hb.DepartmanId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CMN_HizmetBinalari_PER_Departmanlar");

            builder.HasMany(hb => hb.Personeller)
                .WithOne(p => p.HizmetBinasi)
                .HasForeignKey(p => p.HizmetBinasiId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PER_Personeller_CMN_HizmetBinalari");
        }
    }
}