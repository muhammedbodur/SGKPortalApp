using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri
{
    public class KioskMenuIslemFormModel
    {
        public int KioskMenuId { get; set; }
        public int KanalAltId { get; set; }
        public Aktiflik Aktiflik { get; set; }
        public int MenuSira { get; set; } = 1;
    }
}
