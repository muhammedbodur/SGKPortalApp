using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class UnvanConfiguration : IEntityTypeConfiguration<Unvan>
    {
        public void Configure(EntityTypeBuilder<Unvan> builder)
        {
            builder.ToTable("PER_Unvanlar", "dbo");

            builder.HasKey(u => u.UnvanId);

            builder.Property(u => u.UnvanId)
                .ValueGeneratedOnAdd();

            builder.Property(u => u.UnvanAdi)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Unvan adı");

            builder.Property(u => u.UnvanAktiflik)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif)
                .HasComment("Unvan aktiflik durumu");

            builder.Property(u => u.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(u => u.UnvanAdi)
                .IsUnique()
                .HasDatabaseName("IX_PER_Unvanlar_UnvanAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasIndex(u => u.UnvanAktiflik)
                .HasDatabaseName("IX_PER_Unvanlar_UnvanAktiflik");

            builder.HasQueryFilter(u => !u.SilindiMi);

            builder.HasMany(u => u.Personeller)
                .WithOne(p => p.Unvan)
                .HasForeignKey(p => p.UnvanId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PER_Personeller_PER_Unvanlar");
        }
    }
}