using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.Common
{
    public class DepartmanHizmetBinasiConfiguration : IEntityTypeConfiguration<DepartmanHizmetBinasi>
    {
        public void Configure(EntityTypeBuilder<DepartmanHizmetBinasi> builder)
        {
            builder.ToTable("CMN_DepartmanHizmetBinalari", "dbo");

            builder.HasKey(dhb => dhb.DepartmanHizmetBinasiId);

            builder.Property(dhb => dhb.DepartmanHizmetBinasiId)
                .ValueGeneratedOnAdd();

            builder.Property(dhb => dhb.AnaBina)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(dhb => dhb.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(dhb => dhb.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(dhb => dhb.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // Unique index: Aynı departman-bina kombinasyonu tekrar edemez
            builder.HasIndex(dhb => new { dhb.DepartmanId, dhb.HizmetBinasiId })
                .IsUnique()
                .HasDatabaseName("IX_CMN_DepartmanHizmetBinalari_DepartmanId_HizmetBinasiId")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(dhb => !dhb.SilindiMi);

            // Departman ilişkisi
            builder.HasOne(dhb => dhb.Departman)
                .WithMany(d => d.DepartmanHizmetBinalari)
                .HasForeignKey(dhb => dhb.DepartmanId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CMN_DepartmanHizmetBinalari_PER_Departmanlar");

            // HizmetBinasi ilişkisi (HizmetBinasiConfiguration'da tanımlandı)
        }
    }
}
