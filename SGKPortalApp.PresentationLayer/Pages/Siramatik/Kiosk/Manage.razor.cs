using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.Kiosk
{
    public partial class Manage
    {
        [Inject] private IKioskApiService _kioskService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;

        [Parameter] public int? KioskId { get; set; }

        // State
        private bool isLoading;
        private bool isSaving;
        private bool IsEditMode => KioskId.HasValue && KioskId.Value > 0;

        // Form model
        private KioskFormModel model = new();
        private bool isAktif = true;

        // Lookups
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadDropdownsAsync();

            if (IsEditMode)
            {
                await LoadKioskAsync();
            }
            else
            {
                model = new KioskFormModel
                {
                    HizmetBinasiId = 0,
                    KioskAdi = string.Empty,
                    KioskIp = string.Empty,
                    Aktiflik = Aktiflik.Aktif
                };
                isAktif = true;
            }
        }

        private async Task LoadDropdownsAsync()
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
                else
                {
                    hizmetBinalari = new List<HizmetBinasiResponseDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropdown verileri yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Liste verileri yüklenirken hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task LoadKioskAsync()
        {
            if (!IsEditMode)
            {
                return;
            }

            try
            {
                isLoading = true;
                var result = await _kioskService.GetByIdAsync(KioskId!.Value);

                if (result.Success && result.Data != null)
                {
                    var kiosk = result.Data;
                    model = new KioskFormModel
                    {
                        KioskId = kiosk.KioskId,
                        KioskAdi = kiosk.KioskAdi,
                        HizmetBinasiId = kiosk.HizmetBinasiId,
                        KioskIp = kiosk.KioskIp,
                        Aktiflik = kiosk.Aktiflik
                    };
                    isAktif = kiosk.Aktiflik == Aktiflik.Aktif;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Kiosk bulunamadı");
                    NavigateBack();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Kiosk yüklenirken hata oluştu");
                NavigateBack();
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task HandleSubmit()
        {
            try
            {
                isSaving = true;
                model.Aktiflik = isAktif ? Aktiflik.Aktif : Aktiflik.Pasif;

                if (IsEditMode)
                {
                    await UpdateKioskAsync();
                }
                else
                {
                    await CreateKioskAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk kaydedilirken hata oluştu");
                await _toastService.ShowErrorAsync("Kayıt sırasında bir hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task CreateKioskAsync()
        {
            var createDto = new KioskCreateRequestDto
            {
                KioskAdi = model.KioskAdi,
                HizmetBinasiId = model.HizmetBinasiId,
                KioskIp = model.KioskIp,
                Aktiflik = model.Aktiflik
            };

            var result = await _kioskService.CreateAsync(createDto);
            if (result.Success)
            {
                await _toastService.ShowSuccessAsync("Kiosk başarıyla oluşturuldu");
                NavigateBack();
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Kiosk oluşturulamadı");
            }
        }

        private async Task UpdateKioskAsync()
        {
            var updateDto = new KioskUpdateRequestDto
            {
                KioskId = model.KioskId,
                KioskAdi = model.KioskAdi,
                HizmetBinasiId = model.HizmetBinasiId,
                KioskIp = model.KioskIp,
                Aktiflik = model.Aktiflik
            };

            var result = await _kioskService.UpdateAsync(updateDto);
            if (result.Success)
            {
                await _toastService.ShowSuccessAsync("Kiosk başarıyla güncellendi");
                NavigateBack();
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Kiosk güncellenemedi");
            }
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/siramatik/kiosk/list");
        }
    }
}
