using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    public enum Aktiflik : int
    {
        [Display(Name = "Pasif")]
        Pasif = 0,

        [Display(Name = "Aktif")]
        Aktif = 1
    }
}