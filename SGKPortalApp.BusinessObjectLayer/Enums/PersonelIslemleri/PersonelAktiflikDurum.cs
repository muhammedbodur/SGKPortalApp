using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    public enum PersonelAktiflikDurum : int
    {
        [Display(Name = "Pasif Personel")]
        Pasif = 0,

        [Display(Name = "Aktif Personel")]
        Aktif = 1,

        [Display(Name = "Emekli Personel")]
        Emekli = 2
    }
}