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
        public int DepartmanHizmetBinasiId { get; set; }
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;
        public int? BankoId { get; set; }
        public int KanalAltIslemId { get; set; } // SignalR broadcast için gerekli
        public PersonelUzmanlik Uzmanlik { get; set; }

        // ⭐ Incremental update için (GetBankoPanelBekleyenSiralarBySiraIdAsync)
        public string? PersonelTc { get; set; } // Bu sırayı görebilecek personel TC
        public string? ConnectionId { get; set; } // SignalR ConnectionId (direkt mesaj için)

        // Yönlendirme bilgileri (Banko panelinde bilgilendirici metin göstermek için)
        public bool YonlendirildiMi { get; set; }
        public string? YonlendirenPersonelTc { get; set; }
        public string? YonlendirenPersonelAdSoyad { get; set; }
        public YonlendirmeTipi? YonlendirmeTipi { get; set; }
        public string? YonlendirmeAciklamasi { get; set; }
    }
}
