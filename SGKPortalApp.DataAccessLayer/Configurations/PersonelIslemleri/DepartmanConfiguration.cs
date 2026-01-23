using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class DepartmanConfiguration : IEntityTypeConfiguration<Departman>
    {
        public void Configure(EntityTypeBuilder<Departman> builder)
        {
            // Table
            builder.ToTable("PER_Departmanlar", "dbo");

            // Primary Key
            builder.HasKey(d => d.DepartmanId);

            // Properties
            builder.Property(d => d.DepartmanId)
                .ValueGeneratedOnAdd();

            builder.Property(d => d.DepartmanAdi)
                .IsRequired()
                .HasMaxLength(150)
                .HasComment("Departman adı");

            builder.Property(d => d.DepartmanAdiKisa)
                .HasMaxLength(50)
                .HasComment("Departman kısa adı");

            builder.Property(d => d.Aktiflik)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(Aktiflik.Aktif)
                .HasComment("Departman aktiflik durumu (0: Pasif, 1: Aktif)");

            // Auditing
            builder.Property(d => d.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Kayıt oluşturulma tarihi");

            builder.Property(d => d.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasComment("Son güncelleme tarihi");

            builder.Property(d => d.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Soft delete flag");

            // Indexes
            builder.HasIndex(d => d.DepartmanAdi)
                .IsUnique()
                .HasDatabaseName("IX_PER_Departmanlar_DepartmanAdi")
                .HasFilter("[SilindiMi] = 0");

            builder.HasIndex(d => d.Aktiflik)
                .HasDatabaseName("IX_PER_Departmanlar_Aktiflik");

            builder.HasIndex(d => new { d.Aktiflik, d.SilindiMi })
                .HasDatabaseName("IX_PER_Departmanlar_Aktiflik_SilindiMi");

            // Soft Delete Query Filter
            builder.HasQueryFilter(d => !d.SilindiMi);

            // Relationships
            builder.HasMany(d => d.Personeller)
                .WithOne(p => p.Departman)
                .HasForeignKey(p => p.DepartmanId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PER_Personeller_PER_Departmanlar");

            // Many-to-many ilişki DepartmanHizmetBinasi üzerinden (DepartmanHizmetBinasiConfiguration'da tanımlı)
        }
    }
}