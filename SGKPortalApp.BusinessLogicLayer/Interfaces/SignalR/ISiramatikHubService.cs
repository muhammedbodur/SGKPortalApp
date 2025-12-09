using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SignalR;

namespace SGKPortalApp.BusinessLogicLayer.Interfaces.SignalR
{
    /// <summary>
    /// Sıramatik SignalR yayın servisi interface
    /// Business katmanından hub'a mesaj göndermek için kullanılır
    ///
    /// NOT: Bu interface Business katmanında, implementasyonu ise
    /// ISignalRBroadcaster aracılığıyla Presentation/API katmanından alır.
    /// ⭐ 100% Request/Command Pattern
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
        /// Sıra tamamlandığında ilgili banko panellerine bildirim gönder
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastSiraCompletedAsync(BroadcastSiraCompletedRequest request);

        /// <summary>
        /// Sıra iptal edildiğinde ilgili banko panellerine bildirim gönder
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastSiraCancelledAsync(BroadcastSiraCancelledRequest request);

        /// <summary>
        /// Sıra yönlendirildiğinde kaynak ve hedef banko panellerine bildirim gönder
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastSiraRedirectedAsync(BroadcastSiraRedirectedRequest request);

        /// <summary>
        /// ⭐ INCREMENTAL UPDATE: Sıra alındığında/yönlendirildiğinde etkilenen personellere
        /// güncel sıra listelerini gönderir. Her personele ConnectionId ile direkt mesaj gönderilir.
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastBankoPanelGuncellemesiAsync(BroadcastBankoPanelGuncellemesiRequest request);

        // ═══════════════════════════════════════════════════════
        // KIOSK / YENİ SIRA BİLDİRİMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Yeni sıra alındığında (Kiosk'tan) ilgili banko panellerine bildirim gönder
        /// Masaüstü Kiosk, Web Kiosk veya herhangi bir client'tan sıra alındığında çağrılır
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastNewSiraAsync(BroadcastNewSiraRequest request);

        // ═══════════════════════════════════════════════════════
        // TV EKRANI BİLDİRİMLERİ
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// Sıra çağrıldığında TV ekranlarına bildirim gönder
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastSiraToTvAsync(BroadcastSiraToTvRequest request);

        /// <summary>
        /// Sıra çağırıldığında TV'lere bildirim gönderir
        /// ⭐ Request/Command Pattern
        /// </summary>
        Task BroadcastSiraCalledToTvAsync(BroadcastSiraCalledToTvRequest request);
    }
}
