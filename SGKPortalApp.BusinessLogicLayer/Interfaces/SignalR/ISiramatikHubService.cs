using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR
{
    /// <summary>
    /// Sıramatik SignalR yayın servisi interface
    /// Business katmanından hub'a mesaj göndermek için kullanılır
    /// 
    /// NOT: Bu interface Business katmanında, implementasyonu ise
    /// ISignalRBroadcaster aracılığıyla Presentation/API katmanından alır.
    /// </summary>
    public interface ISiramatikHubService
    {
        // ═══════════════════════════════════════════════════════
        // SIRA ÇAĞIRMA PANELİ BİLDİRİMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Sıra çağrıldığında ilgili banko panellerine bildirim gönder
        /// </summary>
        Task BroadcastSiraCalledAsync(
            SiraCagirmaResponseDto sira,
            int callerBankoId,
            string bankoNo,
            string callerPersonelTc);

        /// <summary>
        /// Sıra tamamlandığında ilgili banko panellerine bildirim gönder
        /// </summary>
        Task BroadcastSiraCompletedAsync(int siraId, int hizmetBinasiId, int kanalAltIslemId);

        /// <summary>
        /// Sıra iptal edildiğinde ilgili banko panellerine bildirim gönder
        /// </summary>
        Task BroadcastSiraCancelledAsync(int siraId, int hizmetBinasiId, int kanalAltIslemId);

        /// <summary>
        /// Sıra yönlendirildiğinde kaynak ve hedef banko panellerine bildirim gönder
        /// </summary>
        Task BroadcastSiraRedirectedAsync(
            SiraCagirmaResponseDto sira,
            int sourceBankoId,
            int targetBankoId,
            string sourcePersonelTc);

        /// <summary>
        /// ⭐ INCREMENTAL UPDATE: Sıra alındığında/yönlendirildiğinde etkilenen personellere
        /// güncel sıra listelerini gönderir. Her personele ConnectionId ile direkt mesaj gönderilir.
        /// </summary>
        Task BroadcastBankoPanelGuncellemesiAsync(int siraId);

        // ═══════════════════════════════════════════════════════
        // KIOSK / YENİ SIRA BİLDİRİMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Yeni sıra alındığında (Kiosk'tan) ilgili banko panellerine bildirim gönder
        /// Masaüstü Kiosk, Web Kiosk veya herhangi bir client'tan sıra alındığında çağrılır
        /// </summary>
        Task BroadcastNewSiraAsync(
            SiraCagirmaResponseDto sira,
            int hizmetBinasiId,
            int kanalAltIslemId);

        // ═══════════════════════════════════════════════════════
        // TV EKRANI BİLDİRİMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Sıra çağrıldığında TV ekranlarına bildirim gönder
        /// </summary>
        Task BroadcastSiraToTvAsync(
            SiraCagirmaResponseDto sira,
            string bankoNo,
            int hizmetBinasiId);

        /// <summary>
        /// Sıra çağırıldığında TV'lere bildirim gönderir
        /// </summary>
        Task BroadcastSiraCalledToTvAsync(SiraCagirmaResponseDto sira, int bankoId, string bankoNo);
    }
}
