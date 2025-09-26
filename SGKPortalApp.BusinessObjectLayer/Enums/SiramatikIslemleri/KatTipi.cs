using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    public enum KatTipi : int
    {
        [Display(Name = "Zemin")]
        zemin = 0,

        [Display(Name = "1.Kat")]
        bir = 1,

        [Display(Name = "2.Kat")]
        iki = 2,

        [Display(Name = "3.Kat")]
        uc = 3,

        [Display(Name = "4.Kat")]
        dort = 4,

        [Display(Name = "5.Kat")]
        bes = 5,

        [Display(Name = "6.Kat")]
        alti = 6
    }
}