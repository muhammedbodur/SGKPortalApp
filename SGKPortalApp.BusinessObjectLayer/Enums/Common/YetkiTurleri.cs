using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    public enum YetkiTurleri
    {
        [Display(Name = "Ana Yetki")]
        AnaYetki,

        [Display(Name = "Orta Yetki")]
        OrtaYetki,

        [Display(Name = "Alt Yetki")]
        AltYetki
    }
}