using System;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// İzin/Mazeret talebi detay Response DTO
    /// Tam bilgi ile döndürür (detay sayfası için)
    /// </summary>
    public class IzinMazeretTalepResponseDto
    {
        public int IzinMazeretTalepId { get; set; }

        // ═══════════════════════════════════════════════════════
        // PERSONEL BİLGİLERİ
        // ═══════════════════════════════════════════════════════

        public string TcKimlikNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public int SicilNo { get; set; }
        public string? DepartmanAdi { get; set; }
        public string? ServisAdi { get; set; }

        // ═══════════════════════════════════════════════════════
        // TALEP BİLGİLERİ
        // ═══════════════════════════════════════════════════════

        public IzinMazeretTuru Turu { get; set; }
        public string TuruAdi { get; set; } = string.Empty; // Display name
        public string? Aciklama { get; set; }
        public DateTime TalepTarihi { get; set; }
        public bool IsActive { get; set; }

        // ═══════════════════════════════════════════════════════
        // İZİN ALANLARI
        // ═══════════════════════════════════════════════════════

        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int? ToplamGun { get; set; }

        // ═══════════════════════════════════════════════════════
        // MAZERET ALANLARI
        // ═══════════════════════════════════════════════════════

        public DateTime? MazeretTarihi { get; set; }
        public string? SaatDilimi { get; set; }

        // ═══════════════════════════════════════════════════════
        // BİRİNCİ ONAYCI
        // ═══════════════════════════════════════════════════════

        public string? BirinciOnayciTcKimlikNo { get; set; }
        public string? BirinciOnayciAdSoyad { get; set; }
        public OnayDurumu BirinciOnayDurumu { get; set; }
        public string BirinciOnayDurumuAdi { get; set; } = string.Empty; // Display name
        public DateTime? BirinciOnayTarihi { get; set; }
        public string? BirinciOnayAciklama { get; set; }

        // ═══════════════════════════════════════════════════════
        // İKİNCİ ONAYCI
        // ═══════════════════════════════════════════════════════

        public string? IkinciOnayciTcKimlikNo { get; set; }
        public string? IkinciOnayciAdSoyad { get; set; }
        public OnayDurumu IkinciOnayDurumu { get; set; }
        public string IkinciOnayDurumuAdi { get; set; } = string.Empty; // Display name
        public DateTime? IkinciOnayTarihi { get; set; }
        public string? IkinciOnayAciklama { get; set; }

        // ═══════════════════════════════════════════════════════
        // EK BİLGİLER
        // ═══════════════════════════════════════════════════════

        public string? BelgeEki { get; set; }

        // Audit bilgileri
        public DateTime EklenmeTarihi { get; set; }
        public string? EkleyenKullanici { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
        public string? DuzenleyenKullanici { get; set; }
    }
}
