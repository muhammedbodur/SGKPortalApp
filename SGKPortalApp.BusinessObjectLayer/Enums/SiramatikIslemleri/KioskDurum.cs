using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    public enum KioskDurum
    {
        [Display(Name = "Pasif")]
        Pasif = 0,

        [Display(Name = "Aktif")]
        Aktif = 1,

        [Display(Name = "Bakım")]
        Bakim = 2,

        [Display(Name = "Arızalı")]
        Arizali = 3,

        [Display(Name = "Çevrimdışı")]
        Offline = 4
    }
}