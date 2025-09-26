using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    public enum PersonelTipi
    {
        [Display(Name = "Memur")]
        memur,

        [Display(Name = "İşçi")]
        isci,

        [Display(Name = "Taşeron")]
        taseron
    }
}