using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    public enum YonlendirmeTipi
    {
        [Display(Name = "Başka Bankoya")]
        BaskaBanko = 1,

        [Display(Name = "Şef/Yetkili Masasına")]
        Sef = 2,

        [Display(Name = "Uzman Personele")]
        UzmanPersonel = 3
    }
}
