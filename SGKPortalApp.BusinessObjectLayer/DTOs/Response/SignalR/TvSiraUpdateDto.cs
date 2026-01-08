using System;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.SignalR
{
    /// <summary>
    /// TV ekranına basit sıra bildirimi (eski yapı için - receiveSiraUpdate)
    /// </summary>
    public class TvSiraUpdateDto
    {
        public int SiraNo { get; set; }
        public string BankoNo { get; set; } = string.Empty;
        public string KanalAltAdi { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
