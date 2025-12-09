using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR;
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
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastSiraCalledAsync(BroadcastSiraCalledRequest request);

        /// <summary>
        /// Sıra çağrıldığında ilgili banko panellerine bildirim gönder
        /// </summary>
        [Obsolete("Use BroadcastSiraCalledAsync(BroadcastSiraCalledRequest) instead. This method will be removed in future versions.")]
        Task BroadcastSiraCalledAsync(
            SiraCagirmaResponseDto sira,
            int callerBankoId,
            string bankoNo,
            string callerPersonelTc);

        /// <summary>
        /// Sıra tamamlandığında ilgili banko panellerine bildirim gönder
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastSiraCompletedAsync(BroadcastSiraCompletedRequest request);

        /// <summary>
        /// Sıra tamamlandığında ilgili banko panellerine bildirim gönder
        /// </summary>
        [Obsolete("Use BroadcastSiraCompletedAsync(BroadcastSiraCompletedRequest) instead.")]
        Task BroadcastSiraCompletedAsync(int siraId, int hizmetBinasiId, int kanalAltIslemId);

        /// <summary>
        /// Sıra iptal edildiğinde ilgili banko panellerine bildirim gönder
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastSiraCancelledAsync(BroadcastSiraCancelledRequest request);

        /// <summary>
        /// Sıra iptal edildiğinde ilgili banko panellerine bildirim gönder
        /// </summary>
        [Obsolete("Use BroadcastSiraCancelledAsync(BroadcastSiraCancelledRequest) instead.")]
        Task BroadcastSiraCancelledAsync(int siraId, int hizmetBinasiId, int kanalAltIslemId);

        /// <summary>
        /// Sıra yönlendirildiğinde kaynak ve hedef banko panellerine bildirim gönder
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastSiraRedirectedAsync(BroadcastSiraRedirectedRequest request);

        /// <summary>
        /// Sıra yönlendirildiğinde kaynak ve hedef banko panellerine bildirim gönder
        /// </summary>
        [Obsolete("Use BroadcastSiraRedirectedAsync(BroadcastSiraRedirectedRequest) instead.")]
        Task BroadcastSiraRedirectedAsync(
            SiraCagirmaResponseDto sira,
            int sourceBankoId,
            int? targetBankoId,
            string sourcePersonelTc);

        /// <summary>
        /// ⭐ INCREMENTAL UPDATE: Sıra alındığında/yönlendirildiğinde etkilenen personellere
        /// güncel sıra listelerini gönderir. Her personele ConnectionId ile direkt mesaj gönderilir.
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastBankoPanelGuncellemesiAsync(BroadcastBankoPanelGuncellemesiRequest request);

        /// <summary>
        /// ⭐ INCREMENTAL UPDATE: Sıra alındığında/yönlendirildiğinde etkilenen personellere
        /// güncel sıra listelerini gönderir. Her personele ConnectionId ile direkt mesaj gönderilir.
        /// </summary>
        [Obsolete("Use BroadcastBankoPanelGuncellemesiAsync(BroadcastBankoPanelGuncellemesiRequest) instead.")]
        Task BroadcastBankoPanelGuncellemesiAsync(int siraId);

        // ═══════════════════════════════════════════════════════
        // KIOSK / YENİ SIRA BİLDİRİMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Yeni sıra alındığında (Kiosk'tan) ilgili banko panellerine bildirim gönder
        /// Masaüstü Kiosk, Web Kiosk veya herhangi bir client'tan sıra alındığında çağrılır
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastNewSiraAsync(BroadcastNewSiraRequest request);

        /// <summary>
        /// Yeni sıra alındığında (Kiosk'tan) ilgili banko panellerine bildirim gönder
        /// Masaüstü Kiosk, Web Kiosk veya herhangi bir client'tan sıra alındığında çağrılır
        /// </summary>
        [Obsolete("Use BroadcastNewSiraAsync(BroadcastNewSiraRequest) instead.")]
        Task BroadcastNewSiraAsync(
            SiraCagirmaResponseDto sira,
            int hizmetBinasiId,
            int kanalAltIslemId);

        // ═══════════════════════════════════════════════════════
        // TV EKRANI BİLDİRİMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Sıra çağrıldığında TV ekranlarına bildirim gönder
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastSiraToTvAsync(BroadcastSiraToTvRequest request);

        /// <summary>
        /// Sıra çağrıldığında TV ekranlarına bildirim gönder
        /// </summary>
        [Obsolete("Use BroadcastSiraToTvAsync(BroadcastSiraToTvRequest) instead.")]
        Task BroadcastSiraToTvAsync(
            SiraCagirmaResponseDto sira,
            string bankoNo,
            int hizmetBinasiId);

        /// <summary>
        /// Sıra çağırıldığında TV'lere bildirim gönderir
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastSiraCalledToTvAsync(BroadcastSiraCalledToTvRequest request);

        /// <summary>
        /// Sıra çağırıldığında TV'lere bildirim gönderir
        /// </summary>
        [Obsolete("Use BroadcastSiraCalledToTvAsync(BroadcastSiraCalledToTvRequest) instead.")]
        Task BroadcastSiraCalledToTvAsync(SiraCagirmaResponseDto sira, int bankoId, string bankoNo);
    }
}
