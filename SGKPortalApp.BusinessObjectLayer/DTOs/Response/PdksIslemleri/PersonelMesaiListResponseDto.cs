using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri
{
    /// <summary>
    /// Personel Mesai Listesi Response DTO
    /// PDKS.Net4.8: sp_PersonelMesaiBilgisiToplu stored procedure sonucu
    /// </summary>
    public class PersonelMesaiListResponseDto
    {
        public DateTime Tarih { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public int SicilNo { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public string? ServisAdi { get; set; }
        public TimeSpan? GirisSaati { get; set; }
        public TimeSpan? CikisSaati { get; set; }
        public string MesaiSuresi { get; set; } = string.Empty; // HH:MM format
        public int? MesaiSuresiDakika { get; set; }
        public string? Detay { get; set; } // Mazeret/İzin/Hafta Sonu bilgisi
        public bool HaftaSonu { get; set; }
        public bool GecKalma { get; set; }
        public string? IzinTipi { get; set; }
        public string? MazeretTipi { get; set; }
    }
}
