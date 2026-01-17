using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Mesai
{
    public partial class DepartmanMesaiList
    {
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private ILogger<DepartmanMesaiList> Logger { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        private DepartmanMesaiReportDto? report;
        private List<DepartmanDto> departmanList = new();
        private List<ServisDto> servisList = new();
        private HashSet<string> expandedRows = new();

        private bool isLoading = false;
        private int? filterDepartmanId = null;
        private int? filterServisId = null;
        private int? userDepartmanId = null;
        private int? userServisId = null;
        private DateTime baslangicTarihi = DateTime.Now.AddDays(-30);
        private DateTime bitisTarihi = DateTime.Now;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadFilterData();
        }

        private async Task LoadFilterData()
        {
            try
            {
                // Get user's departman and servis from claims
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var departmanIdClaim = authState.User.FindFirst("DepartmanId")?.Value;
                var servisIdClaim = authState.User.FindFirst("ServisId")?.Value;

                if (int.TryParse(departmanIdClaim, out var deptId))
                {
                    userDepartmanId = deptId;
                    filterDepartmanId = deptId; // Set user's departman as default filter
                }

                if (int.TryParse(servisIdClaim, out var servId))
                {
                    userServisId = servId;
                    filterServisId = servId; // Set user's servis as default filter
                }

                // Load departman list - only user's departman or all if user has permission
                var departmanResponse = await HttpClient.GetFromJsonAsync<ApiResponseDto<List<DepartmanDto>>>("/api/departman/yetkili-liste");
                if (departmanResponse?.Success == true && departmanResponse.Data != null)
                {
                    departmanList = departmanResponse.Data;

                    // If user has specific departman but it's not in the list, add it
                    if (userDepartmanId.HasValue && !departmanList.Any(d => d.DepartmanId == userDepartmanId.Value))
                    {
                        var fallbackResponse = await HttpClient.GetFromJsonAsync<ApiResponseDto<List<DepartmanDto>>>("/api/departman/liste");
                        if (fallbackResponse?.Success == true && fallbackResponse.Data != null)
                        {
                            var userDept = fallbackResponse.Data.FirstOrDefault(d => d.DepartmanId == userDepartmanId.Value);
                            if (userDept != null)
                            {
                                departmanList = new List<DepartmanDto> { userDept };
                            }
                        }
                    }
                }

                // Load servis list - only user's servis or all if user has permission
                var servisResponse = await HttpClient.GetFromJsonAsync<ApiResponseDto<List<ServisDto>>>("/api/servis/yetkili-liste");
                if (servisResponse?.Success == true && servisResponse.Data != null)
                {
                    servisList = servisResponse.Data;

                    // If user has specific servis but it's not in the list, add it
                    if (userServisId.HasValue && !servisList.Any(s => s.ServisId == userServisId.Value))
                    {
                        var fallbackResponse = await HttpClient.GetFromJsonAsync<ApiResponseDto<List<ServisDto>>>("/api/servis/liste");
                        if (fallbackResponse?.Success == true && fallbackResponse.Data != null)
                        {
                            var userServ = fallbackResponse.Data.FirstOrDefault(s => s.ServisId == userServisId.Value);
                            if (userServ != null)
                            {
                                servisList = new List<ServisDto> { userServ };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Filtre verileri yüklenirken hata");
            }
        }

        private async Task LoadReport()
        {
            try
            {
                if (!filterDepartmanId.HasValue)
                {
                    Logger.LogWarning("Departman seçilmeden rapor oluşturulamaz");
                    return;
                }

                isLoading = true;
                report = null;
                expandedRows.Clear();

                var request = new DepartmanMesaiFilterRequestDto
                {
                    DepartmanId = filterDepartmanId.Value,
                    ServisId = filterServisId,
                    BaslangicTarihi = baslangicTarihi,
                    BitisTarihi = bitisTarihi
                };

                var response = await HttpClient.PostAsJsonAsync("/api/departman-mesai/rapor", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<DepartmanMesaiReportDto>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        report = result.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Departman mesai raporu yüklenirken hata");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ToggleRow(string tcKimlikNo)
        {
            if (expandedRows.Contains(tcKimlikNo))
            {
                expandedRows.Remove(tcKimlikNo);
            }
            else
            {
                expandedRows.Add(tcKimlikNo);
            }
        }

        /// <summary>
        /// Departman değişiklik event handler - Authorization kontrolü ile
        /// </summary>
        private async Task OnDepartmanChangedEvent(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int deptId))
            {
                // ✅ ERİŞİM KONTROLÜ: Kullanıcı bu departmanı görüntüleyebilir mi?
                if (deptId > 0 && !CanAccessDepartman(deptId))
                {
                    await ToastService.ShowWarningAsync("Bu departmanı görüntüleme yetkiniz yok!");
                    // Kullanıcının kendi departmanına geri dön
                    filterDepartmanId = userDepartmanId;
                    return;
                }

                filterDepartmanId = deptId > 0 ? deptId : null;
                // Departman değiştiğinde servis listesini temizle
                filterServisId = null;
            }
        }

        /// <summary>
        /// Servis değişiklik event handler - Authorization kontrolü ile
        /// </summary>
        private async Task OnServisChangedEvent(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int servId))
            {
                // ✅ ERİŞİM KONTROLÜ: Kullanıcı bu servisi görüntüleyebilir mi?
                if (servId > 0 && !CanAccessServis(servId))
                {
                    await ToastService.ShowWarningAsync("Bu servisi görüntüleme yetkiniz yok!");
                    // Kullanıcının kendi servisine geri dön
                    filterServisId = userServisId;
                    return;
                }

                filterServisId = servId > 0 ? servId : null;
            }
        }

        /// <summary>
        /// Kullanıcının belirtilen departmanı görüntüleme yetkisi var mı?
        /// </summary>
        private bool CanAccessDepartman(int departmanId)
        {
            var userDeptId = GetCurrentUserDepartmanId();

            // Kullanıcının departmanı yoksa (admin vs) tüm departmanlara erişebilir
            if (userDeptId == 0)
                return true;

            // Kullanıcı sadece kendi departmanını görebilir
            return userDeptId == departmanId;
        }

        /// <summary>
        /// Kullanıcının belirtilen servisi görüntüleme yetkisi var mı?
        /// </summary>
        private bool CanAccessServis(int servisId)
        {
            var userServId = GetCurrentUserServisId();

            // Kullanıcının servisi yoksa departman bazlı kontrol yap
            if (userServId == 0)
            {
                // Kullanıcının departmanındaki tüm servislere erişebilir
                return true;
            }

            // Kullanıcı sadece kendi servisini görebilir
            return userServId == servisId;
        }

        /// <summary>
        /// Kullanıcının departman ID'sini döndürür
        /// </summary>
        private int GetCurrentUserDepartmanId()
        {
            return userDepartmanId ?? 0;
        }

        /// <summary>
        /// Kullanıcının servis ID'sini döndürür
        /// </summary>
        private int GetCurrentUserServisId()
        {
            return userServisId ?? 0;
        }

    }
}
