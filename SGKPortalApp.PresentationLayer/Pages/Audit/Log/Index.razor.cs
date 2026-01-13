using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.AuditLog;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.AuditLog;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PersonelIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Components.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Personel;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Audit.Log
{
    public partial class Index : FieldPermissionPageBase
    {
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        // DEPENDENCY INJECTION
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        [Inject] private IAuditLogApiService _auditLogApiService { get; set; } = default!;
        [Inject] private IDepartmanApiService _departmanApiService { get; set; } = default!;
        [Inject] private IServisApiService _servisApiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        // STATE
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        private AuditLogFilterDto Filter = new()
        {
            StartDate = DateTime.Now.AddDays(-7),
            EndDate = DateTime.Now,
            PageNumber = 1,
            PageSize = 25
        };

        private string SearchText = "";
        private AuditLogPagedResultDto? result;
        private bool isLoading = false;

        // Dropdown data
        private List<DepartmanResponseDto> Departmanlar = new();
        private List<ServisResponseDto> TumServisler = new();
        private List<ServisResponseDto> FiltreliServisler = new();

        // Selected values
        private int selectedDepartmanId = 0;
        private int selectedServisId = 0;

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        // LIFECYCLE
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadDropdownData();

            // KullanÄ±cÄ±nÄ±n kendi departmanÄ±nÄ± default olarak seÃ§
            var userDepartmanId = GetCurrentUserDepartmanId();
            if (userDepartmanId > 0 && Departmanlar.Any(d => d.DepartmanId == userDepartmanId))
            {
                selectedDepartmanId = userDepartmanId;
                Filter.DepartmanId = userDepartmanId;
                await OnDepartmanChanged(selectedDepartmanId);
            }

            await SearchAsync();
        }

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        // DATA LOADING
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        private async Task LoadDropdownData()
        {
            try
            {
                // DepartmanlarÄ± yÃ¼kle
                var deptResult = await _departmanApiService.GetActiveAsync();
                if (deptResult.Success && deptResult.Data != null)
                {
                    Departmanlar = deptResult.Data;
                }

                // TÃ¼m servisleri yÃ¼kle
                var servisResult = await _servisApiService.GetActiveAsync();
                if (servisResult.Success && servisResult.Data != null)
                {
                    TumServisler = servisResult.Data;
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync("Dropdown verileri yÃ¼klenirken hata oluÅŸtu");
            }
        }

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        // FILTER EVENT HANDLERS
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        private async Task OnDepartmanChangedEvent(ChangeEventArgs e)
        {
            // âœ… YETKÄ° KONTROLÃœ: KullanÄ±cÄ± departman filtresini deÄŸiÅŸtirebilir mi?
            if (!CanEditFieldInList("DEPARTMANID"))
            {
                await _toastService.ShowWarningAsync("Bu filtreyi deÄŸiÅŸtirme yetkiniz yok!");
                return;
            }

            if (int.TryParse(e.Value?.ToString(), out int deptId))
            {
                // âœ… ERÄ°ÅÄ°M KONTROLÃœ: Bu departmana eriÅŸim var mÄ±?
                if (deptId > 0 && !CanEditFieldInList("DEPARTMANID") && !CanAccessDepartman(deptId))
                {
                    await _toastService.ShowWarningAsync("Bu departmanÄ± gÃ¶rÃ¼ntÃ¼leme yetkiniz yok!");
                    return;
                }

                selectedDepartmanId = deptId;
                await OnDepartmanChanged(deptId);
            }
        }

        private async Task OnDepartmanChanged(int deptId)
        {
            if (deptId > 0)
            {
                Filter.DepartmanId = deptId;

                // Servisler global - tÃ¼m servisleri gÃ¶ster
                FiltreliServisler = TumServisler.ToList();

                // Servis seÃ§imini sÄ±fÄ±rla
                selectedServisId = 0;
                Filter.ServisId = null;
            }
            else
            {
                Filter.DepartmanId = null;
                FiltreliServisler = TumServisler.ToList();
                selectedServisId = 0;
                Filter.ServisId = null;
            }

            await SearchAsync();
        }

        private async Task OnServisChangedEvent(ChangeEventArgs e)
        {
            // âœ… YETKÄ° KONTROLÃœ: KullanÄ±cÄ± servis filtresini deÄŸiÅŸtirebilir mi?
            if (!CanEditFieldInList("SERVISID"))
            {
                await _toastService.ShowWarningAsync("Bu filtreyi deÄŸiÅŸtirme yetkiniz yok!");
                return;
            }

            if (int.TryParse(e.Value?.ToString(), out int servisId))
            {
                selectedServisId = servisId;
                Filter.ServisId = servisId > 0 ? servisId : null;
                await SearchAsync();
            }
        }

        private async Task OnPageSizeChangedEvent(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int pageSize))
            {
                Filter.PageSize = pageSize;
                Filter.PageNumber = 1; // Sayfa boyutu deÄŸiÅŸtiÄŸinde ilk sayfaya dÃ¶n
                await SearchAsync();
            }
        }

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        // SEARCH & PAGINATION
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        private async Task SearchAsync()
        {
            // SearchText'i Filter.SearchText'e aktar
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                Filter.SearchText = SearchText.Trim();
            }
            else
            {
                Filter.SearchText = null;
            }

            isLoading = true;
            try
            {
                result = await _auditLogApiService.GetLogsAsync(Filter);
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync("Log'lar yÃ¼klenirken hata oluÅŸtu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task GoToPage(int pageNumber)
        {
            if (pageNumber < 1 || pageNumber > (result?.TotalPages ?? 1))
                return;

            Filter.PageNumber = pageNumber;
            await SearchAsync();
        }

        private async Task ResetFilters()
        {
            Filter = new AuditLogFilterDto
            {
                StartDate = DateTime.Now.AddDays(-7),
                EndDate = DateTime.Now,
                PageNumber = 1,
                PageSize = 25
            };
            SearchText = "";
            selectedDepartmanId = 0;
            selectedServisId = 0;
            FiltreliServisler = new();

            await SearchAsync();
        }

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        // HELPER METHODS
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        private List<TransactionGroup> GetTransactionGroups()
        {
            if (result == null || !result.Logs.Any())
                return new List<TransactionGroup>();

            return result.Logs
                .GroupBy(l => l.TransactionId ?? Guid.Empty)
                .Select(g => new TransactionGroup
                {
                    TransactionId = g.Key == Guid.Empty ? null : g.Key,
                    Logs = g.ToList()
                })
                .OrderByDescending(g => g.Logs.Max(l => l.IslemZamani))
                .ToList();
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

        /// <summary>
        /// KullanÄ±cÄ±nÄ±n belirtilen departmanÄ± gÃ¶rÃ¼ntÃ¼leme yetkisi var mÄ±?
        /// </summary>
        private bool CanAccessDepartman(int departmanId)
        {
            var userDepartmanId = GetCurrentUserDepartmanId();
            return userDepartmanId == departmanId;
        }

        /// <summary>
        /// KullanÄ±cÄ±nÄ±n departman ID'sini dÃ¶ndÃ¼rÃ¼r
        /// </summary>
        private int GetCurrentUserDepartmanId()
        {
            var authState = AuthStateProvider.GetAuthenticationStateAsync().Result;
            var claim = authState.User.FindFirst("DepartmanId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
        }

        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•
        // INNER CLASSES
        // â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•

        private class TransactionGroup
        {
            public Guid? TransactionId { get; set; }
            public List<AuditLogDto> Logs { get; set; } = new();
        }
    }
}
