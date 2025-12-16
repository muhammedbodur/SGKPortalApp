using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common
{
    public class ModulControllerIslemResponseDto
    {
        public int ModulControllerIslemId { get; set; }
        public string ModulControllerIslemAdi { get; set; } = string.Empty;
        public YetkiIslemTipi IslemTipi { get; set; }
        public string? Route { get; set; }
        public string PermissionKey { get; set; } = string.Empty;
        public YetkiSeviyesi MinYetkiSeviyesi { get; set; }
        public SayfaTipi SayfaTipi { get; set; }
        public int? UstIslemId { get; set; }
        public string? UstIslemAdi { get; set; }
        public string? Aciklama { get; set; }
        public int ModulControllerId { get; set; }
        public string ModulControllerAdi { get; set; } = string.Empty;
        public int ModulId { get; set; }
        public string ModulAdi { get; set; } = string.Empty;
        public string? DtoTypeName { get; set; }
        public string? DtoFieldName { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime DuzenlenmeTarihi { get; set; }
    }
}
