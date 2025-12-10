using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SignalR
{
    /// <summary>
    /// SignalR Mesaj Gönderim Durumu
    /// </summary>
    public enum SignalRDeliveryStatus
    {
        /// <summary>Gönderilmeyi bekliyor</summary>
        [Display(Name = "Beklemede")]
        Pending = 0,

        /// <summary>Başarıyla gönderildi</summary>
        [Display(Name = "Gönderildi")]
        Sent = 1,

        /// <summary>Gönderim başarısız</summary>
        [Display(Name = "Başarısız")]
        Failed = 2,

        /// <summary>Hedef bulunamadı (connection yok)</summary>
        [Display(Name = "Hedef Yok")]
        NoTarget = 3,

        /// <summary>Client tarafından alındı (ACK - Faz 2 için)</summary>
        [Display(Name = "Alındı")]
        Acknowledged = 10
    }
}
