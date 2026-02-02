namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Elasticsearch
{
    /// <summary>
    /// SQL -> Elasticsearch senkronizasyon servisi interface
    /// </summary>
    public interface IPersonelIndexSyncService
    {
        /// <summary>
        /// Tüm personelleri yeniden indexler
        /// Index'i siler ve baştan oluşturur
        /// </summary>
        Task<int> FullReindexAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli bir tarihten sonra güncellenen personelleri indexler
        /// Incremental sync için
        /// </summary>
        Task<int> IncrementalSyncAsync(DateTime sinceDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tek bir personeli senkronize eder
        /// Personel oluşturulduğunda veya güncellendiğinde çağrılır
        /// </summary>
        Task<bool> SyncPersonelAsync(string tcKimlikNo);

        /// <summary>
        /// Personeli index'ten kaldırır
        /// Personel silindiğinde çağrılır
        /// </summary>
        Task<bool> RemovePersonelAsync(string tcKimlikNo);

        /// <summary>
        /// Index durumu hakkında bilgi verir
        /// </summary>
        Task<IndexStatusInfo> GetIndexStatusAsync();
    }

    /// <summary>
    /// Index durum bilgisi
    /// </summary>
    public class IndexStatusInfo
    {
        public bool IsAvailable { get; set; }
        public bool IndexExists { get; set; }
        public long DocumentCount { get; set; }
        public long SqlRecordCount { get; set; }
        public DateTime? LastSyncTime { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
