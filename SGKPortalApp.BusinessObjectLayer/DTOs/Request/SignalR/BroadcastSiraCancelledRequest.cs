namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR
{
    /// <summary>
    /// Sıra iptal broadcast request
    /// </summary>
    public class BroadcastSiraCancelledRequest
    {
        public int SiraId { get; set; }
        public int HizmetBinasiId { get; set; }
        public int KanalAltIslemId { get; set; }
    }
}
