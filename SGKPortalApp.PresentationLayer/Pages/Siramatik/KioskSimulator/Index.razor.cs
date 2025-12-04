using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KioskSimulator
{
    public partial class Index
    {
        [Inject] private IKioskSimulatorApiService _kioskSimulatorService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;

        // State
        private bool isLoading = false;
        private int selectedHizmetBinasiId = 0;
        private KioskStep currentStep = KioskStep.MenuSecimi;

        // Data
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private List<KioskMenuDto> kioskMenuler = new();
        private List<KioskAltIslemDto> altIslemler = new();

        // Selections
        private KioskMenuDto? selectedMenu = null;
        private KioskAltIslemDto? selectedAltIslem = null;
        private KioskSiraAlResponseDto? siraAlResponse = null;

        protected override async Task OnInitializedAsync()
        {
            await LoadHizmetBinalariAsync();
        }

        private async Task LoadHizmetBinalariAsync()
        {
            try
            {
                var result = await _hizmetBinasiService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    hizmetBinalari = result.Data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet binaları yüklenirken hata");
                await _toastService.ShowErrorAsync("Hizmet binaları yüklenemedi");
            }
        }

        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var hizmetBinasiId))
            {
                selectedHizmetBinasiId = hizmetBinasiId;
                ResetKiosk();

                if (hizmetBinasiId > 0)
                {
                    await LoadKioskMenulerAsync();
                }
            }
        }

        private async Task LoadKioskMenulerAsync()
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                var result = await _kioskSimulatorService.GetKioskMenulerAsync(selectedHizmetBinasiId);
                if (result.Success && result.Data != null)
                {
                    kioskMenuler = result.Data;
                    _logger.LogInformation("Kiosk menüleri yüklendi: {Count} menü", kioskMenuler.Count);
                }
                else
                {
                    kioskMenuler = new();
                    await _toastService.ShowWarningAsync(result.Message ?? "Menüler yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk menüleri yüklenirken hata");
                await _toastService.ShowErrorAsync("Menüler yüklenemedi");
                kioskMenuler = new();
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task SelectMenu(KioskMenuDto menu)
        {
            selectedMenu = menu;
            currentStep = KioskStep.AltIslemSecimi;
            
            isLoading = true;
            StateHasChanged();

            try
            {
                var result = await _kioskSimulatorService.GetKioskMenuAltIslemleriAsync(selectedHizmetBinasiId, menu.KioskMenuId);
                if (result.Success && result.Data != null)
                {
                    altIslemler = result.Data;
                    _logger.LogInformation("Alt işlemler yüklendi: {Count} işlem", altIslemler.Count);
                }
                else
                {
                    altIslemler = new();
                    await _toastService.ShowWarningAsync(result.Message ?? "Alt işlemler yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Alt işlemler yüklenirken hata");
                await _toastService.ShowErrorAsync("Alt işlemler yüklenemedi");
                altIslemler = new();
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task SelectAltIslem(KioskAltIslemDto islem)
        {
            selectedAltIslem = islem;
            
            isLoading = true;
            StateHasChanged();

            try
            {
                var request = new KioskSiraAlRequestDto
                {
                    KioskMenuIslemId = islem.KioskMenuIslemId,
                    HizmetBinasiId = selectedHizmetBinasiId
                };

                var result = await _kioskSimulatorService.SiraAlAsync(request);
                if (result.Success && result.Data != null)
                {
                    siraAlResponse = result.Data;
                    currentStep = KioskStep.SiraAlindi;
                    await _toastService.ShowSuccessAsync($"Sıra No: {siraAlResponse.SiraNo} alındı!");
                    _logger.LogInformation("Sıra alındı: {SiraNo}", siraAlResponse.SiraNo);
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Sıra alınamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sıra alınırken hata");
                await _toastService.ShowErrorAsync("Sıra alınamadı");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void GoBack()
        {
            if (currentStep == KioskStep.AltIslemSecimi)
            {
                currentStep = KioskStep.MenuSecimi;
                selectedMenu = null;
                altIslemler = new();
            }
            else if (currentStep == KioskStep.SiraAlindi)
            {
                currentStep = KioskStep.AltIslemSecimi;
                siraAlResponse = null;
            }
        }

        private void ResetKiosk()
        {
            currentStep = KioskStep.MenuSecimi;
            selectedMenu = null;
            selectedAltIslem = null;
            siraAlResponse = null;
            kioskMenuler = new();
            altIslemler = new();
        }

        private string GetSelectedHizmetBinasiAdi()
        {
            return hizmetBinalari.FirstOrDefault(h => h.HizmetBinasiId == selectedHizmetBinasiId)?.HizmetBinasiAdi ?? "-";
        }

        private string GetCurrentStepName()
        {
            return currentStep switch
            {
                KioskStep.MenuSecimi => "Menü Seçimi",
                KioskStep.AltIslemSecimi => "Alt İşlem Seçimi",
                KioskStep.SiraAlindi => "Sıra Alındı",
                _ => "-"
            };
        }

        // Enum for steps
        private enum KioskStep
        {
            MenuSecimi,
            AltIslemSecimi,
            SiraAlindi
        }
    }
}
