using SGKPortalApp.BusinessObjectLayer.DTOs.Elasticsearch;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.Elasticsearch
{
    /// <summary>
    /// Elasticsearch personel arama servisi interface
    /// </summary>
    public interface IPersonelSearchService
    {
        /// <summary>
        /// Elasticsearch index'ini oluşturur (yoksa)
        /// </summary>
        Task<bool> CreateIndexAsync();

        /// <summary>
        /// Index'in var olup olmadığını kontrol eder
        /// </summary>
        Task<bool> IndexExistsAsync();

        /// <summary>
        /// Index'i siler ve yeniden oluşturur
        /// </summary>
        Task<bool> RecreateIndexAsync();

        /// <summary>
        /// Tek bir personeli indexler
        /// </summary>
        Task<bool> IndexPersonelAsync(PersonelElasticDto personel);

        /// <summary>
        /// Birden fazla personeli toplu indexler
        /// </summary>
        Task<int> BulkIndexAsync(IEnumerable<PersonelElasticDto> personeller);

        /// <summary>
        /// Personeli index'ten siler
        /// </summary>
        Task<bool> DeletePersonelAsync(string tcKimlikNo);

        /// <summary>
        /// Fuzzy search ile personel arar
        /// Türkçe karakter ve yanlış yazım toleranslı
        /// </summary>
        /// <param name="searchTerm">Arama terimi</param>
        /// <param name="departmanIds">Yetki bazlı departman filtreleri (null = tüm departmanlar)</param>
        /// <param name="sadecAktif">Sadece aktif personel mi?</param>
        /// <param name="size">Maksimum sonuç sayısı</param>
        Task<IEnumerable<PersonelElasticDto>> SearchAsync(
            string searchTerm,
            IEnumerable<int>? departmanIds = null,
            bool sadeceAktif = true,
            int size = 20);

        /// <summary>
        /// Autocomplete için önek araması
        /// </summary>
        Task<IEnumerable<PersonelElasticDto>> AutocompleteAsync(
            string prefix,
            IEnumerable<int>? departmanIds = null,
            bool sadeceAktif = true,
            int size = 10);

        /// <summary>
        /// Index'teki toplam doküman sayısını döner
        /// </summary>
        Task<long> GetDocumentCountAsync();

        /// <summary>
        /// Elasticsearch bağlantısını test eder
        /// </summary>
        Task<bool> PingAsync();

        /// <summary>
        /// Index'i tamamen siler
        /// </summary>
        Task<bool> DeleteIndexAsync();
    }
}
