using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    public enum BankoTipi
    {
        [Display(Name = "BANKO")]
        Normal = 1,

        [Display(Name = "ÖNCELİKLİ BANKO")]
        Oncelikli = 2,

        [Display(Name = "ENGELLİ BANKO")]
        Engelli = 3,

        [Display(Name = "ŞEF MASASI")]
        SefMasasi = 4
    }
}