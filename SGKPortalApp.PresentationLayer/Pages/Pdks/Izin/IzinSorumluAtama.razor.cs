using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Izin
{
    public partial class IzinSorumluAtama
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private HttpClient _httpClient { get; set; } = default!;
        [Inject] private IPersonelListApiService _personelListApiService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = default!;
        [Inject] private IJSRuntime _js { get; set; } = default!;
        [Inject] private ILogger<IzinSorumluAtama> _logger { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<IzinSorumluResponseDto> Sorumlular { get; set; } = new();

        // Filter dropdown'lar için (yetkili liste)
        private List<DepartmanDto> DepartmanList { get; set; } = new();
        private List<ServisDto> ServisList { get; set; } = new();

        // Modal dropdown'lar için (tüm liste)
        private List<DepartmanDto> AllDepartmanList { get; set; } = new();
        private List<ServisDto> AllServisList { get; set; } = new();

        private List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri.PersonelListResponseDto> PersonelList { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private int? FilterDepartmanId { get; set; } = null;
        private int? FilterServisId { get; set; } = null;
        private string? FilterAktif { get; set; } = "";
        private int? userDepartmanId = null;
        private int? userServisId = null;

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = false;
        private bool ShowModal { get; set; } = false;
        private bool IsEditMode { get; set; } = false;
        private bool IsSaving { get; set; } = false;
        private string? ErrorMessage { get; set; } = null;

        // ═══════════════════════════════════════════════════════
        // FORM MODEL
        // ═══════════════════════════════════════════════════════

        private IzinSorumluFormModel EditModel { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // LIFECYCLE METHODS
        // ═══════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadFilterData();
            await LoadSorumlular();
        }

        // ═══════════════════════════════════════════════════════
        // DATA LOADING METHODS
        // ═══════════════════════════════════════════════════════

        private async Task LoadFilterData()
        {
            try
            {
                // Get user's departman and servis from claims
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var departmanIdClaim = authState.User.FindFirst("DepartmanId")?.Value;
                var servisIdClaim = authState.User.FindFirst("ServisId")?.Value;

                if (int.TryParse(departmanIdClaim, out var deptId))
                {
                    userDepartmanId = deptId;
                }

                if (int.TryParse(servisIdClaim, out var servId))
                {
                    userServisId = servId;
                }

                // Filtre için yetkili departman listesi
                var deptResult = await _personelListApiService.GetDepartmanListeAsync();
                if (deptResult.Success && deptResult.Data != null)
                {
                    DepartmanList = deptResult.Data;
                }

                // Filtre için yetkili servis listesi
                var servResult = await _personelListApiService.GetServisListeAsync();
                if (servResult.Success && servResult.Data != null)
                {
                    ServisList = servResult.Data;
                }

                // Modal için TÜM departman listesi
                var allDeptResult = await _personelListApiService.GetAllDepartmanListeAsync();
                if (allDeptResult.Success && allDeptResult.Data != null)
                {
                    AllDepartmanList = allDeptResult.Data;
                }

                // Modal için TÜM servis listesi
                var allServResult = await _personelListApiService.GetAllServisListeAsync();
                if (allServResult.Success && allServResult.Data != null)
                {
                    AllServisList = allServResult.Data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Filtre verileri yüklenirken hata");
            }
        }

        private async Task LoadSorumlular()
        {
            try
            {
                IsLoading = true;
                Sorumlular.Clear();

                var url = "/api/izin-sorumlu";
                if (!string.IsNullOrEmpty(FilterAktif))
                {
                    url = FilterAktif == "true" ? "/api/izin-sorumlu/aktif" : "/api/izin-sorumlu";
                }

                var response = await _httpClient.GetFromJsonAsync<ApiResponseDto<List<IzinSorumluResponseDto>>>(url);
                if (response?.Success == true && response.Data != null)
                {
                    Sorumlular = response.Data;

                    if (FilterDepartmanId.HasValue)
                    {
                        Sorumlular = Sorumlular.Where(s => s.DepartmanId == FilterDepartmanId.Value || !s.DepartmanId.HasValue).ToList();
                    }

                    if (FilterServisId.HasValue)
                    {
                        Sorumlular = Sorumlular.Where(s => s.ServisId == FilterServisId.Value || !s.ServisId.HasValue).ToList();
                    }

                    if (!string.IsNullOrEmpty(FilterAktif))
                    {
                        var aktif = FilterAktif == "true";
                        Sorumlular = Sorumlular.Where(s => s.Aktif == aktif).ToList();
                    }
                }
            }
            catch
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Modal için personel listesini departman/servise göre yükler
        /// </summary>
        private async Task LoadPersonelForModal()
        {
            try
            {
                PersonelList.Clear();

                var perRequest = new
                {
                    DepartmanId = EditModel.DepartmanId,
                    ServisId = EditModel.ServisId,
                    SadeceAktifler = true
                };

                var perResult = await _personelListApiService.GetPersonelListeAsync(perRequest);
                if (perResult.Success && perResult.Data != null)
                {
                    PersonelList = perResult.Data.Select(p => new SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri.PersonelListResponseDto
                    {
                        TcKimlikNo = p.TcKimlikNo,
                        AdSoyad = p.AdSoyad,
                        SicilNo = p.SicilNo
                    }).ToList();
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Modal personel listesi yüklenirken hata");
            }
        }

        // ═══════════════════════════════════════════════════════
        // EVENT HANDLERS
        // ═══════════════════════════════════════════════════════

        private async Task OpenCreateModal()
        {
            IsEditMode = false;
            EditModel = new IzinSorumluFormModel { OnaySeviyes = 1, Aktif = true };
            ErrorMessage = null;
            PersonelList.Clear();
            ShowModal = true;
            await Task.CompletedTask;
        }

        private async Task OpenEditModal(IzinSorumluResponseDto item)
        {
            IsEditMode = true;
            EditModel = new IzinSorumluFormModel
            {
                IzinSorumluId = item.IzinSorumluId,
                DepartmanId = item.DepartmanId,
                ServisId = item.ServisId,
                SorumluPersonelTcKimlikNo = item.SorumluPersonelTcKimlikNo,
                OnaySeviyes = item.OnaySeviyes,
                Aktif = item.Aktif,
                Aciklama = item.Aciklama
            };
            ErrorMessage = null;
            ShowModal = true;

            // Modal açıldığında mevcut departman/servise göre personel listesini yükle
            await LoadPersonelForModal();
        }

        private void CloseModal()
        {
            ShowModal = false;
            EditModel = new();
            ErrorMessage = null;
        }

        private async Task SaveSorumlu()
        {
            try
            {
                IsSaving = true;
                ErrorMessage = null;

                if (string.IsNullOrEmpty(EditModel.SorumluPersonelTcKimlikNo))
                {
                    ErrorMessage = "Sorumlu personel seçilmelidir";
                    return;
                }

                if (IsEditMode)
                {
                    // Güncelleme
                    var updateDto = new IzinSorumluUpdateDto
                    {
                        IzinSorumluId = EditModel.IzinSorumluId,
                        DepartmanId = EditModel.DepartmanId,
                        ServisId = EditModel.ServisId,
                        SorumluPersonelTcKimlikNo = EditModel.SorumluPersonelTcKimlikNo,
                        OnaySeviyes = EditModel.OnaySeviyes,
                        Aktif = EditModel.Aktif,
                        Aciklama = EditModel.Aciklama
                    };

                    var response = await _httpClient.PutAsJsonAsync($"/api/izin-sorumlu/{EditModel.IzinSorumluId}", updateDto);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinSorumluResponseDto>>();
                        if (result?.Success == true)
                        {
                            CloseModal();
                            await LoadSorumlular();
                        }
                        else
                        {
                            ErrorMessage = result?.Message ?? "Güncelleme başarısız";
                        }
                    }
                    else
                    {
                        ErrorMessage = "Güncelleme sırasında hata oluştu";
                    }
                }
                else
                {
                    // Yeni oluşturma
                    var createDto = new IzinSorumluCreateDto
                    {
                        DepartmanId = EditModel.DepartmanId,
                        ServisId = EditModel.ServisId,
                        SorumluPersonelTcKimlikNo = EditModel.SorumluPersonelTcKimlikNo,
                        OnaySeviyes = EditModel.OnaySeviyes,
                        Aciklama = EditModel.Aciklama
                    };

                    var response = await _httpClient.PostAsJsonAsync("/api/izin-sorumlu", createDto);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<IzinSorumluResponseDto>>();
                        if (result?.Success == true)
                        {
                            CloseModal();
                            await LoadSorumlular();
                        }
                        else
                        {
                            ErrorMessage = result?.Message ?? "Kayıt başarısız";
                        }
                    }
                    else
                    {
                        ErrorMessage = "Kayıt sırasında hata oluştu";
                    }
                }
            }
            catch
            {
                ErrorMessage = "İşlem sırasında hata oluştu";
            }
            finally
            {
                IsSaving = false;
            }
        }

        /// <summary>
        /// Modal'da departman değiştiğinde personel listesini güncelle
        /// </summary>
        private async Task OnModalDepartmanChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int deptId) && deptId > 0)
            {
                EditModel.DepartmanId = deptId;
            }
            else
            {
                EditModel.DepartmanId = null;
            }

            // Departman değiştiğinde servisi sıfırla ve personel listesini yeniden yükle
            EditModel.ServisId = null;
            EditModel.SorumluPersonelTcKimlikNo = null;
            await LoadPersonelForModal();
        }

        /// <summary>
        /// Modal'da servis değiştiğinde personel listesini güncelle
        /// </summary>
        private async Task OnModalServisChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int servId) && servId > 0)
            {
                EditModel.ServisId = servId;
            }
            else
            {
                EditModel.ServisId = null;
            }

            // Servis değiştiğinde personel seçimini sıfırla ve personel listesini yeniden yükle
            EditModel.SorumluPersonelTcKimlikNo = null;
            await LoadPersonelForModal();
        }

        private async Task DeactivateSorumlu(int id)
        {
            var confirmed = await _js.InvokeAsync<bool>("confirm", "Bu sorumlu atamasını pasif yapmak istediğinizden emin misiniz?");
            if (!confirmed)
                return;

            try
            {
                var response = await _httpClient.PatchAsync($"/api/izin-sorumlu/{id}/pasif", null);
                if (response.IsSuccessStatusCode)
                {
                    await LoadSorumlular();
                }
            }
            catch
            {
            }
        }

        private async Task DeleteSorumlu(int id)
        {
            var confirmed = await _js.InvokeAsync<bool>("confirm", "Bu sorumlu atamasını silmek istediğinizden emin misiniz?");
            if (!confirmed)
                return;

            try
            {
                var response = await _httpClient.DeleteAsync($"/api/izin-sorumlu/{id}");
                if (response.IsSuccessStatusCode)
                {
                    await LoadSorumlular();
                }
            }
            catch
            {
            }
        }
    }
}
