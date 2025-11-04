using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    public enum BankoTipi
    {
        [Display(Name = "Normal")]
        Normal = 1,

        [Display(Name = "Öncelikli")]
        Oncelikli = 2,

        [Display(Name = "Engelli")]
        Engelli = 3,

        [Display(Name = "Şef Masası")]
        SefMasasi = 4
    }
}