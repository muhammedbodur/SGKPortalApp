using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    public enum EngelDerecesi
    {
        [Display(Name = "1. Derece")]
        Birinci = 1,

        [Display(Name = "2. Derece")]
        Ikinci = 2,

        [Display(Name = "3. Derece")]
        Ucuncu = 3
    }
}
