using System;
using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// SGM Mesai Raporu Response DTO
    /// </summary>
    public class SgmMesaiReportDto
    {
        public string SgmAdi { get; set; } = string.Empty;
        public string? ServisAdi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public List<PersonelMesaiOzetDto> Personeller { get; set; } = new();
    }

    /// <summary>
    /// Personel Mesai Özet DTO - SGM raporunda kullanılır
    /// </summary>
    public class PersonelMesaiOzetDto
    {
        public string TcKimlikNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public int SicilNo { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public string? ServisAdi { get; set; }
        public int ToplamGun { get; set; }
        public int CalistigiGun { get; set; }
        public int IzinliGun { get; set; }
        public int MazeretliGun { get; set; }
        public int DevamsizGun { get; set; }
        public int HaftaSonuCalisma { get; set; }
        public int GecKalma { get; set; }
        public string ToplamMesaiSuresi { get; set; } = "00:00"; // HH:MM format
        public int ToplamMesaiDakika { get; set; }
        public List<PersonelMesaiGunlukDto> GunlukDetay { get; set; } = new();
    }

    /// <summary>
    /// Personel Günlük Mesai DTO - Detay için
    /// </summary>
    public class PersonelMesaiGunlukDto
    {
        public DateTime Tarih { get; set; }
        public TimeSpan? GirisSaati { get; set; }
        public TimeSpan? CikisSaati { get; set; }
        public string MesaiSuresi { get; set; } = "-";
        public string? Durum { get; set; } // İzin, Mazeret, Hafta Sonu, vb.
        public bool HaftaSonu { get; set; }
        public bool GecKalma { get; set; }
    }
}
