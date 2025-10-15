using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// Sıra Çağırma Paneli Response DTO
    /// </summary>
    public class SiraCagirmaResponseDto
    {
        public int SiraId { get; set; }
        public int SiraNo { get; set; }
        public string KanalAltAdi { get; set; } = string.Empty;
        public BeklemeDurum BeklemeDurum { get; set; }
        public DateTime SiraAlisZamani { get; set; }
        public DateTime? IslemBaslamaZamani { get; set; }
        public string? PersonelAdSoyad { get; set; }
        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;
    }
}
