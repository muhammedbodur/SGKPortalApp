using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    /// <summary>
    /// Kiosk için menü detay bilgilerini içeren DTO
    /// Aliağa SGM gibi belirli bir kiosk için menülerin personel durumları ile birlikte döner
    /// </summary>
    public class KioskMenuDetayResponseDto
    {
        // Departman-Hizmet Binası Bilgileri
        public int DepartmanHizmetBinasiId { get; set; }
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
        public string HizmetBinasiAdi { get; set; } = string.Empty;

        // Kiosk Bilgileri
        public int KioskId { get; set; }
        public string KioskAdi { get; set; } = string.Empty;
        public string? KioskIp { get; set; }

        // Menü Bilgileri
        public int KioskMenuId { get; set; }
        public string MenuAdi { get; set; } = string.Empty;
        public string? MenuAciklama { get; set; }
        public int MenuSiraNo { get; set; }

        // Menü Atama Bilgileri
        public int KioskMenuAtamaId { get; set; }
        public DateTime AtamaTarihi { get; set; }

        // Aggregate Bilgiler
        public int ToplamIslemSayisi { get; set; }
        public int ToplamKanalAltSayisi { get; set; }
        public int ToplamPersonelSayisi { get; set; }
    }
}
