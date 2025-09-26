using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    public enum SehitYakinligi
    {
        [Display(Name = "Yok")]
        yok,

        [Display(Name = "Babası")]
        babasi,

        [Display(Name = "Annesi")]
        annesi,

        [Display(Name = "Kardeşi")]
        kardesi,

        [Display(Name = "Çocuğu")]
        cocugu,

        [Display(Name = "Şehit Yakını Eşi")]
        sehit_yakini_esi,

        [Display(Name = "Gazi Yakını")]
        gazi_yakini
    }
}