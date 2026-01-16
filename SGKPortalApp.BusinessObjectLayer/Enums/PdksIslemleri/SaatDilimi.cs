using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri
{
    /// <summary>
    /// Mazeret için saat dilimi
    /// </summary>
    public enum SaatDilimi
    {
        [Display(Name = "Sabah (08:00-12:00)")]
        Sabah = 1,

        [Display(Name = "Öğle (12:00-13:00)")]
        Ogle = 2,

        [Display(Name = "Akşam (13:00-17:00)")]
        Aksam = 3,

        [Display(Name = "Tam Gün")]
        TamGun = 4
    }
}
