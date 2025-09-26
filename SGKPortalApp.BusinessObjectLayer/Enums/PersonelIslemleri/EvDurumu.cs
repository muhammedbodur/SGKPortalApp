using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    public enum EvDurumu
    {
        [Display(Name = "Ev Sahibi")]
        ev_sahibi,

        [Display(Name = "Kiracı")]
        kiraci,

        [Display(Name = "Diğer")]
        diger
    }
}