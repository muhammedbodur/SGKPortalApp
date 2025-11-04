using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class BankoHareketConfiguration : IEntityTypeConfiguration<BankoHareket>
    {
        public void Configure(EntityTypeBuilder<BankoHareket> builder)
        {
            builder.ToTable("SIR_BankoHareketleri", "dbo");

            builder.HasKey(bh => bh.BankoHareketId);

            builder.Property(bh => bh.BankoHareketId)
                .ValueGeneratedOnAdd();

            builder.Property(bh => bh.PersonelTcKimlikNo)
                .IsRequired()
                .HasMaxLength(11)
                .HasComment("İşlemi yapan personel TC");

            builder.Property(bh => bh.SiraNo)
                .IsRequired()
                .HasComment("Sıra numarası");

            builder.Property(bh => bh.IslemBaslamaZamani)
                .IsRequired()
                .HasComment("İşlem başlama zamanı");

            builder.Property(bh => bh.IslemBitisZamani)
                .HasComment("İşlem bitiş zamanı (NULL = hala işlemde)");

            builder.Property(bh => bh.IslemSuresiSaniye)
                .HasComment("İşlem süresi (saniye)");

            builder.Property(bh => bh.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(bh => bh.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(bh => bh.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(bh => bh.BankoId)
                .HasDatabaseName("IX_SIR_BankoHareketleri_BankoId");

            builder.HasIndex(bh => bh.PersonelTcKimlikNo)
                .HasDatabaseName("IX_SIR_BankoHareketleri_PersonelTc");

            builder.HasIndex(bh => bh.SiraId)
                .HasDatabaseName("IX_SIR_BankoHareketleri_SiraId");

            builder.HasIndex(bh => bh.IslemBaslamaZamani)
                .HasDatabaseName("IX_SIR_BankoHareketleri_IslemBaslamaZamani");

            builder.HasQueryFilter(bh => !bh.SilindiMi);

            // Relationships
            builder.HasOne(bh => bh.Banko)
                .WithMany(b => b.BankoHareketleri)
                .HasForeignKey(bh => bh.BankoId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_BankoHareketleri_SIR_Bankolar");

            builder.HasOne(bh => bh.Personel)
                .WithMany()
                .HasForeignKey(bh => bh.PersonelTcKimlikNo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_BankoHareketleri_PER_Personeller");

            builder.HasOne(bh => bh.Sira)
                .WithMany(s => s.BankoHareketleri)
                .HasForeignKey(bh => bh.SiraId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_BankoHareketleri_SIR_Siralar");

            builder.HasOne(bh => bh.KanalIslem)
                .WithMany()
                .HasForeignKey(bh => bh.KanalIslemId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_BankoHareketleri_SIR_KanalIslemleri");

            builder.HasOne(bh => bh.KanalAltIslem)
                .WithMany()
                .HasForeignKey(bh => bh.KanalAltIslemId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_BankoHareketleri_SIR_KanalAltIslemleri");
        }
    }
}
