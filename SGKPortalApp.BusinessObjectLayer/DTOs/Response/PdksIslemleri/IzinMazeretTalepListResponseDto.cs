using System;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// İzin/Mazeret talebi liste Response DTO
    /// Liste görünümleri için hafif DTO (tablo görünümü)
    /// </summary>
    public class IzinMazeretTalepListResponseDto
    {
        public int IzinMazeretTalepId { get; set; }

        // Personel bilgileri
        public string TcKimlikNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public int SicilNo { get; set; }
        public string? DepartmanAdi { get; set; }
        public string? ServisAdi { get; set; }

        // Talep bilgileri
        public int IzinMazeretTuruId { get; set; }
        public string TuruAdi { get; set; } = string.Empty;
        public DateTime TalepTarihi { get; set; }

        // İzin/Mazeret tarihleri (birleştirilmiş display için)
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public DateTime? MazeretTarihi { get; set; }
        public TimeSpan? BaslangicSaati { get; set; }
        public TimeSpan? BitisSaati { get; set; }
        public int? ToplamGun { get; set; }

        // Onay durumları
        public OnayDurumu BirinciOnayDurumu { get; set; }
        public string BirinciOnayDurumuAdi { get; set; } = string.Empty;
        public OnayDurumu IkinciOnayDurumu { get; set; }
        public string IkinciOnayDurumuAdi { get; set; } = string.Empty;

        // Onaycı bilgileri
        public string? BirinciOnayciTcKimlikNo { get; set; }
        public string? BirinciOnayciAdSoyad { get; set; }
        public string? IkinciOnayciTcKimlikNo { get; set; }
        public string? IkinciOnayciAdSoyad { get; set; }

        // Genel durum göstergesi (UI için)
        public string GenelDurum { get; set; } = string.Empty; // "Beklemede", "Onaylandı", "Reddedildi", "İptal"
        public bool IsActive { get; set; }

        // SGK Sistem İşlem Takibi
        public bool IzinIslendiMi { get; set; }
        public DateTime? IzinIslemTarihi { get; set; }
        public string? IzinIslemNotlari { get; set; }
    }
}
