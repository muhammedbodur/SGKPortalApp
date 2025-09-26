using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    public enum PersonelUzmanlik : int
    {
        [Display(Name = "Bilgisi Yok")]
        BilgisiYok = 0,

        [Display(Name = "Konusunda Uzman")]
        Uzman = 1,

        [Display(Name = "Konusunda Yrd. Uzman")]
        YrdUzman = 2
    }
}