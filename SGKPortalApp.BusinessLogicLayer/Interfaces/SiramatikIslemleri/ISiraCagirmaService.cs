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
        Task<bool> SiraIptalAsync(int siraId, string iptalNedeni);

        /// <summary>
        /// Banko paneli için personelin uzmanlığına uygun bekleyen sıraları getirir
        /// </summary>
        Task<List<SiraCagirmaResponseDto>> GetBankoPanelSiralarAsync(string tcKimlikNo);

        /// <summary>
        /// ⭐ Personelin ilk çağrılabilir sırasını getirir (sadece tek sıra - performans için)
        /// </summary>
        Task<SiraCagirmaResponseDto?> GetIlkCagrilabilirSiraAsync(string tcKimlikNo);

        /// <summary>
        /// ⭐ INCREMENTAL UPDATE: Belirli bir sıra alındığında/yönlendirildiğinde,
        /// o sırayı görebilecek TÜM personellerin güncel sıra listelerini getirir.
        /// PersonelTc'ye göre gruplandırılmış liste döner.
        /// </summary>
        Task<Dictionary<string, List<SiraCagirmaResponseDto>>> GetBankoPanelSiralarBySiraIdAsync(int siraId);
    }
}
