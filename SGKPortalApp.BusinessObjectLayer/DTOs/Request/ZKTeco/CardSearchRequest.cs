namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco
{
    /// <summary>
    /// Kart sorgulama isteği
    /// </summary>
    public class CardSearchRequest
    {
        /// <summary>
        /// Kart numarası
        /// </summary>
        public long CardNumber { get; set; }

        /// <summary>
        /// Sadece belirli bir cihazda ara (opsiyonel)
        /// </summary>
        public int? DeviceId { get; set; }

        /// <summary>
        /// Tüm cihazlarda ara mı?
        /// </summary>
        public bool SearchAllDevices { get; set; } = false;

        /// <summary>
        /// Uyuşmazlıkları detaylı göster
        /// </summary>
        public bool IncludeMismatches { get; set; } = true;

        /// <summary>
        /// Personel bilgilerini de getir
        /// </summary>
        public bool IncludePersonelInfo { get; set; } = true;
    }
}
