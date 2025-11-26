using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    /// <summary>
    /// Kanal personelinin uzmanlık seviyesi
    /// </summary>
    public enum PersonelUzmanlik : int
    {
        [Display(Name = "Uzman")]
        Uzman = 1,

        [Display(Name = "Bilgisi Yok")]
        BilgisiYok = 0,

        [Display(Name = "Yrd. Uzman")]
        YrdUzman = 2,

        [Display(Name = "Şef")]
        Sef = 3
    }
}
