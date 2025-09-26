using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    public enum BankoStatus
    {
        [Display(Name = "Boşta")]
        Bosta = 0,

        [Display(Name = "Meşgul")]
        Mesgul = 1,

        [Display(Name = "Bakım")]
        Bakim = 2,

        [Display(Name = "Arızalı")]
        Arizali = 3
    }
}