using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class ServisConfiguration : IEntityTypeConfiguration<Servis>
    {
        public void Configure(EntityTypeBuilder<Servis> builder)
        {
            builder.ToTable("PER_Servisler", "dbo");

            builder.HasKey(s => s.ServisId);

            builder.Property(s => s.ServisId)
                .ValueGeneratedOnAdd();

            builder.Property(s => s.ServisAdi)
                .IsRequired()
                .HasMaxLength(150)
                .HasComment("Servis adı");

            builder.Property(s => s.Aktiflik)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif)
                .HasComment("Servis aktiflik durumu");

            builder.Property(s => s.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(s => s.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(s => s.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(s => s.ServisAdi)
                .IsUnique()
                .HasDatabaseName("IX_PER_Servisler_ServisAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasIndex(s => s.Aktiflik)
                .HasDatabaseName("IX_PER_Servisler_Aktiflik");

            builder.HasQueryFilter(s => !s.SilindiMi);

            builder.HasMany(s => s.Personeller)
                .WithOne(p => p.Servis)
                .HasForeignKey(p => p.ServisId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PER_Personeller_PER_Servisler");
        }
    }
}