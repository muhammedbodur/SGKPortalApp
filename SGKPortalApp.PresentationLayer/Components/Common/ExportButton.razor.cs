using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;

namespace SGKPortalApp.PresentationLayer.Components.Common
{
    /// <summary>
    /// Export işlemleri için standart buton component
    /// </summary>
    public partial class ExportButton : ComponentBase
    {
        /// <summary>
        /// Export tipi (Excel, PDF, CSV, Word)
        /// </summary>
        [Parameter, EditorRequired]
        public ExportType ExportType { get; set; }

        /// <summary>
        /// Export butonu tıklama event'i
        /// </summary>
        [Parameter, EditorRequired]
        public EventCallback OnClick { get; set; }

        /// <summary>
        /// Herhangi bir export işlemi devam ediyor mu?
        /// </summary>
        [Parameter]
        public bool IsExporting { get; set; }

        /// <summary>
        /// Şu anda hangi export tipi çalışıyor? (lowercase string: "excel", "pdf", vb.)
        /// </summary>
        [Parameter]
        public string CurrentExportType { get; set; } = string.Empty;

        /// <summary>
        /// Özel label (belirtilmezse ExportType'dan otomatik)
        /// </summary>
        [Parameter]
        public string? CustomLabel { get; set; }

        /// <summary>
        /// Özel icon (belirtilmezse ExportType'dan otomatik)
        /// </summary>
        [Parameter]
        public string? CustomIcon { get; set; }

        /// <summary>
        /// Özel CSS class (belirtilmezse ExportType'dan otomatik)
        /// </summary>
        [Parameter]
        public string? CustomCssClass { get; set; }

        private string Label => CustomLabel ?? ExportType.ToString();

        private string Icon => CustomIcon ?? ExportType switch
        {
            ExportType.Excel => "bx-download",
            ExportType.PDF => "bxs-file-pdf",
            ExportType.CSV => "bx-file",
            ExportType.Word => "bxs-file-doc",
            _ => "bx-download"
        };

        private string CssClass => CustomCssClass ?? ExportType switch
        {
            ExportType.Excel => "btn-success",
            ExportType.PDF => "btn-danger",
            ExportType.CSV => "btn-info",
            ExportType.Word => "btn-primary",
            _ => "btn-secondary"
        };

        /// <summary>
        /// Bu butonun export'u şu anda çalışıyor mu?
        /// </summary>
        private bool IsCurrentlyExporting =>
            IsExporting &&
            !string.IsNullOrEmpty(CurrentExportType) &&
            CurrentExportType.Equals(ExportType.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
