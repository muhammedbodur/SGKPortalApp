using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class PersonelCocukConfiguration : IEntityTypeConfiguration<PersonelCocuk>
    {
        public void Configure(EntityTypeBuilder<PersonelCocuk> builder)
        {
            builder.ToTable("PER_PersonelCocuklari", "dbo");

            builder.HasKey(pc => pc.PersonelCocukId);

            builder.Property(pc => pc.PersonelCocukId)
                .ValueGeneratedOnAdd();

            builder.Property(pc => pc.PersonelTcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(pc => pc.CocukAdi)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pc => pc.OgrenimDurumu)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(pc => pc.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(pc => pc.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(pc => pc.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(pc => pc.PersonelTcKimlikNo)
                .HasDatabaseName("IX_PER_PersonelCocuklari_TcKimlikNo");

            builder.HasQueryFilter(pc => !pc.SilindiMi);

            // Relationship - CASCADE DELETE (çocuk entity)
            builder.HasOne(pc => pc.Personel)
                .WithMany(p => p.PersonelCocuklari)
                .HasForeignKey(pc => pc.PersonelTcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PER_PersonelCocuklari_PER_Personeller");
        }
    }
}