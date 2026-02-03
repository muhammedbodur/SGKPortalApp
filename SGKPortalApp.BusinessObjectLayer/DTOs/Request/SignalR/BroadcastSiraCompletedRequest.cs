namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR
{
    /// <summary>
    /// Sıra tamamlama broadcast request
    /// </summary>
    public class BroadcastSiraCompletedRequest
    {
        public int SiraId { get; set; }
        public int HizmetBinasiId { get; set; }
        public int KanalAltIslemId { get; set; }
    }
}
