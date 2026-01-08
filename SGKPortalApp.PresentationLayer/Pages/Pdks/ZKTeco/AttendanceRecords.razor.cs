using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.Enums.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Components.Base;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.ZKTeco;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.ZKTeco
{
    public partial class AttendanceRecords : FieldPermissionPageBase
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IZKTecoDeviceApiService DeviceApiService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<AttendanceLogDto> records = new List<AttendanceLogDto>();
        private List<AttendanceLogDto> filteredRecords = new List<AttendanceLogDto>();
        private List<DeviceResponseDto> devices = new List<DeviceResponseDto>();
        private int selectedDeviceId = 0;
        private bool isLoading = false;
        private int totalCount = 0;

        // Filters
        private DateTime? startDate = null;
        private DateTime? endDate = null;
        private string enrollNumberFilter = "";
        private VerifyMethod? verifyMethodFilter = null;
        private InOutMode? inOutModeFilter = null;

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadDevices();
        }

        // ═══════════════════════════════════════════════════════
        // DATA LOADING
        // ═══════════════════════════════════════════════════════

        private async Task LoadDevices()
        {
            try
            {
                var result = await DeviceApiService.GetActiveAsync();
                if (result.Success && result.Data != null)
                {
                    devices = result.Data;
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Cihazlar yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private async Task LoadRecords()
        {
            if (selectedDeviceId == 0)
            {
                await ToastService.ShowWarningAsync("Lütfen bir cihaz seçin");
                return;
            }

            isLoading = true;
            try
            {
                var result = await DeviceApiService.GetAttendanceLogsAsync(selectedDeviceId);
                if (result.Success && result.Data != null)
                {
                    records = result.Data;
                    ApplyFilters();
                    
                    var countResult = await DeviceApiService.GetAttendanceLogCountAsync(selectedDeviceId);
                    if (countResult.Success)
                    {
                        totalCount = countResult.Data;
                    }
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Kayıtlar yüklenemedi");
                    records = new List<AttendanceLogDto>();
                    filteredRecords = new List<AttendanceLogDto>();
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
                records = new List<AttendanceLogDto>();
                filteredRecords = new List<AttendanceLogDto>();
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task OnDeviceChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int deviceId))
            {
                selectedDeviceId = deviceId;
                await LoadRecords();
            }
            else
            {
                selectedDeviceId = 0;
                records.Clear();
                filteredRecords.Clear();
            }
        }

        // ═══════════════════════════════════════════════════════
        // FILTERING
        // ═══════════════════════════════════════════════════════

        private void ApplyFilters()
        {
            filteredRecords = records;

            if (startDate.HasValue)
            {
                filteredRecords = filteredRecords.Where(r => r.DateTime >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                filteredRecords = filteredRecords.Where(r => r.DateTime <= endDate.Value.AddDays(1).AddSeconds(-1)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(enrollNumberFilter))
            {
                filteredRecords = filteredRecords.Where(r => r.EnrollNumber.Contains(enrollNumberFilter)).ToList();
            }

            if (verifyMethodFilter.HasValue)
            {
                filteredRecords = filteredRecords.Where(r => r.VerifyMethod == verifyMethodFilter.Value).ToList();
            }

            if (inOutModeFilter.HasValue)
            {
                filteredRecords = filteredRecords.Where(r => r.InOutMode == inOutModeFilter.Value).ToList();
            }
        }

        private void ClearFilters()
        {
            startDate = null;
            endDate = null;
            enrollNumberFilter = "";
            verifyMethodFilter = null;
            inOutModeFilter = null;
            ApplyFilters();
        }

        // ═══════════════════════════════════════════════════════
        // OPERATIONS
        // ═══════════════════════════════════════════════════════

        private async Task ClearAllLogs()
        {
            if (selectedDeviceId == 0)
            {
                await ToastService.ShowWarningAsync("Lütfen bir cihaz seçin");
                return;
            }

            if (!await JS.InvokeAsync<bool>("confirm", "⚠️ DİKKAT! Cihazdaki TÜM attendance kayıtlarını silmek istediğinize emin misiniz?\n\nBu işlem geri alınamaz!"))
            {
                return;
            }

            try
            {
                var result = await DeviceApiService.ClearAttendanceLogsAsync(selectedDeviceId);
                if (result.Success && result.Data)
                {
                    await ToastService.ShowSuccessAsync("Tüm kayıtlar silindi");
                    await LoadRecords();
                }
                else
                {
                    await ToastService.ShowErrorAsync(result.Message ?? "Kayıtlar silinemedi");
                }
            }
            catch (Exception ex)
            {
                await ToastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }
    }
}
