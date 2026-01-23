using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.Collections.Generic;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    public class KioskResponseDto
    {
        public int KioskId { get; set; }
        public string KioskAdi { get; set; } = string.Empty;
        public int DepartmanHizmetBinasiId { get; set; }
        public int DepartmanId { get; set; }
        public string DepartmanAdi { get; set; } = string.Empty;
        public int HizmetBinasiId { get; set; }
        public string? HizmetBinasiAdi { get; set; }
        public string? KioskIp { get; set; }
        public Aktiflik Aktiflik { get; set; }
        
        // Atanmış menü bilgisi
        public int? AtananKioskMenuId { get; set; }
        public string? AtananKioskMenuAdi { get; set; }
    }
}
