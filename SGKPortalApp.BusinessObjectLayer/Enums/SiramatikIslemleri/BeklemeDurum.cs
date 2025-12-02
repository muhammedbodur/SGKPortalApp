using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri
{
    public enum BeklemeDurum : int
    {
        [Display(Name = "İşlem Beklemede")]
        Beklemede = 0,

        [Display(Name = "İşlem Çağrıldı")]
        Cagrildi = 1,

        [Display(Name = "İşlem Bitti")]
        Bitti = 2,

        [Display(Name = "İşlem Yönlendirildi")]
        Yonlendirildi = 3
    }
}