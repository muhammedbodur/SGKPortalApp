using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    public enum YetkiTipleri : int
    {
        [Display(Name = "Görme")]
        None = 0,

        [Display(Name = "Gör")]
        View = 1,

        [Display(Name = "Düzenle")]
        Edit = 2
    }
}