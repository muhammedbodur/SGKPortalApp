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
                .HasComment("Personel sicil numarası (nullable)");

            builder.Property(p => p.AdSoyad)
                .IsRequired()
                .HasMaxLength(200)
                .HasComment("Ad Soyad");

            builder.Property(p => p.NickName)
                .IsRequired()
                .HasMaxLength(8)
                .HasComment("Cihazda görünecek kısa isim (max 8 karakter)");

            builder.Property(p => p.Email)
                .HasMaxLength(100)
                .HasComment("E-posta adresi");

            builder.Property(p => p.CepTelefonu)
                .HasMaxLength(20)
                .HasComment("Cep telefonu numarası");

            builder.Property(p => p.Adres)
                .HasMaxLength(500)
                .HasComment("Adres bilgisi");

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
                .HasFilter("[SicilNo] IS NOT NULL")  // NULL'ları unique constraint'ten hariç tut
                .HasDatabaseName("IX_PER_Personeller_SicilNo");

            builder.HasIndex(p => p.PersonelKayitNo)
                .IsUnique()
                .HasFilter("[PersonelKayitNo] IS NOT NULL")  // NULL'ları unique constraint'ten hariç tut
                .HasDatabaseName("IX_PER_Personeller_PersonelKayitNo");

            builder.HasIndex(p => p.KartNo)
                .HasDatabaseName("IX_PER_Personeller_KartNo");

            builder.HasIndex(p => p.DepartmanId)
                .HasDatabaseName("IX_PER_Personeller_DepartmanId");

            builder.HasQueryFilter(p => !p.SilindiMi);

            // ═══════════════════════════════════════════════════════
            // RELATIONSHIPS
            // ═══════════════════════════════════════════════════════

            // User ile One-to-One ilişki (Required)
            builder.HasOne(p => p.User)
                .WithOne(u => u.Personel)
                .HasForeignKey<Personel>(p => p.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired()
                .HasConstraintName("FK_PER_Personeller_CMN_Users");

            // Departman ile Many-to-One ilişki
            builder.HasOne(p => p.Departman)
                .WithMany(d => d.Personeller)
                .HasForeignKey(p => p.DepartmanId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired()
                .HasConstraintName("FK_PER_Personeller_PER_Departmanlar");

            // Servis ile Many-to-One ilişki
            builder.HasOne(p => p.Servis)
                .WithMany(s => s.Personeller)
                .HasForeignKey(p => p.ServisId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired()
                .HasConstraintName("FK_PER_Personeller_PER_Servisler");

            // Unvan ile Many-to-One ilişki
            builder.HasOne(p => p.Unvan)
                .WithMany(u => u.Personeller)
                .HasForeignKey(p => p.UnvanId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired()
                .HasConstraintName("FK_PER_Personeller_PER_Unvanlar");

            // AtanmaNedeni ile Many-to-One ilişki
            builder.HasOne(p => p.AtanmaNedeni)
                .WithMany(a => a.Personeller)
                .HasForeignKey(p => p.AtanmaNedeniId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired()
                .HasConstraintName("FK_PER_Personeller_PER_AtanmaNedenleri");

            // Il ile Many-to-One ilişki
            builder.HasOne(p => p.Il)
                .WithMany()
                .HasForeignKey(p => p.IlId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired()
                .HasConstraintName("FK_PER_Personeller_CMN_Iller");

            // Ilce ile Many-to-One ilişki
            builder.HasOne(p => p.Ilce)
                .WithMany()
                .HasForeignKey(p => p.IlceId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired()
                .HasConstraintName("FK_PER_Personeller_CMN_Ilceler");

            // Sendika ile Many-to-One ilişki (NULLABLE)
            builder.HasOne(p => p.Sendika)
                .WithMany(s => s.Personeller)
                .HasForeignKey(p => p.SendikaId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false)
                .HasConstraintName("FK_PER_Personeller_PER_Sendikalar");

            // EsininIsIl ile Many-to-One ilişki (NULLABLE)
            builder.HasOne(p => p.EsininIsIl)
                .WithMany()
                .HasForeignKey(p => p.EsininIsIlId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false)
                .HasConstraintName("FK_PER_Personeller_CMN_Iller_EsIsIl");

            // EsininIsIlce ile Many-to-One ilişki (NULLABLE)
            builder.HasOne(p => p.EsininIsIlce)
                .WithMany()
                .HasForeignKey(p => p.EsininIsIlceId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false)
                .HasConstraintName("FK_PER_Personeller_CMN_Ilceler_EsIsIlce");

            // DepartmanHizmetBinasi ile Many-to-One ilişki
            builder.HasOne(p => p.DepartmanHizmetBinasi)
                .WithMany(dhb => dhb.Personeller)
                .HasForeignKey(p => p.DepartmanHizmetBinasiId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired()
                .HasConstraintName("FK_PER_Personeller_PER_DepartmanHizmetBinalari");

            // One-to-Many ilişkiler
            builder.HasMany(p => p.PersonelCocuklari)
                .WithOne(pc => pc.Personel)
                .HasForeignKey(pc => pc.PersonelTcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PersonelYetkileri)
                .WithOne(py => py.Personel)
                .HasForeignKey(py => py.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.KanalPersonelleri)
                .WithOne(kp => kp.Personel)
                .HasForeignKey(kp => kp.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.BankoKullanicilari)
                .WithOne(bk => bk.Personel)
                .HasForeignKey(bk => bk.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade);

            // Diğer one-to-many ilişkiler
            builder.HasMany(p => p.PersonelHizmetleri)
                .WithOne(ph => ph.Personel)
                .HasForeignKey(ph => ph.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PersonelEgitimleri)
                .WithOne(pe => pe.Personel)
                .HasForeignKey(pe => pe.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PersonelImzaYetkileri)
                .WithOne(piy => piy.Personel)
                .HasForeignKey(piy => piy.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PersonelCezalari)
                .WithOne(pc => pc.Personel)
                .HasForeignKey(pc => pc.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PersonelEngelleri)
                .WithOne(pe => pe.Personel)
                .HasForeignKey(pe => pe.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.IzinMazeretTalepleri)
                .WithOne(imt => imt.Personel)
                .HasForeignKey(imt => imt.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade);

            // IzinSorumluluklar - Sorumlu personel için
            builder.HasMany(p => p.IzinSorumluluklar)
                .WithOne(is_rel => is_rel.SorumluPersonel)
                .HasForeignKey(is_rel => is_rel.SorumluPersonelTcKimlikNo)
                .OnDelete(DeleteBehavior.Restrict);  // Sorumluları silerken dikkatli olmalıyız
        }
    }
}