using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.PdksIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.PersonelIslemleri
{
    /// <summary>
    /// İzinMazeretTalep entity EF Core yapılandırması
    /// Tablo adı: PDKS_IzinMazeretTalep
    /// Tek tabloda İzin ve Mazeret talepleri
    /// </summary>
    public class IzinMazeretTalepConfiguration : IEntityTypeConfiguration<IzinMazeretTalep>
    {
        public void Configure(EntityTypeBuilder<IzinMazeretTalep> builder)
        {
            // ═══════════════════════════════════════════════════════
            // TABLO VE PRIMARY KEY
            // ═══════════════════════════════════════════════════════

            builder.ToTable("PDKS_IzinMazeretTalep", "dbo");

            builder.HasKey(t => t.IzinMazeretTalepId);

            builder.Property(t => t.IzinMazeretTalepId)
                .ValueGeneratedOnAdd();

            // ═══════════════════════════════════════════════════════
            // PERSONEL İLİŞKİSİ
            // ═══════════════════════════════════════════════════════

            builder.Property(t => t.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            // ═══════════════════════════════════════════════════════
            // TALEP BİLGİLERİ
            // ═══════════════════════════════════════════════════════

            builder.Property(t => t.IzinMazeretTuruId)
                .IsRequired();

            builder.HasOne(t => t.IzinMazeretTuru)
                .WithMany()
                .HasForeignKey(t => t.IzinMazeretTuruId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(t => t.Aciklama)
                .HasMaxLength(500);

            // ═══════════════════════════════════════════════════════
            // İZİN ALANLARI (Planlı izinler için)
            // ═══════════════════════════════════════════════════════

            builder.Property(t => t.BaslangicTarihi)
                .HasColumnType("date");

            builder.Property(t => t.BitisTarihi)
                .HasColumnType("date");

            builder.Property(t => t.ToplamGun);

            // ═══════════════════════════════════════════════════════
            // MAZERET ALANLARI (Geriye dönük mazeretler için)
            // ═══════════════════════════════════════════════════════

            builder.Property(t => t.MazeretTarihi)
                .HasColumnType("date");

            builder.Property(t => t.BaslangicSaati)
                .HasColumnType("time");

            builder.Property(t => t.BitisSaati)
                .HasColumnType("time");

            // ═══════════════════════════════════════════════════════
            // BİRİNCİ ONAYCI
            // ═══════════════════════════════════════════════════════

            builder.Property(t => t.BirinciOnayciTcKimlikNo)
                .HasMaxLength(11);

            builder.Property(t => t.BirinciOnayDurumu)
                .IsRequired()
                .HasDefaultValue(BusinessObjectLayer.Enums.PdksIslemleri.OnayDurumu.Beklemede)
                .HasConversion<int>();

            builder.Property(t => t.BirinciOnayAciklama)
                .HasMaxLength(500);

            // ═══════════════════════════════════════════════════════
            // İKİNCİ ONAYCI
            // ═══════════════════════════════════════════════════════

            builder.Property(t => t.IkinciOnayciTcKimlikNo)
                .HasMaxLength(11);

            builder.Property(t => t.IkinciOnayDurumu)
                .IsRequired()
                .HasDefaultValue(BusinessObjectLayer.Enums.PdksIslemleri.OnayDurumu.Beklemede)
                .HasConversion<int>();

            builder.Property(t => t.IkinciOnayAciklama)
                .HasMaxLength(500);

            // ═══════════════════════════════════════════════════════
            // EK BİLGİLER
            // ═══════════════════════════════════════════════════════

            builder.Property(t => t.TalepTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(t => t.IsActive)
                .IsRequired()
                .HasDefaultValue(true);


            // ═══════════════════════════════════════════════════════
            // AUDİT ALANLARI (AuditableEntity'den gelenler)
            // ═══════════════════════════════════════════════════════

            builder.Property(t => t.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(t => t.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(t => t.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            // ═══════════════════════════════════════════════════════
            // İNDEXLER
            // ═══════════════════════════════════════════════════════

            // TC Kimlik No index (sık sorgulanan)
            builder.HasIndex(t => t.TcKimlikNo)
                .HasDatabaseName("IX_PDKS_IzinMazeretTalep_TcKimlikNo");

            // Tarih aralığı index (rapor sorgularında kullanılır)
            builder.HasIndex(t => new { t.BaslangicTarihi, t.BitisTarihi })
                .HasDatabaseName("IX_PDKS_IzinMazeretTalep_TarihAralik");

            // Onay durumu index (onay bekleyen talepleri hızlı bulmak için)
            builder.HasIndex(t => new { t.BirinciOnayDurumu, t.IkinciOnayDurumu })
                .HasDatabaseName("IX_PDKS_IzinMazeretTalep_OnayDurumlari");

            // Türe göre index
            builder.HasIndex(t => t.IzinMazeretTuruId)
                .HasDatabaseName("IX_PDKS_IzinMazeretTalep_IzinMazeretTuruId");

            // Aktiflik index
            builder.HasIndex(t => t.IsActive)
                .HasDatabaseName("IX_PDKS_IzinMazeretTalep_IsActive");

            // ═══════════════════════════════════════════════════════
            // QUERY FİLTER (Soft Delete)
            // ═══════════════════════════════════════════════════════

            builder.HasQueryFilter(t => !t.SilindiMi);

            // ═══════════════════════════════════════════════════════
            // İLİŞKİLER
            // ═══════════════════════════════════════════════════════

            // Personel ile ilişki (talep sahibi)
            builder.HasOne(t => t.Personel)
                .WithMany(p => p.IzinMazeretTalepleri)
                .HasForeignKey(t => t.TcKimlikNo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PDKS_IzinMazeretTalep_PER_Personeller");

            // 1. Onayci ile ilişki (optional, cascade yok)
            builder.HasOne(t => t.BirinciOnayci)
                .WithMany()
                .HasForeignKey(t => t.BirinciOnayciTcKimlikNo)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_PDKS_IzinMazeretTalep_BirinciOnayci");

            // 2. Onayci ile ilişki (optional, cascade yok)
            builder.HasOne(t => t.IkinciOnayci)
                .WithMany()
                .HasForeignKey(t => t.IkinciOnayciTcKimlikNo)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_PDKS_IzinMazeretTalep_IkinciOnayci");
        }
    }
}
