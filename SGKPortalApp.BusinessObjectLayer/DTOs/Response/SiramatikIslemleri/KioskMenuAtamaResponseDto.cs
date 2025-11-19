using System;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    public class KioskMenuAtamaResponseDto
    {
        public int KioskMenuAtamaId { get; set; }
        public int KioskId { get; set; }
        public string? KioskAdi { get; set; }
        public string? HizmetBinasiAdi { get; set; }
        public int KioskMenuId { get; set; }
        public string? KioskMenuAdi { get; set; }
        public DateTime AtamaTarihi { get; set; }
        public Aktiflik Aktiflik { get; set; }
    }
}
