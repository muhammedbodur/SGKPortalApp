namespace SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco
{
    /// <summary>
    /// Uyuşmazlık detayı
    /// </summary>
    public class MismatchDetail
    {
        /// <summary>
        /// Alan adı
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Cihazdaki değer
        /// </summary>
        public string? DeviceValue { get; set; }

        /// <summary>
        /// DB'deki değer
        /// </summary>
        public string? PersonelValue { get; set; }

        /// <summary>
        /// Uyuşmazlık tipi
        /// </summary>
        public MismatchType Type { get; set; }

        /// <summary>
        /// Açıklama
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
