using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class PersonelCezaConfiguration : IEntityTypeConfiguration<PersonelCeza>
    {
        public void Configure(EntityTypeBuilder<PersonelCeza> builder)
        {
            builder.ToTable("PER_PersonelCezalari", "dbo");

            builder.HasKey(pc => pc.PersonelCezaId);

            builder.Property(pc => pc.PersonelCezaId)
                .ValueGeneratedOnAdd();

            builder.Property(pc => pc.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(pc => pc.CezaSebebi)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(pc => pc.AltBendi)
                .HasMaxLength(200);

            builder.Property(pc => pc.CezaTarihi)
                .IsRequired();

            builder.Property(pc => pc.Aciklama)
                .HasMaxLength(500);

            builder.Property(pc => pc.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(pc => pc.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(pc => pc.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(pc => pc.TcKimlikNo)
                .HasDatabaseName("IX_PER_PersonelCezalari_TcKimlikNo");

            builder.HasQueryFilter(pc => !pc.SilindiMi);

            // Relationship
            builder.HasOne(pc => pc.Personel)
                .WithMany(p => p.PersonelCezalari)
                .HasForeignKey(pc => pc.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PER_PersonelCezalari_PER_Personeller");
        }
    }
}
