using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class PersonelEgitimConfiguration : IEntityTypeConfiguration<PersonelEgitim>
    {
        public void Configure(EntityTypeBuilder<PersonelEgitim> builder)
        {
            builder.ToTable("PER_PersonelEgitimleri", "dbo");

            builder.HasKey(pe => pe.PersonelEgitimId);

            builder.Property(pe => pe.PersonelEgitimId)
                .ValueGeneratedOnAdd();

            builder.Property(pe => pe.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(pe => pe.EgitimAdi)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(pe => pe.EgitimBaslangicTarihi)
                .IsRequired();

            builder.Property(pe => pe.Aciklama)
                .HasMaxLength(500);

            builder.Property(pe => pe.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(pe => pe.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(pe => pe.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(pe => pe.TcKimlikNo)
                .HasDatabaseName("IX_PER_PersonelEgitimleri_TcKimlikNo");

            builder.HasQueryFilter(pe => !pe.SilindiMi);

            // Relationship
            builder.HasOne(pe => pe.Personel)
                .WithMany(p => p.PersonelEgitimleri)
                .HasForeignKey(pe => pe.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PER_PersonelEgitimleri_PER_Personeller");
        }
    }
}
