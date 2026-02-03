using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Elasticsearch;

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
}
