using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri
{
    public class KanalAltIslemFormModel
    {
        public int KanalAltId { get; set; }
        public int KanalIslemId { get; set; }
        public int HizmetBinasiId { get; set; }
        public int Sira { get; set; }
        public Aktiflik Aktiflik { get; set; }
    }
}
