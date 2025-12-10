using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SignalR
{
    /// <summary>
    /// SignalR Event Tipleri
    /// Gönderilen mesajın amacını belirtir
    /// </summary>
    public enum SignalREventType
    {
        /// <summary>Yeni sıra oluşturuldu (Kiosk'tan)</summary>
        [Display(Name = "Yeni Sıra")]
        NewSira = 1,

        /// <summary>Sıra çağrıldı</summary>
        [Display(Name = "Sıra Çağrıldı")]
        SiraCalled = 2,

        /// <summary>Sıra tamamlandı</summary>
        [Display(Name = "Sıra Tamamlandı")]
        SiraCompleted = 3,

        /// <summary>Sıra iptal edildi</summary>
        [Display(Name = "Sıra İptal")]
        SiraCancelled = 4,

        /// <summary>Sıra yönlendirildi</summary>
        [Display(Name = "Sıra Yönlendirildi")]
        SiraRedirected = 5,

        /// <summary>Panel güncelleme (liste refresh)</summary>
        [Display(Name = "Panel Güncelleme")]
        PanelUpdate = 10,

        /// <summary>TV sıra güncelleme</summary>
        [Display(Name = "TV Güncelleme")]
        TvUpdate = 20,

        /// <summary>Banko modu aktif</summary>
        [Display(Name = "Banko Modu Aktif")]
        BankoModeActivated = 30,

        /// <summary>Banko modu pasif</summary>
        [Display(Name = "Banko Modu Pasif")]
        BankoModeDeactivated = 31,

        /// <summary>Zorla çıkış</summary>
        [Display(Name = "Zorla Çıkış")]
        ForceLogout = 40,

        /// <summary>Duyuru güncelleme</summary>
        [Display(Name = "Duyuru")]
        Announcement = 50,

        /// <summary>Diğer/Bilinmeyen</summary>
        [Display(Name = "Diğer")]
        Other = 99
    }
}
