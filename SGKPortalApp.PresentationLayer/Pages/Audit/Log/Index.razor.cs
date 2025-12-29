using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.AuditLog;
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
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IAuditLogApiService _auditLogApiService { get; set; } = default!;
        [Inject] private IDepartmanApiService _departmanApiService { get; set; } = default!;
        [Inject] private IServisApiService _servisApiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // STATE
        // ═══════════════════════════════════════════════════════

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

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadDropdownData();

            // Kullanıcının kendi departmanını default olarak seç
            var userDepartmanId = GetCurrentUserDepartmanId();
            if (userDepartmanId > 0 && Departmanlar.Any(d => d.DepartmanId == userDepartmanId))
            {
                selectedDepartmanId = userDepartmanId;
                Filter.DepartmanId = userDepartmanId;
                await OnDepartmanChanged(selectedDepartmanId);
            }

            await SearchAsync();
        }

        // ═══════════════════════════════════════════════════════
        // DATA LOADING
        // ═══════════════════════════════════════════════════════

        private async Task LoadDropdownData()
        {
            try
            {
                // Departmanları yükle
                var deptResult = await _departmanApiService.GetActiveAsync();
                if (deptResult.Success && deptResult.Data != null)
                {
                    Departmanlar = deptResult.Data;
                }

                // Tüm servisleri yükle
                var servisResult = await _servisApiService.GetActiveAsync();
                if (servisResult.Success && servisResult.Data != null)
                {
                    TumServisler = servisResult.Data;
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync("Dropdown verileri yüklenirken hata oluştu");
            }
        }

        // ═══════════════════════════════════════════════════════
        // FILTER EVENT HANDLERS
        // ═══════════════════════════════════════════════════════

        private async Task OnDepartmanChangedEvent(ChangeEventArgs e)
        {
            // ✅ YETKİ KONTROLÜ: Kullanıcı departman filtresini değiştirebilir mi?
            if (!CanEditFieldInList("DEPARTMANID"))
            {
                await _toastService.ShowWarningAsync("Bu filtreyi değiştirme yetkiniz yok!");
                return;
            }

            if (int.TryParse(e.Value?.ToString(), out int deptId))
            {
                // ✅ ERİŞİM KONTROLÜ: Bu departmana erişim var mı?
                if (deptId > 0 && !CanEditFieldInList("DEPARTMANID") && !CanAccessDepartman(deptId))
                {
                    await _toastService.ShowWarningAsync("Bu departmanı görüntüleme yetkiniz yok!");
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

                // Servisler global - tüm servisleri göster
                FiltreliServisler = TumServisler.ToList();

                // Servis seçimini sıfırla
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
            // ✅ YETKİ KONTROLÜ: Kullanıcı servis filtresini değiştirebilir mi?
            if (!CanEditFieldInList("SERVISID"))
            {
                await _toastService.ShowWarningAsync("Bu filtreyi değiştirme yetkiniz yok!");
                return;
            }

            if (int.TryParse(e.Value?.ToString(), out int servisId))
            {
                selectedServisId = servisId;
                Filter.ServisId = servisId > 0 ? servisId : null;
                await SearchAsync();
            }
        }

        // ═══════════════════════════════════════════════════════
        // SEARCH & PAGINATION
        // ═══════════════════════════════════════════════════════

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
                await _toastService.ShowErrorAsync("Log'lar yüklenirken hata oluştu");
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

        // ═══════════════════════════════════════════════════════
        // HELPER METHODS
        // ═══════════════════════════════════════════════════════

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
        /// Kullanıcının belirtilen departmanı görüntüleme yetkisi var mı?
        /// </summary>
        private bool CanAccessDepartman(int departmanId)
        {
            var userDepartmanId = GetCurrentUserDepartmanId();
            return userDepartmanId == departmanId;
        }

        /// <summary>
        /// Kullanıcının departman ID'sini döndürür
        /// </summary>
        private int GetCurrentUserDepartmanId()
        {
            var authState = AuthStateProvider.GetAuthenticationStateAsync().Result;
            var claim = authState.User.FindFirst("DepartmanId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
        }

        // ═══════════════════════════════════════════════════════
        // INNER CLASSES
        // ═══════════════════════════════════════════════════════

        private class TransactionGroup
        {
            public Guid? TransactionId { get; set; }
            public List<AuditLogDto> Logs { get; set; } = new();
        }
    }
}
