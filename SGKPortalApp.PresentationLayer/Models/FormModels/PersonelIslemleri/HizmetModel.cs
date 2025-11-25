using System;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.PersonelIslemleri
{
    /// <summary>
    /// Personel hizmet geçmişi form modeli
    /// </summary>
    public class HizmetModel
    {
        public int DepartmanId { get; set; }
        public string? Departman { get; set; }
        public int ServisId { get; set; }
        public string? Servis { get; set; }
        public DateTime? BaslamaTarihi { get; set; }
        public DateTime? AyrilmaTarihi { get; set; }
        public string? Sebep { get; set; }
    }
}
