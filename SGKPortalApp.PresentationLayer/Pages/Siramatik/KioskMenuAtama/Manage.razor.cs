using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KioskMenuAtama
{
    public partial class Manage
    {
        [Parameter] public int? KioskMenuAtamaId { get; set; }

        [Inject] private IKioskMenuAtamaApiService _kioskMenuAtamaService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IKioskApiService _kioskService { get; set; } = default!;
        [Inject] private IKioskMenuApiService _kioskMenuService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;

        private bool isLoading;
        private bool isSaving;
        private bool isAktif = true;

        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private List<KioskResponseDto> kiosklar = new();
        private List<KioskMenuResponseDto> kioskMenuleri = new();

        private int selectedHizmetBinasiId;
        private KioskMenuAtamaFormModel model = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadDropdownsAsync();

            if (KioskMenuAtamaId.HasValue)
            {
                await LoadKioskMenuAtamaAsync();
            }
        }

        private async Task LoadDropdownsAsync()
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

                // Load Kiosk Menüleri
                var menuResult = await _kioskMenuService.GetAllAsync();
                if (menuResult.Success && menuResult.Data != null)
                {
                    kioskMenuleri = menuResult.Data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropdown verileri yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Veriler yüklenirken bir hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int binaId))
            {
                selectedHizmetBinasiId = binaId;

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
                    kiosklar.Clear();
                }

                model.KioskId = 0; // Reset kiosk selection
            }
        }

        private async Task LoadKioskMenuAtamaAsync()
        {
            if (!KioskMenuAtamaId.HasValue) return;

            try
            {
                isLoading = true;
                var result = await _kioskMenuAtamaService.GetByIdAsync(KioskMenuAtamaId.Value);

                if (result.Success && result.Data != null)
                {
                    var data = result.Data;
                    model = new KioskMenuAtamaFormModel
                    {
                        KioskId = data.KioskId,
                        KioskMenuId = data.KioskMenuId
                    };
                    isAktif = data.Aktiflik == Aktiflik.Aktif;

                    // Load kiosklar for selected bina
                    var kiosk = kiosklar.FirstOrDefault(k => k.KioskId == data.KioskId);
                    if (kiosk != null)
                    {
                        selectedHizmetBinasiId = kiosk.HizmetBinasiId;
                        await OnHizmetBinasiChanged(new ChangeEventArgs { Value = selectedHizmetBinasiId.ToString() });
                    }
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Atama bulunamadı");
                    _navigationManager.NavigateTo("/siramatik/kiosk-menu-atama");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atama yüklenirken hata oluştu. Id: {Id}", KioskMenuAtamaId);
                await _toastService.ShowErrorAsync("Atama yüklenirken bir hata oluştu");
                _navigationManager.NavigateTo("/siramatik/kiosk-menu-atama");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task HandleSubmit()
        {
            if (KioskMenuAtamaId.HasValue)
            {
                await UpdateKioskMenuAtamaAsync();
            }
            else
            {
                await CreateKioskMenuAtamaAsync();
            }
        }

        private async Task CreateKioskMenuAtamaAsync()
        {
            try
            {
                isSaving = true;

                var request = new KioskMenuAtamaCreateRequestDto
                {
                    KioskId = model.KioskId,
                    KioskMenuId = model.KioskMenuId,
                    Aktiflik = isAktif ? Aktiflik.Aktif : Aktiflik.Pasif
                };

                var result = await _kioskMenuAtamaService.CreateAsync(request);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Atama başarıyla oluşturuldu");
                    _navigationManager.NavigateTo("/siramatik/kiosk-menu-atama");
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Atama oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atama oluşturulurken hata oluştu");
                await _toastService.ShowErrorAsync("Atama oluşturulurken bir hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task UpdateKioskMenuAtamaAsync()
        {
            if (!KioskMenuAtamaId.HasValue) return;

            try
            {
                isSaving = true;

                var request = new KioskMenuAtamaUpdateRequestDto
                {
                    KioskMenuAtamaId = KioskMenuAtamaId.Value,
                    KioskId = model.KioskId,
                    KioskMenuId = model.KioskMenuId,
                    Aktiflik = isAktif ? Aktiflik.Aktif : Aktiflik.Pasif
                };

                var result = await _kioskMenuAtamaService.UpdateAsync(request);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Atama başarıyla güncellendi");
                    _navigationManager.NavigateTo("/siramatik/kiosk-menu-atama");
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Atama güncellenemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atama güncellenirken hata oluştu. Id: {Id}", KioskMenuAtamaId);
                await _toastService.ShowErrorAsync("Atama güncellenirken bir hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }
    }
}
