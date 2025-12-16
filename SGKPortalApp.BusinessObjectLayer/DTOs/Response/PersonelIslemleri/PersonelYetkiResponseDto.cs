using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri
{
    public class PersonelYetkiResponseDto
    {
        public int PersonelYetkiId { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;

        // ModulControllerIslem bilgisi (Yetki tanımı)
        public int ModulControllerIslemId { get; set; }
        public string ModulControllerIslemAdi { get; set; } = string.Empty;
        public string PermissionKey { get; set; } = string.Empty;
        public YetkiIslemTipi IslemTipi { get; set; }

        // Yetki seviyesi
        public YetkiSeviyesi YetkiSeviyesi { get; set; }

        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}
