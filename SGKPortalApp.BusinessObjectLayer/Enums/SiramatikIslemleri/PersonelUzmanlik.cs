using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    /// <summary>
    /// Kanal personelinin uzmanlık seviyesi
    /// </summary>
    public enum PersonelUzmanlik : int
    {
        [Display(Name = "Konusunda Uzman")]
        Uzman = 1,

        [Display(Name = "Bilgisi Yok")]
        BilgisiYok = 0,

        [Display(Name = "Konusunda Yrd. Uzman")]
        YrdUzman = 2
    }
}
