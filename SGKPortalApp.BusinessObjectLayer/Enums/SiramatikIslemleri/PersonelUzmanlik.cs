using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    /// <summary>
    /// Kanal personelinin uzmanlık seviyesi
    /// </summary>
    public enum PersonelUzmanlik : int
    {
        //Konusunda Yetkin Değil
        [Display(Name = "Bilgisi Yok")]
        BilgisiYok = 0,

        //Yrd. Uzman Konusunda Bilgisi Var ama tam yetkin değil
        [Display(Name = "Yrd. Uzman")]
        YrdUzman = 1,

        //Uzman Yetkisi Konusunda Uzman olan kişi
        [Display(Name = "Uzman")]
        Uzman = 2,

        //Şef Yetkisi Konusunda En Uzman olan ve yönlendirme yapılabilecek en yetkili kişi
        [Display(Name = "Şef")]
        Sef = 3
    }
}
