namespace SGKPortalApp.PresentationLayer.Services.Hubs.Constants
{
    /// <summary>
    /// SignalR event adları - camelCase formatında (JavaScript convention)
    /// Hub ve JavaScript tarafında aynı adlar kullanılmalı
    /// </summary>
    public static class SignalREvents
    {
        // ═══════════════════════════════════════════════════════
        // BANKO MODE EVENTS
        // ═══════════════════════════════════════════════════════
        
        /// <summary>Banko moduna girildiğinde tetiklenir</summary>
        public const string BankoModeActivated = "bankoModeActivated";
        
        /// <summary>Banko modundan çıkıldığında tetiklenir</summary>
        public const string BankoModeDeactivated = "bankoModeDeactivated";
        
        /// <summary>Banko modu hatası oluştuğunda tetiklenir</summary>
        public const string BankoModeError = "bankoModeError";

        // ═══════════════════════════════════════════════════════
        // CONNECTION EVENTS
        // ═══════════════════════════════════════════════════════
        
        /// <summary>Kullanıcı zorla çıkış yapması gerektiğinde tetiklenir</summary>
        public const string ForceLogout = "forceLogout";
        
        /// <summary>Bağlantı durumu değiştiğinde tetiklenir</summary>
        public const string ConnectionStatusChanged = "connectionStatusChanged";

        // ═══════════════════════════════════════════════════════
        // SIRA (QUEUE) EVENTS
        // ═══════════════════════════════════════════════════════
        
        /// <summary>Yeni sıra oluşturulduğunda tetiklenir</summary>
        public const string SiraCreated = "siraCreated";
        
        /// <summary>Sıra çağrıldığında tetiklenir</summary>
        public const string SiraCalled = "siraCalled";
        
        /// <summary>Sıra güncellendiğinde tetiklenir</summary>
        public const string SiraUpdated = "siraUpdated";
        
        /// <summary>Sıra tamamlandığında tetiklenir</summary>
        public const string SiraCompleted = "siraCompleted";
        
        /// <summary>Sıra iptal edildiğinde tetiklenir</summary>
        public const string SiraCancelled = "siraCancelled";
        
        /// <summary>Sıra yönlendirildiğinde tetiklenir</summary>
        public const string SiraRedirected = "siraRedirected";

        // ═══════════════════════════════════════════════════════
        // TV DISPLAY EVENTS
        // ═══════════════════════════════════════════════════════
        
        /// <summary>TV ekranına sıra güncellemesi gönderildiğinde tetiklenir</summary>
        public const string ReceiveSiraUpdate = "receiveSiraUpdate";
        
        /// <summary>TV ekranına duyuru güncellemesi gönderildiğinde tetiklenir</summary>
        public const string ReceiveDuyuruUpdate = "receiveDuyuruUpdate";

        // ═══════════════════════════════════════════════════════
        // NOTIFICATION EVENTS
        // ═══════════════════════════════════════════════════════
        
        /// <summary>Bildirim gönderildiğinde tetiklenir</summary>
        public const string NotificationReceived = "notificationReceived";
        
        /// <summary>Uyarı mesajı gönderildiğinde tetiklenir</summary>
        public const string AlertReceived = "alertReceived";
    }
}
