using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik
{
    /// <summary>
    /// Sıra Çağırma API Servisi - Presentation Layer
    /// </summary>
    public interface ISiraCagirmaApiService
    {
        /// <summary>
        /// Beklemedeki sıraları getirir
        /// </summary>
        Task<List<SiraCagirmaResponseDto>> GetBekleyenSiralarAsync();

        /// <summary>
        /// Personelin bekleyen sıralarını getirir
        /// </summary>
        Task<List<SiraCagirmaResponseDto>> GetPersonelBekleyenSiralarAsync(string tcKimlikNo);

        /// <summary>
        /// Sıradaki vatandaşı çağırır
        /// </summary>
        /// <param name="siraId">Çağrılacak sıra ID</param>
        /// <param name="personelTcKimlikNo">Çağıran personel TC</param>
        /// <param name="bankoId">Çağıran banko ID (TV bildirimi için)</param>
        /// <param name="bankoNo">Çağıran banko numarası (TV bildirimi için)</param>
        /// <param name="firstCallableSiraId">Concurrency kontrolü için ilk çağrılabilir sıra ID</param>
        Task<SiraCagirmaResponseDto?> SiradakiCagirAsync(int siraId, string personelTcKimlikNo, int? bankoId = null, string? bankoNo = null, int? firstCallableSiraId = null);

        /// <summary>
        /// Sırayı tamamlar
        /// </summary>
        Task<bool> SiraTamamlaAsync(int siraId);

        /// <summary>
        /// Sırayı iptal eder
        /// </summary>
        Task<bool> SiraIptalAsync(int siraId);

        /// <summary>
        /// Banko paneli için personelin uzmanlığına göre bekleyen sıraları getirir
        /// </summary>
        Task<List<SiraCagirmaResponseDto>> GetBankoPanelSiralarAsync(string tcKimlikNo);
    }
}
