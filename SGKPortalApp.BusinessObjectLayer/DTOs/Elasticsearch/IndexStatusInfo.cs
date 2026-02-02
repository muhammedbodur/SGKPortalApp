namespace SGKPortalApp.BusinessObjectLayer.DTOs.Elasticsearch
{
    /// <summary>
    /// Elasticsearch index durum bilgisi
    /// </summary>
    public class IndexStatusInfo
    {
        /// <summary>
        /// Elasticsearch bağlantısı aktif mi?
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Index mevcut mu?
        /// </summary>
        public bool IndexExists { get; set; }

        /// <summary>
        /// Elasticsearch'teki toplam doküman sayısı
        /// </summary>
        public long DocumentCount { get; set; }

        /// <summary>
        /// SQL'deki toplam personel sayısı
        /// </summary>
        public long SqlRecordCount { get; set; }

        /// <summary>
        /// Hata mesajı (varsa)
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
