using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    public enum PersonelDurum : int
    {
        [Display(Name = "Pasif Personel")]
        Pasif = 0,

        [Display(Name = "Aktif Personel")]
        Aktif = 1,

        [Display(Name = "Emekli Personel")]
        Emekli = 2,

        [Display(Name = "İstifa Etti")]
        IstifaEtti = 3,

        [Display(Name = "Vefat")]
        Vefat = 4
    }
}