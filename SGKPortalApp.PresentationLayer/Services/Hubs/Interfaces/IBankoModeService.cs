using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.PresentationLayer.Services.Hubs.Interfaces
{
    /// <summary>
    /// Banko modu yönetimi için servis interface
    /// </summary>
    public interface IBankoModeService
    {
        /// <summary>
        /// Personelin atanmış banko bilgisini getir
        /// </summary>
        Task<BankoResponseDto?> GetPersonelAssignedBankoAsync(string tcKimlikNo);

        /// <summary>
        /// Personel banko modunda mı?
        /// </summary>
        Task<bool> IsPersonelInBankoModeAsync(string tcKimlikNo);

        /// <summary>
        /// Banko kullanımda mı?
        /// </summary>
        Task<bool> IsBankoInUseAsync(int bankoId);

        /// <summary>
        /// Bankodaki aktif personel bilgisini getir
        /// </summary>
        Task<string?> GetBankoActivePersonelNameAsync(int bankoId);

        /// <summary>
        /// Banko moduna geç (Tam C# implementasyonu)
        /// </summary>
        /// <param name="currentConnectionId">Aktif tab'ın ConnectionId'si (bu tab kapatılmayacak)</param>
        Task<bool> EnterBankoModeAsync(string tcKimlikNo, int bankoId, string? currentConnectionId = null);

        /// <summary>
        /// Banko modundan çık (Tam C# implementasyonu)
        /// </summary>
        Task<bool> ExitBankoModeAsync(string tcKimlikNo);
    }
}
