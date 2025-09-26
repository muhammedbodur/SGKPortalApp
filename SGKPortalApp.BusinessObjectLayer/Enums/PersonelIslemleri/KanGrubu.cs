using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    public enum KanGrubu
    {
        [Display(Name = "0 Rh(+)")]
        sifir_arti,

        [Display(Name = "0 Rh(-)")]
        sifir_eksi,

        [Display(Name = "AB Rh(+)")]
        ab_arti,

        [Display(Name = "AB Rh(-)")]
        ab_eksi,

        [Display(Name = "A Rh(+)")]
        a_arti,

        [Display(Name = "A Rh(-)")]
        a_eksi,

        [Display(Name = "B Rh(+)")]
        b_arti,

        [Display(Name = "B Rh(-)")]
        b_eksi
    }
}