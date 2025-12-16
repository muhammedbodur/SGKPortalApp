using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using System.ComponentModel.DataAnnotations;

namespace SGKPortalApp.BusinessObjectLayer.DTOs.Request.Common
{
    public class ModulControllerIslemCreateRequestDto
    {
        [Required(ErrorMessage = "İşlem adı zorunludur")]
        [StringLength(100, ErrorMessage = "İşlem adı en fazla 100 karakter olabilir")]
        public string ModulControllerIslemAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Controller seçimi zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir controller seçiniz")]
        public int ModulControllerId { get; set; }

        public YetkiIslemTipi IslemTipi { get; set; } = YetkiIslemTipi.Page;

        [StringLength(200, ErrorMessage = "Route en fazla 200 karakter olabilir")]
        public string? Route { get; set; }

        [Required(ErrorMessage = "Permission Key zorunludur")]
        [StringLength(200, ErrorMessage = "Permission Key en fazla 200 karakter olabilir")]
        public string PermissionKey { get; set; } = string.Empty;

        public YetkiSeviyesi MinYetkiSeviyesi { get; set; } = YetkiSeviyesi.View;

        public SayfaTipi SayfaTipi { get; set; } = SayfaTipi.Protected;

        public int? UstIslemId { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Aciklama { get; set; }
    }
}
