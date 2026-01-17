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
    public partial class PersonelMesaiList
    {
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private ILogger<PersonelMesaiList> Logger { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;

        private List<PersonelListResponseDto> personelList = new();
        private List<DepartmanDto> departmanList = new();

        private bool isLoading = false;
        private int? filterDepartmanId = null;
        private int? userDepartmanId = null;
        private bool sadeceAktifler = true;
        private string? aramaMetni = null;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadFilterData();
            await LoadPersonelList();
        }

        private async Task LoadFilterData()
        {
            try
            {
                // Get user's departman from claims
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var departmanIdClaim = authState.User.FindFirst("DepartmanId")?.Value;
                if (int.TryParse(departmanIdClaim, out var deptId))
                {
                    userDepartmanId = deptId;
                    filterDepartmanId = deptId; // Set user's departman as default filter
                }

                // Load departman list - only user's departman or all if user has permission
                var deptResponse = await HttpClient.GetFromJsonAsync<ApiResponseDto<List<DepartmanDto>>>("/api/departman/yetkili-liste");
                if (deptResponse?.Success == true && deptResponse.Data != null)
                {
                    departmanList = deptResponse.Data;

                    // If user has specific departman but it's not in the list, add it
                    if (userDepartmanId.HasValue && !departmanList.Any(d => d.DepartmanId == userDepartmanId.Value))
                    {
                        // Fallback: if yetkili-liste doesn't include user's departman, use standard liste
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
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Filtre verileri yüklenirken hata");
            }
        }

        private async Task LoadPersonelList()
        {
            try
            {
                isLoading = true;
                personelList.Clear();

                var request = new PersonelListFilterRequestDto
                {
                    DepartmanId = filterDepartmanId,
                    SadeceAktifler = sadeceAktifler,
                    AramaMetni = aramaMetni
                };

                var response = await HttpClient.PostAsJsonAsync("/api/personel-list/liste", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<PersonelListResponseDto>>>();

                    if (result?.Success == true && result.Data != null)
                    {
                        personelList = result.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Personel listesi yüklenirken hata");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task ToggleAktifDurum(PersonelListResponseDto personel)
        {
            try
            {
                var request = new PersonelAktifDurumUpdateDto
                {
                    TcKimlikNo = personel.TcKimlikNo,
                    Aktif = !personel.Aktif
                };

                var response = await HttpClient.PutAsJsonAsync("/api/personel-list/toggle-aktif", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();

                    if (result?.Success == true)
                    {
                        // Reload list to reflect changes
                        await LoadPersonelList();
                    }
                    else
                    {
                        Logger.LogWarning("Aktif durum güncelleme başarısız: {Message}", result?.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Aktif durum güncellenirken hata: {TcKimlikNo}", personel.TcKimlikNo);
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
                await LoadPersonelList();
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
        /// Kullanıcının departman ID'sini döndürür
        /// </summary>
        private int GetCurrentUserDepartmanId()
        {
            return userDepartmanId ?? 0;
        }

    }
}
