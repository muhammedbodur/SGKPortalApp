using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.AuditLog;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Log
{
    public partial class Detail
    {
        [Parameter]
        public int LogId { get; set; }

        [Inject]
        private IAuditLogApiService AuditLogApiService { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private AuditLogDetailDto? detail;
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadDetailAsync();
        }

        private async Task LoadDetailAsync()
        {
            isLoading = true;
            try
            {
                detail = await AuditLogApiService.GetLogDetailAsync(LogId);
            }
            finally
            {
                isLoading = false;
            }
        }

        private string GetActionBadgeClass(DatabaseAction action)
        {
            return action switch
            {
                DatabaseAction.INSERT or DatabaseAction.CREATE_COMPLETE => "bg-success",
                DatabaseAction.UPDATE or DatabaseAction.UPDATE_COMPLETE => "bg-warning text-dark",
                DatabaseAction.DELETE => "bg-danger",
                _ => "bg-secondary"
            };
        }
    }
}
