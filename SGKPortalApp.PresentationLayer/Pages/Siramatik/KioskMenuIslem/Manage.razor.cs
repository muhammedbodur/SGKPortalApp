using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KioskMenuIslem
{
    public partial class Manage
    {
        [Parameter] public int? KioskMenuIslemId { get; set; }
        [Parameter] [SupplyParameterFromQuery] public int? kioskMenuId { get; set; }

        [Inject] private IKioskMenuApiService _kioskMenuService { get; set; } = default!;
        [Inject] private IKioskMenuIslemApiService _kioskMenuIslemService { get; set; } = default!;
        [Inject] private IKanalAltApiService _kanalAltService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;

        private bool isLoading;
        private bool isSaving;
        private bool isAktif = true;

        private List<KioskMenuResponseDto> kioskMenuleri = new();
        private List<KanalAltResponseDto> kanalAltlar = new();

        private KioskMenuIslemFormModel model = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadDropdownsAsync();

            if (KioskMenuIslemId.HasValue)
            {
                await LoadKioskMenuIslemAsync();
            }
            else if (kioskMenuId.HasValue)
            {
                model.KioskMenuId = kioskMenuId.Value;
            }
        }

        private async Task LoadDropdownsAsync()
        {
            try
            {
                isLoading = true;

                // Load Kiosk Menüleri
                var menuResult = await _kioskMenuService.GetAllAsync();
                if (menuResult.Success && menuResult.Data != null)
                {
                    kioskMenuleri = menuResult.Data
                        .OrderBy(x => x.MenuAdi)
                        .ToList();
                }

                // Load Kanal Altlar
                var kanalAltResult = await _kanalAltService.GetAllAsync();
                if (kanalAltResult.Success && kanalAltResult.Data != null)
                {
                    kanalAltlar = kanalAltResult.Data
                        .Where(x => x.Aktiflik == Aktiflik.Aktif)
                        .OrderBy(x => x.KanalAltAdi)
                        .ToList();
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

        private async Task LoadKioskMenuIslemAsync()
        {
            if (!KioskMenuIslemId.HasValue) return;

            try
            {
                isLoading = true;
                var result = await _kioskMenuIslemService.GetByIdAsync(KioskMenuIslemId.Value);

                if (result.Success && result.Data != null)
                {
                    var data = result.Data;
                    model = new KioskMenuIslemFormModel
                    {
                        KioskMenuId = data.KioskMenuId,
                        KanalAltId = data.KanalAltId,
                        MenuSira = data.MenuSira
                    };
                    isAktif = data.Aktiflik == Aktiflik.Aktif;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İşlem bulunamadı");
                    _navigationManager.NavigateTo("/siramatik/kiosk-menu-islem");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İşlem yüklenirken hata oluştu. Id: {Id}", KioskMenuIslemId);
                await _toastService.ShowErrorAsync("İşlem yüklenirken bir hata oluştu");
                _navigationManager.NavigateTo("/siramatik/kiosk-menu-islem");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task HandleSubmit()
        {
            if (KioskMenuIslemId.HasValue)
            {
                await UpdateKioskMenuIslemAsync();
            }
            else
            {
                await CreateKioskMenuIslemAsync();
            }
        }

        private async Task CreateKioskMenuIslemAsync()
        {
            try
            {
                isSaving = true;

                // İşlem adını seçilen KanalAlt'tan otomatik al
                var selectedKanalAlt = kanalAltlar.FirstOrDefault(ka => ka.KanalAltId == model.KanalAltId);
                var islemAdi = selectedKanalAlt?.KanalAltAdi ?? "Bilinmeyen İşlem";

                var request = new KioskMenuIslemCreateRequestDto
                {
                    KioskMenuId = model.KioskMenuId,
                    IslemAdi = islemAdi,
                    KanalAltId = model.KanalAltId,
                    MenuSira = model.MenuSira,
                    Aktiflik = isAktif ? Aktiflik.Aktif : Aktiflik.Pasif
                };

                var result = await _kioskMenuIslemService.CreateAsync(request);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("İşlem başarıyla oluşturuldu");
                    _navigationManager.NavigateTo("/siramatik/kiosk-menu-islem");
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İşlem oluşturulamadı");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İşlem oluşturulurken hata oluştu");
                await _toastService.ShowErrorAsync("İşlem oluşturulurken bir hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task UpdateKioskMenuIslemAsync()
        {
            if (!KioskMenuIslemId.HasValue) return;

            try
            {
                isSaving = true;

                // İşlem adını seçilen KanalAlt'tan otomatik al
                var selectedKanalAlt = kanalAltlar.FirstOrDefault(ka => ka.KanalAltId == model.KanalAltId);
                var islemAdi = selectedKanalAlt?.KanalAltAdi ?? "Bilinmeyen İşlem";

                var request = new KioskMenuIslemUpdateRequestDto
                {
                    KioskMenuIslemId = KioskMenuIslemId.Value,
                    KioskMenuId = model.KioskMenuId,
                    IslemAdi = islemAdi,
                    KanalAltId = model.KanalAltId,
                    MenuSira = model.MenuSira,
                    Aktiflik = isAktif ? Aktiflik.Aktif : Aktiflik.Pasif
                };

                var result = await _kioskMenuIslemService.UpdateAsync(request);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("İşlem başarıyla güncellendi");
                    _navigationManager.NavigateTo("/siramatik/kiosk-menu-islem");
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İşlem güncellenemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İşlem güncellenirken hata oluştu. Id: {Id}", KioskMenuIslemId);
                await _toastService.ShowErrorAsync("İşlem güncellenirken bir hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }
    }
}
