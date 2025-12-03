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
        Task<SiraCagirmaResponseDto?> SiradakiCagirAsync(int siraId, string personelTcKimlikNo, int? firstCallableSiraId);

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
