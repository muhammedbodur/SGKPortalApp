using System;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.PersonelIslemleri
{
    /// <summary>
    /// Personel ceza bilgileri form modeli
    /// </summary>
    public class CezaModel
    {
        public string? CezaSebebi { get; set; }
        public string? AltBendi { get; set; }
        public DateTime? CezaTarihi { get; set; }
    }
}
