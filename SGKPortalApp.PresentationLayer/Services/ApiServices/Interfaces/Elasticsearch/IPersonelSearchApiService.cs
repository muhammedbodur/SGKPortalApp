using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Elasticsearch;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Elasticsearch
{
    /// <summary>
    /// Elasticsearch personel arama API servisi
    /// Fuzzy search, Türkçe karakter toleransı
    /// </summary>
    public interface IPersonelSearchApiService
    {
        /// <summary>
        /// Fuzzy personel araması
        /// </summary>
        /// <param name="query">Arama terimi (min 2 karakter)</param>
        /// <param name="departmanIds">Yetki bazlı departman filtreleri (virgülle ayrılmış)</param>
        /// <param name="sadeceAktif">Sadece aktif personeller (default: true)</param>
        /// <param name="size">Maksimum sonuç sayısı (default: 20)</param>
        Task<ServiceResult<List<PersonelElasticDto>>> SearchAsync(
            string query,
            string? departmanIds = null,
            bool sadeceAktif = true,
            int size = 20);

        /// <summary>
        /// Autocomplete araması
        /// Yazdıkça sonuç gösterimi için
        /// </summary>
        Task<ServiceResult<List<PersonelElasticDto>>> AutocompleteAsync(
            string prefix,
            string? departmanIds = null,
            bool sadeceAktif = true,
            int size = 10);

        /// <summary>
        /// Elasticsearch bağlantı testi
        /// </summary>
        Task<ServiceResult<bool>> PingAsync();
    }
}
