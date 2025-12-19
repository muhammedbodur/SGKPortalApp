using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.Common.Extensions;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.Kiosk
{
    public partial class Index
    {

        [Inject] private IKioskApiService _kioskService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;

        // State
        private bool isLoading;
        private bool isDeleting;
        private bool showDeleteModal;

        // Data
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private List<KioskResponseDto> allKiosklar = new();
        private List<KioskResponseDto> filteredKiosklar = new();
        private KioskResponseDto? selectedKiosk;

        // Filters
        private int selectedHizmetBinasiId;
        private Aktiflik? selectedAktiflik;
        private string searchText = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadDropdownDataAsync();

            // Kullanıcının kendi Hizmet Binasını default olarak seç
            var userHizmetBinasiId = GetCurrentUserHizmetBinasiId();
            if (userHizmetBinasiId > 0 && hizmetBinalari.Any(b => b.HizmetBinasiId == userHizmetBinasiId))
            {
                selectedHizmetBinasiId = userHizmetBinasiId;
                await LoadKiosksAsync();
            }
            else if (hizmetBinalari.Any())
            {
                selectedHizmetBinasiId = hizmetBinalari.First().HizmetBinasiId;
                await LoadKiosksAsync();
            }
        }

        private async Task LoadDropdownDataAsync()
        {
            try
            {
                isLoading = true;

                var binaResult = await _hizmetBinasiService.GetActiveAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data
                        .OrderBy(x => x.HizmetBinasiAdi)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropdown verileri yüklenemedi");
                await _toastService.ShowErrorAsync("Liste verileri yüklenirken hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task LoadKiosksAsync()
        {
            if (selectedHizmetBinasiId <= 0)
            {
                allKiosklar = new();
                filteredKiosklar = new();
                return;
            }

            try
            {
                isLoading = true;
                var result = await _kioskService.GetByHizmetBinasiAsync(selectedHizmetBinasiId);
                if (result.Success && result.Data != null)
                {
                    allKiosklar = result.Data;
                    ApplyFilters();
                }
                else
                {
                    allKiosklar = new();
                    filteredKiosklar = new();
                    await _toastService.ShowWarningAsync(result.Message ?? "Kiosk bulunamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosklar yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Kiosklar yüklenirken hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ApplyFilters()
        {
            filteredKiosklar = allKiosklar.ToList();

            if (selectedAktiflik.HasValue)
            {
                filteredKiosklar = filteredKiosklar
                    .Where(k => k.Aktiflik == selectedAktiflik.Value)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredKiosklar = filteredKiosklar
                    .Where(k => k.KioskAdi.ContainsTurkish(searchText, StringComparison.OrdinalIgnoreCase)
                             || (k.KioskIp?.ContainsTurkish(searchText, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }
        }

        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            // ✅ 1. YETKİ KONTROLÜ: Kullanıcı bu filtreyi değiştirebilir mi?
            if (!CanEditFieldInList("HIZMET_BINASI"))
            {
                await _toastService.ShowWarningAsync("Bu filtreyi değiştirme yetkiniz yok!");
                _logger.LogWarning("Yetkisiz filtre değiştirme denemesi: HIZMET_BINASI");
                return; // ❌ İşlemi durdur
            }

            if (int.TryParse(e.Value?.ToString(), out var binaId))
            {
                // ✅ 2. DATA VALİDATİON: Kullanıcı başka Hizmet Binasını seçmeye çalışıyor mu?
                if (binaId > 0 && !CanAccessHizmetBinasi(binaId))
                {
                    await _toastService.ShowWarningAsync("Bu Hizmet Binasını görüntüleme yetkiniz yok!");
                    _logger.LogWarning("Yetkisiz Hizmet Binası erişim denemesi: {BinaId}", binaId);
                    return; // ❌ İşlemi durdur
                }

                selectedHizmetBinasiId = binaId;
                selectedAktiflik = null;
                searchText = string.Empty;
                await LoadKiosksAsync();
            }
        }

        private void OnAktiflikChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value?.ToString()))
            {
                selectedAktiflik = null;
            }
            else if (int.TryParse(e.Value.ToString(), out var aktifValue))
            {
                selectedAktiflik = (Aktiflik)aktifValue;
            }

            ApplyFilters();
        }

        private void OnSearchChanged()
        {
            ApplyFilters();
        }

        private async Task RefreshAsync()
        {
            await LoadKiosksAsync();
        }

        private string GetManageUrl()
        {
            if (selectedHizmetBinasiId > 0)
            {
                return $"/siramatik/kiosk/manage?hizmetBinasiId={selectedHizmetBinasiId}";
            }
            return "/siramatik/kiosk/manage";
        }

        private void ShowDeleteConfirm(KioskResponseDto kiosk)
        {
            selectedKiosk = kiosk;
            showDeleteModal = true;
        }

        private void HideDeleteConfirm()
        {
            showDeleteModal = false;
            selectedKiosk = null;
        }

        private async Task DeleteKioskAsync()
        {
            if (selectedKiosk == null)
            {
                return;
            }

            try
            {
                isDeleting = true;
                var result = await _kioskService.DeleteAsync(selectedKiosk.KioskId);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Kiosk silindi");
                    HideDeleteConfirm();
                    await LoadKiosksAsync();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Kiosk silinemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk silinirken hata oluştu");
                await _toastService.ShowErrorAsync("Kiosk silinirken hata oluştu");
            }
            finally
            {
                isDeleting = false;
            }
        }
    }
}
