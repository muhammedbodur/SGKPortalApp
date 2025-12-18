using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.Common.Extensions;

namespace SGKPortalApp.PresentationLayer.Components.Permission
{
    /// <summary>
    /// ActionType enum ile çalışan yetki kontrollü buton component
    /// </summary>
    public partial class ActionButton : ComponentBase
    {
        /// <summary>
        /// Action tipi (DETAIL, EDIT, DELETE, vb.)
        /// </summary>
        [Parameter, EditorRequired]
        public ActionType ActionType { get; set; }

        /// <summary>
        /// Buton tıklama event'i
        /// </summary>
        [Parameter, EditorRequired]
        public EventCallback OnClick { get; set; }

        /// <summary>
        /// Buton görünür mü? (Parent'tan IsActionVisible sonucu)
        /// </summary>
        [Parameter]
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Buton enabled mı? (Parent'tan CanAction/CanEditAction sonucu)
        /// </summary>
        [Parameter]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Icon class (varsayılan: ActionType'a göre otomatik)
        /// </summary>
        [Parameter]
        public string? Icon { get; set; }

        /// <summary>
        /// CSS class (varsayılan: ActionType'a göre otomatik)
        /// </summary>
        [Parameter]
        public string? CssClass { get; set; }

        /// <summary>
        /// Title (varsayılan: ActionType'ın display name'i)
        /// </summary>
        [Parameter]
        public string? Title { get; set; }

        /// <summary>
        /// Click event'i propagate etmesin mi? (varsayılan: true)
        /// </summary>
        [Parameter]
        public bool StopPropagation { get; set; } = true;

        protected override void OnParametersSet()
        {
            // Icon belirtilmemişse ActionType'a göre otomatik ata
            if (string.IsNullOrEmpty(Icon))
            {
                Icon = ActionType switch
                {
                    ActionType.DETAIL => "bx-show",
                    ActionType.EDIT => "bx-edit",
                    ActionType.DELETE => "bx-trash",
                    ActionType.ADD => "bx-plus",
                    ActionType.TOGGLE => "bx-toggle-right",
                    ActionType.EXPORT => "bx-download",
                    ActionType.IMPORT => "bx-upload",
                    ActionType.PRINT => "bx-printer",
                    ActionType.APPROVE => "bx-check-circle",
                    ActionType.REJECT => "bx-x-circle",
                    ActionType.ARCHIVE => "bx-archive",
                    ActionType.BULK_UPDATE => "bx-edit-alt",
                    _ => "bx-info-circle"
                };
            }

            // CssClass belirtilmemişse ActionType'a göre otomatik ata
            if (string.IsNullOrEmpty(CssClass))
            {
                CssClass = ActionType switch
                {
                    ActionType.DETAIL => "btn-outline-primary",
                    ActionType.EDIT => "btn-outline-info",
                    ActionType.DELETE => "btn-outline-danger",
                    ActionType.ADD => "btn-outline-success",
                    ActionType.APPROVE => "btn-outline-success",
                    ActionType.REJECT => "btn-outline-danger",
                    _ => "btn-outline-secondary"
                };
            }

            // Title belirtilmemişse ActionType'ın display name'ini kullan ve yetki durumunu ekle
            if (string.IsNullOrEmpty(Title))
            {
                var actionName = ActionType.GetDisplayName();
                Title = IsEnabled ? actionName : $"{actionName} (Yetki Yok)";
            }
        }

        private async Task HandleClick()
        {
            if (IsEnabled && OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync();
            }
        }
    }
}
