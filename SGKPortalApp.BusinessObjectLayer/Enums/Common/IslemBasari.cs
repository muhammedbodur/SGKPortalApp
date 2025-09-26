using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    public enum IslemBasari : int
    {
        [Display(Name = "İşlem Başarısız")]
        Basarisiz = 0,

        [Display(Name = "İşlem Başarılı")]
        Basarili = 1
    }
}