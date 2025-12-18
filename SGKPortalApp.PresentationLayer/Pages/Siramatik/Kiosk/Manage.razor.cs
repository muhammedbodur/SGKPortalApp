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
        protected override string PagePermissionKey => "SIRA.KIOSK.MANAGE";

        [Inject] private IKioskApiService _kioskService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;

        [Parameter] public int? KioskId { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int? HizmetBinasiId { get; set; }

        // State
        private bool isLoading;
        private bool isSaving;
        private bool IsEditMode => KioskId.HasValue && KioskId.Value > 0;

        // Form model
        private KioskFormModel model = new();
        private bool isAktif = true;

        // Lookups
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private int selectedHizmetBinasiId = 0;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
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

                // ✅ Güvenlik: URL'den HizmetBinasiId parametresi geldiyse yetki kontrolü
                if (HizmetBinasiId.HasValue && HizmetBinasiId.Value > 0)
                {
                    if (!CanAccessHizmetBinasi(HizmetBinasiId.Value))
                    {
                        await _toastService.ShowWarningAsync("Bu Hizmet Binasına erişim yetkiniz yok!");
                        _logger.LogWarning("Yetkisiz Hizmet Binası erişim denemesi (URL): {BinaId}", HizmetBinasiId.Value);
                        _navigationManager.NavigateTo("/siramatik/kiosk/list");
                        return;
                    }

                    selectedHizmetBinasiId = HizmetBinasiId.Value;
                    model.HizmetBinasiId = HizmetBinasiId.Value;
                }
                else
                {
                    // ✅ URL'den parametre gelmediyse kullanıcının kendi HizmetBinası'nı seç
                    var userHizmetBinasiId = GetCurrentUserHizmetBinasiId();
                    if (userHizmetBinasiId > 0)
                    {
                        selectedHizmetBinasiId = userHizmetBinasiId;
                        model.HizmetBinasiId = userHizmetBinasiId;
                    }
                }

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

        private async void OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int hizmetBinasiId))
            {
                // ✅ Güvenlik kontrolü
                if (hizmetBinasiId > 0 && !CanAccessHizmetBinasi(hizmetBinasiId))
                {
                    await _toastService.ShowWarningAsync("Bu Hizmet Binasını seçme yetkiniz yok!");
                    _logger.LogWarning("Yetkisiz Hizmet Binası seçim denemesi: {BinaId}", hizmetBinasiId);
                    return;
                }

                selectedHizmetBinasiId = hizmetBinasiId;
                model.HizmetBinasiId = hizmetBinasiId;
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
            // ✅ Güvenlik: Form submit öncesi son kontrol (form manipulation önlemi)
            if (!CanAccessHizmetBinasi(model.HizmetBinasiId))
            {
                await _toastService.ShowErrorAsync("Bu Hizmet Binasında kayıt oluşturma yetkiniz yok!");
                _logger.LogWarning("Yetkisiz kayıt oluşturma denemesi: HizmetBinasiId={BinaId}", model.HizmetBinasiId);
                return;
            }

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
