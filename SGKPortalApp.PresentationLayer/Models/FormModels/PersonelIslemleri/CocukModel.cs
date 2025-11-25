using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;
using System;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.PersonelIslemleri
{
    /// <summary>
    /// Personel çocuk bilgileri form modeli
    /// </summary>
    public class CocukModel
    {
        public string Isim { get; set; } = string.Empty;
        public DateTime? DogumTarihi { get; set; }
        public OgrenimDurumu OgrenimDurumu { get; set; } = OgrenimDurumu.ilkokul;
    }
}
