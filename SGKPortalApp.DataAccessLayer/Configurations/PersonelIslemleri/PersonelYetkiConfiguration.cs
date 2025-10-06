using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class PersonelYetkiConfiguration : IEntityTypeConfiguration<PersonelYetki>
    {
        public void Configure(EntityTypeBuilder<PersonelYetki> builder)
        {
            builder.ToTable("PER_PersonelYetkileri", "dbo");

            builder.HasKey(py => py.PersonelYetkiId);

            builder.Property(py => py.PersonelYetkiId)
                .ValueGeneratedOnAdd();

            builder.Property(py => py.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(py => py.YetkiTipleri)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(py => py.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(py => py.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(py => py.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(py => new { py.TcKimlikNo, py.YetkiId })
                .IsUnique()
                .HasDatabaseName("IX_PER_PersonelYetkileri_Tc_Yetki")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(py => !py.SilindiMi);

            // Relationships
            builder.HasOne(py => py.Personel)
                .WithMany()
                .HasForeignKey(py => py.TcKimlikNo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PER_PersonelYetkileri_PER_Personeller");

            builder.HasOne(py => py.Yetki)
                .WithMany()
                .HasForeignKey(py => py.YetkiId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PER_PersonelYetkileri_PER_Yetkiler");
        }
    }
}