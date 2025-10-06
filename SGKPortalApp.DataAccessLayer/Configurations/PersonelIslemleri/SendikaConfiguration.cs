using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class SendikaConfiguration : IEntityTypeConfiguration<Sendika>
    {
        public void Configure(EntityTypeBuilder<Sendika> builder)
        {
            builder.ToTable("PER_Sendikalar", "dbo");

            builder.HasKey(s => s.SendikaId);

            builder.Property(s => s.SendikaId)
                .ValueGeneratedOnAdd();

            builder.Property(s => s.SendikaAdi)
                .IsRequired()
                .HasMaxLength(150)
                .HasComment("Sendika adı");

            builder.Property(s => s.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(s => s.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(s => s.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(s => s.SendikaAdi)
                .IsUnique()
                .HasDatabaseName("IX_PER_Sendikalar_SendikaAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(s => !s.SilindiMi);

            builder.HasMany(s => s.Personeller)
                .WithOne(p => p.Sendika)
                .HasForeignKey(p => p.SendikaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PER_Personeller_PER_Sendikalar");
        }
    }
}