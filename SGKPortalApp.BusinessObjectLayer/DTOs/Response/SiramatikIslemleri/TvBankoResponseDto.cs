using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri
{
    public class TvBankoResponseDto
    {
        public int TvBankoId { get; set; }
        public int TvId { get; set; }
        public string? TvAdi { get; set; }
        public int BankoId { get; set; }
        public int BankoNo { get; set; }
        public string? BankoAdi { get; set; }
        public Aktiflik Aktiflik { get; set; }
        public DateTime EklenmeTarihi { get; set; }
    }
}
