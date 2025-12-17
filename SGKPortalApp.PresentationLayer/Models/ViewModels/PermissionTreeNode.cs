using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.PresentationLayer.Models.ViewModels
{
    /// <summary>
    /// Yetki ağacı görünümü için kullanılan model.
    /// Modül > Controller > İşlem hiyerarşisini temsil eder.
    /// </summary>
    public class PermissionTreeNode
    {
        public int ModulControllerIslemId { get; set; }
        public string IslemAdi { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string PermissionKey { get; set; } = string.Empty;
        public YetkiIslemTipi IslemTipi { get; set; }
        public bool IsGroupNode { get; set; }

        public int PersonelYetkiId { get; set; } = 0;
        public YetkiSeviyesi SelectedLevel { get; set; } = YetkiSeviyesi.None;
        public YetkiSeviyesi? OriginalLevel { get; set; } = null; // null = atanmamış
        public YetkiSeviyesi MinYetkiSeviyesi { get; set; } = YetkiSeviyesi.None; // Varsayılan seviye
        public bool IsAssigned { get; set; } = false; // Personele yetki atanmış mı?
        public bool HasChanges { get; set; } = false;

        public List<PermissionTreeNode> Children { get; set; } = new();
    }
}
