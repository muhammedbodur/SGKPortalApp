using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.ZKTeco;
using SGKPortalApp.BusinessObjectLayer.DTOs.Shared.ZKTeco;
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
    public partial class InstantAttendanceRecords : FieldPermissionPageBase
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
        private List<AttendanceLogDto> pagedRecords = new List<AttendanceLogDto>();
        private List<DeviceResponseDto> devices = new List<DeviceResponseDto>();
        private int selectedDeviceId = 0;
        private bool isLoading = false;
        private int totalCount = 0;

        // Pagination
        private int currentPage = 1;
        private int pageSize = 50;
        private int totalPages = 0;

        // Filters
        private DateTime? startDate = null;
        private DateTime? endDate = null;
        private string enrollNumberFilter = "";
        private string sicilNoFilter = "";
        private string tcKimlikNoFilter = "";
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

            if (!string.IsNullOrWhiteSpace(sicilNoFilter))
            {
                filteredRecords = filteredRecords.Where(r => 
                    !string.IsNullOrEmpty(r.PersonelSicilNo) && 
                    r.PersonelSicilNo.Contains(sicilNoFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(tcKimlikNoFilter))
            {
                filteredRecords = filteredRecords.Where(r => 
                    !string.IsNullOrEmpty(r.PersonelTcKimlikNo) && 
                    r.PersonelTcKimlikNo.Contains(tcKimlikNoFilter)).ToList();
            }

            if (verifyMethodFilter.HasValue)
            {
                filteredRecords = filteredRecords.Where(r => r.VerifyMethod == verifyMethodFilter.Value).ToList();
            }

            if (inOutModeFilter.HasValue)
            {
                filteredRecords = filteredRecords.Where(r => r.InOutMode == inOutModeFilter.Value).ToList();
            }

            // Pagination hesapla
            totalPages = (int)Math.Ceiling(filteredRecords.Count / (double)pageSize);
            if (currentPage > totalPages && totalPages > 0)
            {
                currentPage = totalPages;
            }
            if (currentPage < 1)
            {
                currentPage = 1;
            }

            pagedRecords = filteredRecords
                .OrderByDescending(r => r.DateTime)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private void ClearFilters()
        {
            startDate = null;
            endDate = null;
            enrollNumberFilter = "";
            sicilNoFilter = "";
            tcKimlikNoFilter = "";
            verifyMethodFilter = null;
            inOutModeFilter = null;
            currentPage = 1;
            ApplyFilters();
        }

        private void GoToPage(int page)
        {
            if (page >= 1 && page <= totalPages)
            {
                currentPage = page;
                ApplyFilters();
            }
        }

        private void NextPage()
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                ApplyFilters();
            }
        }

        private void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                ApplyFilters();
            }
        }
    }
}
