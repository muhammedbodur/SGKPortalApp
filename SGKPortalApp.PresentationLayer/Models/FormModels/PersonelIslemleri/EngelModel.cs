using SGKPortalApp.BusinessObjectLayer.Enums.PersonelIslemleri;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.PersonelIslemleri
{
    /// <summary>
    /// Personel engel bilgileri form modeli
    /// </summary>
    public class EngelModel
    {
        public EngelDerecesi EngelDerecesi { get; set; }
        public string? EngelNedeni1 { get; set; }
        public string? EngelNedeni2 { get; set; }
        public string? EngelNedeni3 { get; set; }
    }
}
