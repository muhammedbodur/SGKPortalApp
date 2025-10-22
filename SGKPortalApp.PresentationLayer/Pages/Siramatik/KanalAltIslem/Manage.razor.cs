using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KanalAltIslem
{
    public partial class Manage
    {
        [Inject] private IKanalAltIslemApiService _kanalAltIslemService { get; set; } = default!;
        [Inject] private IKanalIslemApiService _kanalIslemService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;

        [Parameter] public int? KanalAltIslemId { get; set; }
        
        [Parameter]
        [SupplyParameterFromQuery]
        public int? HizmetBinasiId { get; set; }

        // State
        private bool isLoading = false;
        private bool isSaving = false;
        private bool IsEditMode => KanalAltIslemId.HasValue && KanalAltIslemId.Value > 0;

        // Form Model
        private KanalAltIslemFormModel model = new();
        private bool isAktif = true;

        // Lookup Data
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private List<KanalIslemResponseDto> kanalIslemler = new();
        private List<KanalAltResponseDto> kanalAltlar = new();
        private int selectedHizmetBinasiId = 0;

        // Edit Mode Data
        private DateTime eklenmeTarihi = DateTime.Now;
        private DateTime? duzenlenmeTarihi;

        protected override async Task OnInitializedAsync()
        {
            await LoadDropdownData();

            if (IsEditMode)
            {
                await LoadKanalAltIslem();
            }
            else
            {
                // Yeni kayıt için varsayılan değerler
                model = new KanalAltIslemFormModel
                {
                    Aktiflik = Aktiflik.Aktif
                };
                isAktif = true;
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                // Hizmet binalarını yükle
                var binaResult = await _hizmetBinasiService.GetActiveAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data;
                }
                else
                {
                    hizmetBinalari = new();
                    await _toastService.ShowWarningAsync("Hizmet binaları yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropdown verileri yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Veriler yüklenirken bir hata oluştu");
            }
        }

        private async Task LoadKanalIslemler()
        {
            if (selectedHizmetBinasiId <= 0)
            {
                kanalIslemler = new();
                return;
            }

            try
            {
                var result = await _kanalIslemService.GetByHizmetBinasiIdAsync(selectedHizmetBinasiId);

                if (result.Success && result.Data != null)
                {
                    kanalIslemler = result.Data.Where(k => k.Aktiflik == Aktiflik.Aktif).ToList();
                }
                else
                {
                    kanalIslemler = new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlemler yüklenirken hata oluştu");
                kanalIslemler = new();
            }
        }

        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int binaId))
            {
                selectedHizmetBinasiId = binaId;
                model.KanalIslemId = 0; // Kanal işlem seçimini sıfırla
                await LoadKanalIslemler();
                StateHasChanged();
            }
        }

        private async Task LoadKanalAltIslem()
        {
            try
            {
                isLoading = true;
                var result = await _kanalAltIslemService.GetByIdWithDetailsAsync(KanalAltIslemId!.Value);

                if (result.Success && result.Data != null)
                {
                    var altIslem = result.Data;
                    
                    model = new KanalAltIslemFormModel
                    {
                        KanalAltId = altIslem.KanalAltId,
                        KanalIslemId = altIslem.KanalIslemId,
                        HizmetBinasiId = altIslem.HizmetBinasiId,
                        Aktiflik = altIslem.Aktiflik
                    };

                    // Hizmet binasını seç ve kanal işlemleri yükle
                    selectedHizmetBinasiId = altIslem.HizmetBinasiId;
                    await LoadKanalIslemler();

                    isAktif = altIslem.Aktiflik == Aktiflik.Aktif;
                    eklenmeTarihi = altIslem.EklenmeTarihi;
                    duzenlenmeTarihi = altIslem.DuzenlenmeTarihi;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Alt işlem bulunamadı");
                    _navigationManager.NavigateTo("/siramatik/kanal-alt-islem");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Alt işlem yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Alt işlem yüklenirken bir hata oluştu");
                _navigationManager.NavigateTo("/siramatik/kanal-alt-islem");
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

                // Aktiflik durumunu modele aktar
                model.Aktiflik = isAktif ? Aktiflik.Aktif : Aktiflik.Pasif;

                // Validasyon
                if (selectedHizmetBinasiId <= 0)
                {
                    await _toastService.ShowErrorAsync("Lütfen bir hizmet binası seçin");
                    return;
                }

                if (model.KanalIslemId <= 0)
                {
                    await _toastService.ShowErrorAsync("Lütfen bir kanal işlem seçin");
                    return;
                }

                // HizmetBinasiId'yi modele aktar
                model.HizmetBinasiId = selectedHizmetBinasiId;

                if (IsEditMode)
                {
                    await UpdateKanalAltIslem();
                }
                else
                {
                    await CreateKanalAltIslem();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Form gönderilirken hata oluştu");
                await _toastService.ShowErrorAsync("İşlem sırasında bir hata oluştu");
            }
            finally
            {
                isSaving = false;
            }
        }

        private async Task CreateKanalAltIslem()
        {
            var createDto = new KanalAltCreateRequestDto
            {
                KanalAltId = model.KanalAltId,
                KanalIslemId = model.KanalIslemId,
                HizmetBinasiId = model.HizmetBinasiId,
                Sira = model.Sira,
                Aktiflik = model.Aktiflik
            };

            var result = await _kanalAltIslemService.CreateAsync(createDto);

            if (result.Success)
            {
                await _toastService.ShowSuccessAsync("Kanal alt işlem başarıyla eklendi");
                _navigationManager.NavigateTo("/siramatik/kanal-alt-islem");
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Alt işlem eklenemedi");
            }
        }

        private async Task UpdateKanalAltIslem()
        {
            var updateDto = new KanalAltUpdateRequestDto
            {
                KanalAltId = model.KanalAltId,
                KanalIslemId = model.KanalIslemId,
                HizmetBinasiId = model.HizmetBinasiId,
                Sira = model.Sira,
                Aktiflik = model.Aktiflik
            };

            var result = await _kanalAltIslemService.UpdateAsync(KanalAltIslemId!.Value, updateDto);

            if (result.Success)
            {
                await _toastService.ShowSuccessAsync("Kanal alt işlem başarıyla güncellendi");
                _navigationManager.NavigateTo("/siramatik/kanal-alt-islem");
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Alt işlem güncellenemedi");
            }
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/siramatik/kanal-alt-islem");
        }
    }
}
