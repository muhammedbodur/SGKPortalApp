using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    public enum BankoTipi
    {
        [Display(Name = "Banko")]
        banko = 1,

        [Display(Name = "Masa")]
        masa = 2
    }
}