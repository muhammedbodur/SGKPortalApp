using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class PersonelImzaYetkisiConfiguration : IEntityTypeConfiguration<PersonelImzaYetkisi>
    {
        public void Configure(EntityTypeBuilder<PersonelImzaYetkisi> builder)
        {
            builder.ToTable("PER_PersonelImzaYetkileri", "dbo");

            builder.HasKey(piy => piy.PersonelImzaYetkisiId);

            builder.Property(piy => piy.PersonelImzaYetkisiId)
                .ValueGeneratedOnAdd();

            builder.Property(piy => piy.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(piy => piy.DepartmanId)
                .IsRequired();

            builder.Property(piy => piy.ServisId)
                .IsRequired();

            builder.Property(piy => piy.GorevDegisimSebebi)
                .HasMaxLength(200);

            builder.Property(piy => piy.ImzaYetkisiBaslamaTarihi)
                .IsRequired();

            builder.Property(piy => piy.Aciklama)
                .HasMaxLength(500);

            builder.Property(piy => piy.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(piy => piy.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(piy => piy.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(piy => piy.TcKimlikNo)
                .HasDatabaseName("IX_PER_PersonelImzaYetkileri_TcKimlikNo");

            builder.HasQueryFilter(piy => !piy.SilindiMi);

            // Relationships
            builder.HasOne(piy => piy.Personel)
                .WithMany(p => p.PersonelImzaYetkileri)
                .HasForeignKey(piy => piy.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PER_PersonelImzaYetkileri_PER_Personeller");

            builder.HasOne(piy => piy.Departman)
                .WithMany()
                .HasForeignKey(piy => piy.DepartmanId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_PER_PersonelImzaYetkileri_PER_Departmanlar");

            builder.HasOne(piy => piy.Servis)
                .WithMany()
                .HasForeignKey(piy => piy.ServisId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_PER_PersonelImzaYetkileri_PER_Servisler");
        }
    }
}
