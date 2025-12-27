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

        [Display(Name = "CREATE_COMPLETE")]
        CREATE_COMPLETE,

        [Display(Name = "UPDATE_COMPLETE")]
        UPDATE_COMPLETE,

        [Display(Name = "BULK_INSERT")]
        BULK_INSERT,

        [Display(Name = "BULK_UPDATE")]
        BULK_UPDATE,

        [Display(Name = "BULK_DELETE")]
        BULK_DELETE,

        [Display(Name = "NONE")]
        NONE
    }
}