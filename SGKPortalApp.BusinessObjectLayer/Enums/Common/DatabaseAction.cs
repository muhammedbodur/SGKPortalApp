using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    public enum DatabaseAction
    {
        [Display(Name = "INSERT")]
        INSERT,

        [Display(Name = "UPDATE")]
        UPDATE,

        [Display(Name = "DELETE")]
        DELETE,

        [Display(Name = "NONE")]
        NONE
    }
}