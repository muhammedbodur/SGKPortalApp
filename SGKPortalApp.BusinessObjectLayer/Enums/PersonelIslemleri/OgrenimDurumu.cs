using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri
{
    public enum OgrenimDurumu
    {
        [Display(Name = "İlk Okul")]
        ilkokul,

        [Display(Name = "İlk Öğretim")]
        ilkogretim,

        [Display(Name = "Lise")]
        lise,

        [Display(Name = "Yüksek Okul")]
        yuksekokul,

        [Display(Name = "Lisans")]
        lisans,

        [Display(Name = "Yüksek Lisans")]
        yukseklisans,

        [Display(Name = "Doktora")]
        doktora
    }
}