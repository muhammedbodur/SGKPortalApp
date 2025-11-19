using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KioskMenuAtama
{
    public partial class Index
    {
        [Inject] private IKioskMenuAtamaApiService _kioskMenuAtamaService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IKioskApiService _kioskService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;

        private bool isLoading;
        private bool isDeleting;
        private bool showDeleteModal;

        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private List<KioskResponseDto> kiosklar = new();
        private List<KioskMenuAtamaResponseDto> allAtamalar = new();
        private List<KioskMenuAtamaResponseDto> filteredAtamalar = new();

        private int selectedHizmetBinasiId;
        private int selectedKioskId;
        private Aktiflik? selectedAktiflik;
        private KioskMenuAtamaResponseDto? selectedAtama;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                isLoading = true;

                // Load Hizmet Binaları
                var binaResult = await _hizmetBinasiService.GetAllAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data;
                }

                // Load Kiosklar
                var kioskResult = await _kioskService.GetAllAsync();
                if (kioskResult.Success && kioskResult.Data != null)
                {
                    kiosklar = kioskResult.Data;
                }

                // Load Atamalar
                await LoadAtamalarAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Veriler yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Veriler yüklenirken bir hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task LoadAtamalarAsync()
        {
            try
            {
                var result = await _kioskMenuAtamaService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    allAtamalar = result.Data;
                    ApplyFilters();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Atamalar yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atamalar yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Atamalar yüklenirken bir hata oluştu");
            }
        }

        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int binaId))
            {
                selectedHizmetBinasiId = binaId;
                selectedKioskId = 0; // Reset kiosk filter

                // Reload kiosklar for selected bina
                if (binaId > 0)
                {
                    var result = await _kioskService.GetByHizmetBinasiAsync(binaId);
                    if (result.Success && result.Data != null)
                    {
                        kiosklar = result.Data;
                    }
                }
                else
                {
                    var result = await _kioskService.GetAllAsync();
                    if (result.Success && result.Data != null)
                    {
                        kiosklar = result.Data;
                    }
                }

                ApplyFilters();
            }
        }

        private void OnKioskChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int kioskId))
            {
                selectedKioskId = kioskId;
                ApplyFilters();
            }
        }

        private void OnAktiflikChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value?.ToString()))
            {
                selectedAktiflik = null;
            }
            else if (int.TryParse(e.Value?.ToString(), out int aktiflikValue))
            {
                selectedAktiflik = (Aktiflik)aktiflikValue;
            }
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            filteredAtamalar = allAtamalar;

            if (selectedKioskId > 0)
            {
                filteredAtamalar = filteredAtamalar.Where(a => a.KioskId == selectedKioskId).ToList();
            }

            if (selectedAktiflik.HasValue)
            {
                filteredAtamalar = filteredAtamalar.Where(a => a.Aktiflik == selectedAktiflik.Value).ToList();
            }
        }

        private void ShowDeleteConfirm(KioskMenuAtamaResponseDto atama)
        {
            selectedAtama = atama;
            showDeleteModal = true;
        }

        private void HideDeleteConfirm()
        {
            showDeleteModal = false;
            selectedAtama = null;
        }

        private async Task DeleteAtama()
        {
            if (selectedAtama == null) return;

            try
            {
                isDeleting = true;
                var result = await _kioskMenuAtamaService.DeleteAsync(selectedAtama.KioskMenuAtamaId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Atama başarıyla silindi");
                    await LoadAtamalarAsync();
                    HideDeleteConfirm();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Atama silinemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atama silinirken hata oluştu. Id: {Id}", selectedAtama.KioskMenuAtamaId);
                await _toastService.ShowErrorAsync("Atama silinirken bir hata oluştu");
            }
            finally
            {
                isDeleting = false;
            }
        }
    }
}
