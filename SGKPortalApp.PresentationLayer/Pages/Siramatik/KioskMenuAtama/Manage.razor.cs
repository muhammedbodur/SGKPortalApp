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

        [Parameter]
        [SupplyParameterFromQuery]
        public int? HizmetBinasiId { get; set; }

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
        private List<KioskMenuResponseDto> allKioskMenuleri = new();
        private List<KioskMenuResponseDto> availableMenuleri = new();

        private int selectedHizmetBinasiId;
        private KioskMenuAtamaFormModel model = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadDropdownsAsync();

            if (KioskMenuAtamaId.HasValue)
            {
                await LoadKioskMenuAtamaAsync();
            }
            else
            {
                // ✅ Güvenlik: URL'den HizmetBinasiId parametresi geldiyse yetki kontrolü
                if (HizmetBinasiId.HasValue && HizmetBinasiId.Value > 0)
                {
                    if (!CanAccessHizmetBinasi(HizmetBinasiId.Value))
                    {
                        await _toastService.ShowWarningAsync("Bu Hizmet Binasına erişim yetkiniz yok!");
                        _logger.LogWarning("Yetkisiz Hizmet Binası erişim denemesi (URL): {BinaId}", HizmetBinasiId.Value);
                        _navigationManager.NavigateTo("/siramatik/kiosk-menu-atama");
                        return;
                    }

                    selectedHizmetBinasiId = HizmetBinasiId.Value;
                    await OnHizmetBinasiChanged(new ChangeEventArgs { Value = selectedHizmetBinasiId.ToString() });
                }
                else
                {
                    // ✅ URL'den parametre gelmediyse kullanıcının kendi HizmetBinası'nı seç
                    var userHizmetBinasiId = GetCurrentUserHizmetBinasiId();
                    if (userHizmetBinasiId > 0)
                    {
                        selectedHizmetBinasiId = userHizmetBinasiId;
                        await OnHizmetBinasiChanged(new ChangeEventArgs { Value = selectedHizmetBinasiId.ToString() });
                    }
                }
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
                    hizmetBinalari = binaResult.Data
                        .Where(x => x.Aktiflik == Aktiflik.Aktif)
                        .OrderBy(x => x.HizmetBinasiAdi)
                        .ToList();
                }

                // Load Kiosk Menüleri
                var menuResult = await _kioskMenuService.GetAllAsync();
                if (menuResult.Success && menuResult.Data != null)
                {
                    allKioskMenuleri = menuResult.Data
                        .Where(x => x.Aktiflik == Aktiflik.Aktif)
                        .OrderBy(x => x.MenuAdi)
                        .ToList();
                    availableMenuleri = allKioskMenuleri;
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
                // ✅ Güvenlik kontrolü
                if (binaId > 0 && !CanAccessHizmetBinasi(binaId))
                {
                    await _toastService.ShowWarningAsync("Bu Hizmet Binasını seçme yetkiniz yok!");
                    _logger.LogWarning("Yetkisiz Hizmet Binası seçim denemesi: {BinaId}", binaId);
                    return;
                }

                selectedHizmetBinasiId = binaId;

                if (binaId > 0)
                {
                    var result = await _kioskService.GetByHizmetBinasiAsync(binaId);
                    if (result.Success && result.Data != null)
                    {
                        kiosklar = result.Data
                            .Where(x => x.Aktiflik == Aktiflik.Aktif)
                            .OrderBy(x => x.KioskAdi)
                            .ToList();
                    }
                }
                else
                {
                    kiosklar.Clear();
                }

                model.KioskId = 0; // Reset kiosk selection
                await FilterAvailableMenusAsync();
            }
        }

        private async Task OnKioskChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int kioskId))
            {
                model.KioskId = kioskId;
                await FilterAvailableMenusAsync();
            }
        }

        private async Task FilterAvailableMenusAsync()
        {
            if (model.KioskId <= 0)
            {
                availableMenuleri = allKioskMenuleri;
                return;
            }

            try
            {
                // Seçili kiosk'a atanmış aktif menüleri al
                var atamalarResult = await _kioskMenuAtamaService.GetByKioskAsync(model.KioskId);
                
                if (atamalarResult.Success && atamalarResult.Data != null)
                {
                    // Zaten atanmış menüleri filtrele (edit modda mevcut atamayı hariç tut)
                    var atanmisMenuIdleri = atamalarResult.Data
                        .Where(x => x.Aktiflik == Aktiflik.Aktif && (!KioskMenuAtamaId.HasValue || x.KioskMenuAtamaId != KioskMenuAtamaId.Value))
                        .Select(x => x.KioskMenuId)
                        .ToList();

                    // Atanmamış menüleri göster (bir kiosk'a birden fazla menü atanabilir, ama aynı menü tekrar atanamaz)
                    availableMenuleri = allKioskMenuleri
                        .Where(m => !atanmisMenuIdleri.Contains(m.KioskMenuId))
                        .ToList();
                }
                else
                {
                    availableMenuleri = allKioskMenuleri;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Menüler filtrelenirken hata oluştu");
                availableMenuleri = allKioskMenuleri;
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

                    // Kiosk bilgisini al ve hizmet binasını seç
                    var kioskResult = await _kioskService.GetByIdAsync(data.KioskId);
                    if (kioskResult.Success && kioskResult.Data != null)
                    {
                        selectedHizmetBinasiId = kioskResult.Data.HizmetBinasiId;
                        await OnHizmetBinasiChanged(new ChangeEventArgs { Value = selectedHizmetBinasiId.ToString() });
                        
                        // Menüleri filtrele (edit modda mevcut atamayı hariç tut)
                        await FilterAvailableMenusAsync();
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

                // ✅ Güvenlik: Form submit öncesi son kontrol (form manipulation önlemi)
                // Seçilen Kiosk'un HizmetBinası'nı kontrol et
                var selectedKiosk = kiosklar.FirstOrDefault(k => k.KioskId == model.KioskId);
                if (selectedKiosk != null && !CanAccessHizmetBinasi(selectedKiosk.HizmetBinasiId))
                {
                    await _toastService.ShowErrorAsync("Bu Hizmet Binasında kayıt oluşturma yetkiniz yok!");
                    _logger.LogWarning("Yetkisiz kayıt oluşturma denemesi: HizmetBinasiId={BinaId}, KioskId={KioskId}",
                        selectedKiosk.HizmetBinasiId, model.KioskId);
                    return;
                }

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
