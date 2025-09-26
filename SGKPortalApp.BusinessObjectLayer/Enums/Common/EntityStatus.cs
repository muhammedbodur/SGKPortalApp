using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    public enum EntityStatus
    {
        [Display(Name = "Aktif")]
        Active = 1,

        [Display(Name = "Pasif")]
        Inactive = 0,

        [Display(Name = "Silindi")]
        Deleted = -1
    }
}