using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SignalR
{
    /// <summary>
    /// SignalR Hedef Tipleri
    /// Mesajın kime gönderildiğini belirtir
    /// </summary>
    public enum SignalRTargetType
    {
        /// <summary>Tek bir connection'a</summary>
        [Display(Name = "Connection")]
        Connection = 1,

        /// <summary>Birden fazla connection'a</summary>
        [Display(Name = "Connections")]
        Connections = 2,

        /// <summary>Bir gruba (TV_1, BANKO_1 gibi)</summary>
        [Display(Name = "Group")]
        Group = 3,

        /// <summary>Tüm bağlantılara (broadcast)</summary>
        [Display(Name = "All")]
        All = 4,

        /// <summary>Personel TC'ye (dolaylı olarak connection'larına)</summary>
        [Display(Name = "Personel")]
        Personel = 5,

        /// <summary>TV'ye (dolaylı olarak connection'larına)</summary>
        [Display(Name = "TV")]
        Tv = 6
    }
}
