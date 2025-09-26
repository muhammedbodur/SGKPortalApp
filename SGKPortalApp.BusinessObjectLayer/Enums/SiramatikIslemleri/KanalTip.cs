using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    public enum KanalTip
    {
        [Display(Name = "Genel")]
        Genel = 1,

        [Display(Name = "Özel")]
        Ozel = 2,

        [Display(Name = "Öncelikli")]
        Oncelikli = 3,

        [Display(Name = "Acil")]
        Acil = 4
    }
}