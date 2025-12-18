using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.Common
{
    /// <summary>
    /// Dışa aktarma format tipleri
    /// </summary>
    public enum ExportType
    {
        [Display(Name = "Excel")]
        Excel = 1,

        [Display(Name = "PDF")]
        PDF = 2,

        [Display(Name = "CSV")]
        CSV = 3,

        [Display(Name = "Word")]
        Word = 4
    }
}
