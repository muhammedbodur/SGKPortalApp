using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    /// <summary>
    /// Audit log'larının hangi storage stratejisi ile saklanacağını belirtir
    /// </summary>
    public enum StorageStrategy
    {
        [Display(Name = "Her Zaman Veritabanı")]
        AlwaysDatabase = 1,

        [Display(Name = "Her Zaman Dosya")]
        AlwaysFile = 2,

        [Display(Name = "Akıllı Hibrit (1KB altı DB, üstü dosya)")]
        SmartHybrid = 3
    }
}
