using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SGKPortalApp.BusinessObjectLayer.DTOs.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.PdksIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Models.FormModels.PdksIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Pdks;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Pdks.Izin
{
    public partial class IzinSorumluAtama
    {
        // ═══════════════════════════════════════════════════════
        // DEPENDENCY INJECTION
        // ═══════════════════════════════════════════════════════

        [Inject] private IIzinSorumluApiService _izinSorumluApiService { get; set; } = default!;
        [Inject] private IPersonelListApiService _personelListApiService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider _authStateProvider { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private IJSRuntime _js { get; set; } = default!;
        [Inject] private ILogger<IzinSorumluAtama> _logger { get; set; } = default!;

        // ═══════════════════════════════════════════════════════
        // DATA PROPERTIES
        // ═══════════════════════════════════════════════════════

        private List<IzinSorumluResponseDto> AllSorumlular { get; set; } = new();
        private List<IzinSorumluResponseDto> Sorumlular { get; set; } = new();

        // Dropdown'lar için (tüm liste)
        private List<DepartmanDto> DepartmanList { get; set; } = new();
        private List<ServisDto> ServisList { get; set; } = new();

        private List<SGKPortalApp.BusinessObjectLayer.DTOs.Response.PdksIslemleri.PersonelListResponseDto> PersonelList { get; set; } = new();

        // ═══════════════════════════════════════════════════════
        // FILTER PROPERTIES
        // ═══════════════════════════════════════════════════════

        private string SearchTerm { get; set; } = "";
        private int? FilterDepartmanId { get; set; } = null;
        private int? FilterServisId { get; set; } = null;
        private string FilterAktiflik { get; set; } = "";
        private string SortBy { get; set; } = "name-asc";
        private int? userDepartmanId = null;
        private int? userServisId = null;

        // ═══════════════════════════════════════════════════════
        // UI STATE
        // ═══════════════════════════════════════════════════════

        private bool IsLoading { get; set; } = false;
        private bool ShowModal { get; set; } = false;
        private bool IsEditMode { get; set; } = false;
        private bool IsSaving { get; set; } = false;
        private bool IsExporting { get; set; } = false;
        private string ExportType { get; set; } = "";
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
                    FilterDepartmanId = deptId; // Set user's departman as default filter
                }

                if (int.TryParse(servisIdClaim, out var servId))
                {
                    userServisId = servId;
                    FilterServisId = servId; // Set user's servis as default filter
                }

                // TÜM departman listesi (filtre ve modal için)
                var deptResult = await _personelListApiService.GetAllDepartmanListeAsync();
                if (deptResult.Success && deptResult.Data != null)
                {
                    DepartmanList = deptResult.Data;
                }

                // TÜM servis listesi (filtre ve modal için)
                var servResult = await _personelListApiService.GetAllServisListeAsync();
                if (servResult.Success && servResult.Data != null)
                {
                    ServisList = servResult.Data;
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
                AllSorumlular.Clear();
                Sorumlular.Clear();

                var result = await _izinSorumluApiService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    AllSorumlular = result.Data;
                    ApplyFilters();
                }
                else
                {
                    _logger.LogWarning("İzin sorumlu listesi alınamadı: {Message}", result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoadSorumlular exception");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilters()
        {
            Sorumlular = AllSorumlular;

            // Arama filtresi
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                var searchLower = SearchTerm.ToLower();
                Sorumlular = Sorumlular.Where(s => 
                    s.SorumluPersonelAdSoyad.ToLower().Contains(searchLower) ||
                    s.SorumluPersonelSicilNo.ToString().Contains(searchLower) ||
                    s.DepartmanAdi.ToLower().Contains(searchLower) ||
                    (s.ServisAdi != null && s.ServisAdi.ToLower().Contains(searchLower))
                ).ToList();
            }

            // Departman filtresi
            if (FilterDepartmanId.HasValue)
            {
                Sorumlular = Sorumlular.Where(s => s.DepartmanId == FilterDepartmanId.Value || !s.DepartmanId.HasValue).ToList();
            }

            // Servis filtresi
            if (FilterServisId.HasValue)
            {
                Sorumlular = Sorumlular.Where(s => s.ServisId == FilterServisId.Value || !s.ServisId.HasValue).ToList();
            }

            // Aktiflik filtresi
            if (!string.IsNullOrEmpty(FilterAktiflik))
            {
                var aktif = FilterAktiflik == "1"; // Aktiflik.Aktif = 1
                Sorumlular = Sorumlular.Where(s => s.Aktif == aktif).ToList();
            }

            // Sıralama
            Sorumlular = SortBy switch
            {
                "name-asc" => Sorumlular.OrderBy(s => s.SorumluPersonelAdSoyad).ToList(),
                "name-desc" => Sorumlular.OrderByDescending(s => s.SorumluPersonelAdSoyad).ToList(),
                "date-newest" => Sorumlular.OrderByDescending(s => s.EklenmeTarihi).ToList(),
                "date-oldest" => Sorumlular.OrderBy(s => s.EklenmeTarihi).ToList(),
                _ => Sorumlular.OrderBy(s => s.SorumluPersonelAdSoyad).ToList()
            };
        }

        private void OnSearchChanged()
        {
            ApplyFilters();
        }

        private void OnSortByChanged(ChangeEventArgs e)
        {
            SortBy = e.Value?.ToString() ?? "name-asc";
            ApplyFilters();
        }

        private async Task OnDepartmanChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value?.ToString()))
            {
                FilterDepartmanId = null;
            }
            else if (int.TryParse(e.Value.ToString(), out int deptId))
            {
                // Yetki kontrolü: Kullanıcı sadece kendi departmanını görebilir
                if (!CanAccessDepartman(deptId))
                {
                    await _toastService.ShowWarningAsync("Bu departmanı görüntüleme yetkiniz yok!");
                    _logger.LogWarning("Yetkisiz departman erişim denemesi: {DeptId}", deptId);
                    FilterDepartmanId = userDepartmanId;
                    return;
                }
                
                FilterDepartmanId = deptId;
            }
            ApplyFilters();
        }

        private async Task OnServisChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value?.ToString()))
            {
                FilterServisId = null;
            }
            else if (int.TryParse(e.Value.ToString(), out int servId))
            {
                // Yetki kontrolü: Kullanıcı sadece kendi servisini görebilir
                if (!CanAccessServis(servId))
                {
                    await _toastService.ShowWarningAsync("Bu servisi görüntüleme yetkiniz yok!");
                    _logger.LogWarning("Yetkisiz servis erişim denemesi: {ServId}", servId);
                    FilterServisId = userServisId;
                    return;
                }
                
                FilterServisId = servId;
            }
            ApplyFilters();
        }

        private void OnAktiflikChanged(ChangeEventArgs e)
        {
            FilterAktiflik = e.Value?.ToString() ?? "";
            ApplyFilters();
        }

        private void ClearFilters()
        {
            SearchTerm = "";
            FilterDepartmanId = userDepartmanId; // Reset to user's departman
            FilterServisId = userServisId; // Reset to user's servis
            FilterAktiflik = "";
            SortBy = "name-asc";
            ApplyFilters();
        }

        private bool CanAccessDepartman(int departmanId)
        {
            var userDeptId = GetCurrentUserDepartmanId();

            // Kullanıcının departmanı yoksa (admin vs) tüm departmanlara erişebilir
            if (userDeptId == 0)
                return true;

            return userDeptId == departmanId;
        }

        private bool CanAccessServis(int servisId)
        {
            var userServId = GetCurrentUserServisId();

            // Kullanıcının servisi yoksa tüm servislere erişebilir
            if (userServId == 0)
                return true;

            return userServId == servisId;
        }

        private int GetCurrentUserDepartmanId()
        {
            return userDepartmanId ?? 0;
        }

        private int GetCurrentUserServisId()
        {
            return userServisId ?? 0;
        }

        private async Task ExportToExcel()
        {
            try
            {
                IsExporting = true;
                ExportType = "excel";
                StateHasChanged();

                // TODO: Excel export implementasyonu
                await Task.Delay(1000); // Simülasyon
                await _js.InvokeVoidAsync("alert", "Excel export özelliği yakında eklenecek.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excel export hatası");
            }
            finally
            {
                IsExporting = false;
                ExportType = "";
                StateHasChanged();
            }
        }

        private async Task ExportToPdf()
        {
            try
            {
                IsExporting = true;
                ExportType = "pdf";
                StateHasChanged();

                // TODO: PDF export implementasyonu
                await Task.Delay(1000); // Simülasyon
                await _js.InvokeVoidAsync("alert", "PDF export özelliği yakında eklenecek.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF export hatası");
            }
            finally
            {
                IsExporting = false;
                ExportType = "";
                StateHasChanged();
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
            EditModel = new IzinSorumluFormModel 
            { 
                OnaySeviyesi = 1, 
                Aktif = true,
                DepartmanId = userDepartmanId, // Set user's departman as default
                ServisId = userServisId // Set user's servis as default
            };
            ErrorMessage = null;
            PersonelList.Clear();
            ShowModal = true;
            
            // Kullanıcının departman/servisine göre personel listesini yükle
            await LoadPersonelForModal();
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
                OnaySeviyesi = item.OnaySeviyesi,
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
                    var updateDto = new IzinSorumluUpdateDto
                    {
                        IzinSorumluId = EditModel.IzinSorumluId,
                        DepartmanId = EditModel.DepartmanId,
                        ServisId = EditModel.ServisId,
                        SorumluPersonelTcKimlikNo = EditModel.SorumluPersonelTcKimlikNo,
                        OnaySeviyesi = EditModel.OnaySeviyesi,
                        Aktif = EditModel.Aktif,
                        Aciklama = EditModel.Aciklama
                    };

                    var result = await _izinSorumluApiService.UpdateAsync(EditModel.IzinSorumluId, updateDto);

                    if (result.Success)
                    {
                        CloseModal();
                        await LoadSorumlular();
                    }
                    else
                    {
                        ErrorMessage = result.Message ?? "İşlem başarısız";
                        _logger.LogWarning("SaveSorumlu failed: {Message}", result.Message);
                    }
                }
                else
                {
                    var createDto = new IzinSorumluCreateDto
                    {
                        DepartmanId = EditModel.DepartmanId,
                        ServisId = EditModel.ServisId,
                        SorumluPersonelTcKimlikNo = EditModel.SorumluPersonelTcKimlikNo,
                        OnaySeviyesi = EditModel.OnaySeviyesi,
                        Aciklama = EditModel.Aciklama
                    };

                    var result = await _izinSorumluApiService.CreateAsync(createDto);

                    if (result.Success)
                    {
                        CloseModal();
                        await LoadSorumlular();
                    }
                    else
                    {
                        ErrorMessage = result.Message ?? "İşlem başarısız";
                        _logger.LogWarning("SaveSorumlu failed: {Message}", result.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "İşlem sırasında hata oluştu";
                _logger.LogError(ex, "SaveSorumlu exception");
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

        private async Task ToggleSorumlu(int id, bool currentStatus)
        {
            var action = currentStatus ? "pasif" : "aktif";
            var confirmed = await _js.InvokeAsync<bool>("confirm", $"Bu sorumlu atamasını {action} yapmak istediğinizden emin misiniz?");
            if (!confirmed)
                return;

            try
            {
                var result = currentStatus
                    ? await _izinSorumluApiService.DeactivateAsync(id)
                    : await _izinSorumluApiService.ActivateAsync(id);

                if (result.Success)
                {
                    await LoadSorumlular();
                }
                else
                {
                    _logger.LogWarning("ToggleSorumlu failed: {Message}", result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ToggleSorumlu exception");
            }
        }

        private async Task DeleteSorumlu(int id)
        {
            var confirmed = await _js.InvokeAsync<bool>("confirm", "Bu sorumlu atamasını silmek istediğinizden emin misiniz?");
            if (!confirmed)
                return;

            try
            {
                var result = await _izinSorumluApiService.DeleteAsync(id);
                if (result.Success)
                {
                    await LoadSorumlular();
                }
                else
                {
                    _logger.LogWarning("DeleteSorumlu failed: {Message}", result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteSorumlu exception");
            }
        }
    }
}
