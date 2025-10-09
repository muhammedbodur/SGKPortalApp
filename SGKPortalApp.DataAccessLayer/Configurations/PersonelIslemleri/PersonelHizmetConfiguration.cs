using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class PersonelHizmetConfiguration : IEntityTypeConfiguration<PersonelHizmet>
    {
        public void Configure(EntityTypeBuilder<PersonelHizmet> builder)
        {
            builder.ToTable("PER_PersonelHizmetleri", "dbo");

            builder.HasKey(ph => ph.PersonelHizmetId);

            builder.Property(ph => ph.PersonelHizmetId)
                .ValueGeneratedOnAdd();

            builder.Property(ph => ph.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(ph => ph.DepartmanId)
                .IsRequired();

            builder.Property(ph => ph.ServisId)
                .IsRequired();

            builder.Property(ph => ph.GorevBaslamaTarihi)
                .IsRequired();

            builder.Property(ph => ph.Sebep)
                .HasMaxLength(500);

            builder.Property(ph => ph.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(ph => ph.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(ph => ph.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(ph => ph.TcKimlikNo)
                .HasDatabaseName("IX_PER_PersonelHizmetleri_TcKimlikNo");

            builder.HasQueryFilter(ph => !ph.SilindiMi);

            // Relationships
            builder.HasOne(ph => ph.Personel)
                .WithMany(p => p.PersonelHizmetleri)
                .HasForeignKey(ph => ph.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PER_PersonelHizmetleri_PER_Personeller");

            builder.HasOne(ph => ph.Departman)
                .WithMany()
                .HasForeignKey(ph => ph.DepartmanId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_PER_PersonelHizmetleri_PER_Departmanlar");

            builder.HasOne(ph => ph.Servis)
                .WithMany()
                .HasForeignKey(ph => ph.ServisId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_PER_PersonelHizmetleri_PER_Servisler");
        }
    }
}
