using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KioskMenu
{
    public partial class Manage
    {

        [Inject] private IKioskMenuApiService _kioskMenuService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;

        [Parameter] public int? KioskMenuId { get; set; }

        private bool isLoading;
        private bool isSaving;
        private bool IsEditMode => KioskMenuId.HasValue && KioskMenuId.Value > 0;

        private KioskMenuFormModel model = new();
        private bool isAktif = true;

        protected override async Task OnInitializedAsync()
        {
            if (IsEditMode)
            {
                await LoadKioskMenuAsync();
            }
            else
            {
                model = new KioskMenuFormModel
                {
                    MenuAdi = string.Empty,
                    Aciklama = string.Empty,
                    Aktiflik = Aktiflik.Aktif
                };
                isAktif = true;
            }
        }

        private async Task LoadKioskMenuAsync()
        {
            if (!IsEditMode)
            {
                return;
            }

            try
            {
                isLoading = true;
                var result = await _kioskMenuService.GetByIdAsync(KioskMenuId!.Value);
                if (result.Success && result.Data != null)
                {
                    var dto = result.Data;
                    model = new KioskMenuFormModel
                    {
                        MenuAdi = dto.MenuAdi,
                        Aciklama = dto.Aciklama,
                        Aktiflik = dto.Aktiflik
                    };
                    isAktif = dto.Aktiflik == Aktiflik.Aktif;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Kiosk menü bulunamadı");
                    NavigateBack();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk menü yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Menü yüklenirken bir hata oluştu");
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
                    await UpdateKioskMenuAsync();
                }
                else
                {
                    await CreateKioskMenuAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk menü kaydedilirken hata oluştu");
                await _toastService.ShowErrorAsync("Kayıt sırasında bir hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task CreateKioskMenuAsync()
        {
            var createDto = new KioskMenuCreateRequestDto
            {
                MenuAdi = model.MenuAdi,
                Aciklama = model.Aciklama,
                Aktiflik = model.Aktiflik
            };

            var result = await _kioskMenuService.CreateAsync(createDto);
            if (result.Success)
            {
                await _toastService.ShowSuccessAsync("Kiosk menü başarıyla oluşturuldu");
                NavigateBack();
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Kiosk menü oluşturulamadı");
            }
        }

        private async Task UpdateKioskMenuAsync()
        {
            var updateDto = new KioskMenuUpdateRequestDto
            {
                KioskMenuId = KioskMenuId!.Value,
                MenuAdi = model.MenuAdi,
                Aciklama = model.Aciklama,
                Aktiflik = model.Aktiflik
            };

            var result = await _kioskMenuService.UpdateAsync(updateDto);
            if (result.Success)
            {
                await _toastService.ShowSuccessAsync("Kiosk menü başarıyla güncellendi");
                NavigateBack();
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Kiosk menü güncellenemedi");
            }
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/siramatik/kiosk-menu");
        }
    }
}
