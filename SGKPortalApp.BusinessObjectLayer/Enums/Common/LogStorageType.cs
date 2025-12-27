using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    /// <summary>
    /// Log verilerinin nerede saklandığını belirtir
    /// </summary>
    public enum LogStorageType
    {
        [Display(Name = "Veritabanı")]
        Database = 1,

        [Display(Name = "Dosya")]
        File = 2,

        [Display(Name = "Özet (Summary)")]
        Summary = 3
    }
}
