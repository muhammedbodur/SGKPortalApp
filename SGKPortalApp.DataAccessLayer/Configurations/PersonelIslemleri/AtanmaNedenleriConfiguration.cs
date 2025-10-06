using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class AtanmaNedenleriConfiguration : IEntityTypeConfiguration<AtanmaNedenleri>
    {
        public void Configure(EntityTypeBuilder<AtanmaNedenleri> builder)
        {
            builder.ToTable("PER_AtanmaNedenleri", "dbo");

            builder.HasKey(a => a.AtanmaNedeniId);

            builder.Property(a => a.AtanmaNedeniId)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.AtanmaNedeni)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(a => a.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(a => a.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(a => a.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(a => a.AtanmaNedeni)
                .IsUnique()
                .HasDatabaseName("IX_PER_AtanmaNedenleri_AtanmaNedeni")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(a => !a.SilindiMi);

            builder.HasMany(a => a.Personeller)
                .WithOne(p => p.AtanmaNedeni)
                .HasForeignKey(p => p.AtanmaNedeniId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PER_Personeller_PER_AtanmaNedenleri");
        }
    }
}