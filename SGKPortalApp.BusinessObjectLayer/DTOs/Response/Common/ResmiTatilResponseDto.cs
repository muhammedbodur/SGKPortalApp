using System;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class ResmiTatilResponseDto
    {
        public int TatilId { get; set; }
        public string TatilAdi { get; set; } = string.Empty;
        public DateTime Tarih { get; set; }
        public int Yil { get; set; }
        public TatilTipi TatilTipi { get; set; }
        public string TatilTipiText { get; set; } = string.Empty;
        public bool YariGun { get; set; }
        public string? Aciklama { get; set; }
        public bool OtomatikSenkronize { get; set; }
        
        // Audit fields
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
        public string? EkleyenKullanici { get; set; }
        public string? DuzenleyenKullanici { get; set; }
    }
}
