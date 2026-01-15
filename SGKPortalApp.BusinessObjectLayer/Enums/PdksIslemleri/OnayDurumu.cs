using System.ComponentModel;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri
{
    /// <summary>
    /// İzin/Mazeret talepleri için onay durumu
    /// Çift onay sisteminde kullanılır (1. ve 2. onayci)
    /// </summary>
    public enum OnayDurumu
    {
        /// <summary>
        /// Onay bekliyor (varsayılan durum)
        /// </summary>
        [Description("Beklemede")]
        Beklemede = 0,

        /// <summary>
        /// Onaylandı (talep kabul edildi)
        /// </summary>
        [Description("Onaylandı")]
        Onaylandi = 1,

        /// <summary>
        /// Reddedildi (talep kabul edilmedi)
        /// </summary>
        [Description("Reddedildi")]
        Reddedildi = 2,

        /// <summary>
        /// İptal edildi (talep sahibi iptal etti)
        /// </summary>
        [Description("İptal Edildi")]
        IptalEdildi = 3
    }
}
