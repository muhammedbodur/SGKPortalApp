using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KanalAltIslem
{
    public partial class Manage
    {
        [Inject] private IKanalAltIslemApiService _kanalAltIslemService { get; set; } = default!;
        [Inject] private IKanalIslemApiService _kanalIslemService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;

        [Parameter] public int? KanalAltIslemId { get; set; }

        // State
        private bool isLoading = false;
        private bool isSaving = false;
        private bool IsEditMode => KanalAltIslemId.HasValue && KanalAltIslemId.Value > 0;

        // Form Model
        private KanalAltIslemFormModel model = new();
        private bool isAktif = true;

        // Lookup Data
        private List<KanalIslemResponseDto> kanalIslemler = new();
        private List<KanalAltResponseDto> kanalAltlar = new();

        // Edit Mode Data
        private DateTime eklenmeTarihi = DateTime.Now;
        private DateTime? duzenlenmeTarihi;

        protected override async Task OnInitializedAsync()
        {
            await LoadKanalIslemler();

            if (IsEditMode)
            {
                await LoadKanalAltIslem();
            }
            else
            {
                // Yeni kayıt için varsayılan değerler
                model = new KanalAltIslemFormModel
                {
                    Sira = 1,
                    Aktiflik = Aktiflik.Aktif
                };
                isAktif = true;
            }
        }

        private async Task LoadKanalIslemler()
        {
            try
            {
                var result = await _kanalIslemService.GetAllAsync();

                if (result.Success && result.Data != null)
                {
                    kanalIslemler = result.Data.Where(k => k.Aktiflik == Aktiflik.Aktif).ToList();
                }
                else
                {
                    kanalIslemler = new();
                    await _toastService.ShowWarningAsync("Kanal işlemler yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlemler yüklenirken hata oluştu");
                kanalIslemler = new();
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
                        Sira = altIslem.Sira,
                        Aktiflik = altIslem.Aktiflik
                    };

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
                if (model.KanalIslemId <= 0)
                {
                    await _toastService.ShowErrorAsync("Lütfen bir kanal işlem seçin");
                    return;
                }

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
