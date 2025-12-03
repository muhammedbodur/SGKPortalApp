using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SiramatikIslemleri
{
    /// <summary>
    /// Sıra Çağırma Servisi - Business Layer
    /// </summary>
    public interface ISiraCagirmaService
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
        Task<SiraCagirmaResponseDto?> SiradakiCagirAsync(int siraId, string personelTcKimlikNo, int? firstCallableSiraId = null);

        /// <summary>
        /// Sırayı tamamlar
        /// </summary>
        Task<bool> SiraTamamlaAsync(int siraId);

        /// <summary>
        /// Sırayı iptal eder
        /// </summary>
        Task<bool> SiraIptalAsync(int siraId, string iptalNedeni);

        /// <summary>
        /// Banko paneli için personelin uzmanlığına uygun bekleyen sıraları getirir
        /// </summary>
        Task<List<SiraCagirmaResponseDto>> GetBankoPanelSiralarAsync(string tcKimlikNo);
    }
}
