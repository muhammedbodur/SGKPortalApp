using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class PersonelEngelConfiguration : IEntityTypeConfiguration<PersonelEngel>
    {
        public void Configure(EntityTypeBuilder<PersonelEngel> builder)
        {
            builder.ToTable("PER_PersonelEngelleri", "dbo");

            builder.HasKey(pe => pe.PersonelEngelId);

            builder.Property(pe => pe.PersonelEngelId)
                .ValueGeneratedOnAdd();

            builder.Property(pe => pe.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(pe => pe.EngelDerecesi)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(pe => pe.EngelNedeni1)
                .HasMaxLength(200);

            builder.Property(pe => pe.EngelNedeni2)
                .HasMaxLength(200);

            builder.Property(pe => pe.EngelNedeni3)
                .HasMaxLength(200);

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
                .HasDatabaseName("IX_PER_PersonelEngelleri_TcKimlikNo");

            builder.HasQueryFilter(pe => !pe.SilindiMi);

            // Relationship
            builder.HasOne(pe => pe.Personel)
                .WithMany(p => p.PersonelEngelleri)
                .HasForeignKey(pe => pe.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PER_PersonelEngelleri_PER_Personeller");
        }
    }
}
