using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PersonelIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    public class PersonelConfiguration : IEntityTypeConfiguration<Personel>
    {
        public void Configure(EntityTypeBuilder<Personel> builder)
        {
            builder.ToTable("PER_Personeller", "dbo");

            builder.HasKey(p => p.TcKimlikNo);

            builder.Property(p => p.TcKimlikNo)
                .HasMaxLength(11)
                .IsRequired()
                .HasComment("TC Kimlik Numarası - Primary Key");

            builder.Property(p => p.SicilNo)
                .IsRequired()
                .HasComment("Personel sicil numarası");

            builder.Property(p => p.AdSoyad)
                .IsRequired()
                .HasMaxLength(200)
                .HasComment("Ad Soyad");

            builder.Property(p => p.Email)
                .HasMaxLength(100)
                .HasComment("E-posta adresi");

            builder.Property(p => p.CepTelefonu)
                .HasMaxLength(20)
                .HasComment("Cep telefonu numarası");

            builder.Property(p => p.Adres)
                .HasMaxLength(500)
                .HasComment("Adres bilgisi");

            builder.Property(p => p.NickName)
                .HasMaxLength(50);

            builder.Property(p => p.Gorev)
                .HasMaxLength(100);

            builder.Property(p => p.Uzmanlik)
                .HasMaxLength(100);

            builder.Property(p => p.Resim)
                .HasMaxLength(255);

            // Enum conversions
            builder.Property(p => p.Cinsiyet)
                .HasConversion<string>()
                .HasMaxLength(10);

            builder.Property(p => p.PersonelTipi)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.PersonelAktiflikDurum)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.MedeniDurumu)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.KanGrubu)
                .HasConversion<string>()
                .HasMaxLength(10);

            builder.Property(p => p.OgrenimDurumu)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(p => p.EvDurumu)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.EsininIsDurumu)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.SehitYakinligi)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.KartTipi)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            // Auditing
            builder.Property(p => p.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // Indexes
            builder.HasIndex(p => p.TcKimlikNo)
                .IsUnique()
                .HasDatabaseName("IX_PER_Personeller_TcKimlikNo");

            builder.HasIndex(p => p.SicilNo)
                .IsUnique()
                .HasDatabaseName("IX_PER_Personeller_SicilNo");

            builder.HasIndex(p => p.Email)
                .IsUnique()
                .HasDatabaseName("IX_PER_Personeller_Email")
                .HasFilter("[Email] IS NOT NULL AND [SilindiMi] = 0");

            builder.HasIndex(p => new { p.DepartmanId, p.PersonelAktiflikDurum })
                .HasDatabaseName("IX_PER_Personeller_Departman_Aktiflik");

            builder.HasIndex(p => p.PersonelAktiflikDurum)
                .HasDatabaseName("IX_PER_Personeller_AktiflikDurum");

            builder.HasIndex(p => new { p.HizmetBinasiId, p.PersonelAktiflikDurum })
                .HasDatabaseName("IX_PER_Personeller_HizmetBinasi_Aktiflik");

            // Query Filter
            builder.HasQueryFilter(p => !p.SilindiMi);

            // Relationships (zaten SGKDbContext'te tanımlı, çakışma olmaması için yorum satırı)
            // builder.HasOne(p => p.Departman)...
            // builder.HasOne(p => p.Servis)...
            // vs. SGKDbContext'teki ConfigureOnlyNonConflictingRelationships metodunda tanımlı
        }
    }
}