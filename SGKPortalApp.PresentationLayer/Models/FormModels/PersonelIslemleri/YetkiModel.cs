using System;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.PersonelIslemleri
{
    /// <summary>
    /// Personel yetki bilgileri form modeli
    /// </summary>
    public class YetkiModel
    {
        public int DepartmanId { get; set; }
        public int ServisId { get; set; }
        public string? GorevDegisimSebebi { get; set; }
        public DateTime? ImzaYetkisiBaslamaTarihi { get; set; }
        public DateTime? ImzaYetkisiBitisTarihi { get; set; }
    }
}
