using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    public enum MedeniDurumu
    {
        [Display(Name = "Bekar")]
        bekar,

        [Display(Name = "Evli")]
        evli
    }
}