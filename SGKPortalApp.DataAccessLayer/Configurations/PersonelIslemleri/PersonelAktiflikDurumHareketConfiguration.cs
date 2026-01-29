using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class PersonelAktiflikDurumHareketConfiguration : IEntityTypeConfiguration<PersonelAktiflikDurumHareket>
    {
        public void Configure(EntityTypeBuilder<PersonelAktiflikDurumHareket> builder)
        {
            builder.ToTable("PER_PersonelAktiflikDurumHareketleri", "dbo");

            builder.HasKey(h => h.PersonelAktiflikDurumHareketId);

            builder.Property(h => h.PersonelAktiflikDurumHareketId)
                .ValueGeneratedOnAdd();

            builder.Property(h => h.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(h => h.OncekiDurum)
                .HasConversion<int>();

            builder.Property(h => h.YeniDurum)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(h => h.DegisiklikTarihi)
                .IsRequired();

            builder.Property(h => h.Aciklama)
                .HasMaxLength(500);

            builder.Property(h => h.BelgeNo)
                .HasMaxLength(100);

            builder.Property(h => h.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(h => h.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(h => h.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(h => h.TcKimlikNo)
                .HasDatabaseName("IX_PER_PersonelAktiflikDurumHareketleri_TcKimlikNo");

            builder.HasIndex(h => h.DegisiklikTarihi)
                .HasDatabaseName("IX_PER_PersonelAktiflikDurumHareketleri_DegisiklikTarihi");

            builder.HasQueryFilter(h => !h.SilindiMi);

            // Relationship
            builder.HasOne(h => h.Personel)
                .WithMany(p => p.PersonelAktiflikDurumHareketleri)
                .HasForeignKey(h => h.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PER_PersonelAktiflikDurumHareketleri_PER_Personeller");
        }
    }
}
