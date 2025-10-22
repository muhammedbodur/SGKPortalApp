using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KanalIslem
{
    public partial class Manage
    {
        [Inject] private IKanalIslemApiService _kanalIslemService { get; set; } = default!;
        [Inject] private IKanalApiService _kanalService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;

        [Parameter] public int? KanalIslemId { get; set; }

        // State
        private bool isLoading = false;
        private bool isSaving = false;
        private bool IsEditMode => KanalIslemId.HasValue && KanalIslemId.Value > 0;

        // Form Model
        private KanalIslemFormModel model = new();
        private bool isAktif = true;

        // Edit Mode Data
        private DateTime eklenmeTarihi = DateTime.Now;
        private DateTime? duzenlenmeTarihi;

        // Dropdown Lists
        private List<KanalResponseDto> kanallar = new();
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadDropdownData();

            if (IsEditMode)
            {
                await LoadKanalIslem();
            }
            else
            {
                // Yeni kayıt için varsayılan değerler
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                var kanalTask = _kanalService.GetAllAsync();
                var hizmetBinasiTask = _hizmetBinasiService.GetAllAsync();

                await Task.WhenAll(kanalTask, hizmetBinasiTask);

                var kanalResult = await kanalTask;
                var hizmetBinasiResult = await hizmetBinasiTask;

                if (kanalResult.Success && kanalResult.Data != null)
                {
                    kanallar = kanalResult.Data.Where(k => k.Aktiflik == Aktiflik.Aktif).ToList();
                }

                if (hizmetBinasiResult.Success && hizmetBinasiResult.Data != null)
                {
                    hizmetBinalari = hizmetBinasiResult.Data.Where(h => h.HizmetBinasiAktiflik == Aktiflik.Aktif).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dropdown verileri yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Veriler yüklenirken bir hata oluştu");
            }
        }

        private async Task LoadKanalIslem()
        {
            try
            {
                isLoading = true;
                var result = await _kanalIslemService.GetByIdAsync(KanalIslemId!.Value);

                if (result.Success && result.Data != null)
                {
                    var kanal = result.Data;
                    
                    model = new KanalIslemFormModel
                    {
                        KanalId = kanal.KanalId,
                        HizmetBinasiId = kanal.HizmetBinasiId,
                        KanalIslemAdi = kanal.KanalIslemAdi,
                        Sira = kanal.Sira,
                        Aktiflik = kanal.Aktiflik
                    };

                    isAktif = kanal.Aktiflik == Aktiflik.Aktif;
                    eklenmeTarihi = kanal.EklenmeTarihi;
                    duzenlenmeTarihi = kanal.DuzenlenmeTarihi;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Kanal işlem bulunamadı");
                    _navigationManager.NavigateTo("/siramatik/kanal");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Kanal işlem yüklenirken bir hata oluştu");
                _navigationManager.NavigateTo("/siramatik/kanal");
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

                if (IsEditMode)
                {
                    await UpdateKanalIslem();
                }
                else
                {
                    await CreateKanalIslem();
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

        private async Task CreateKanalIslem()
        {
            // Form modelini KanalIslemCreateRequestDto'ya dönüştür
            var createDto = new KanalIslemCreateRequestDto
            {
                KanalId = model.KanalId,
                HizmetBinasiId = model.HizmetBinasiId,
                KanalIslemAdi = model.KanalIslemAdi,
                Sira = model.Sira,
                Aktiflik = model.Aktiflik
            };

            var result = await _kanalIslemService.CreateAsync(createDto);

            if (result.Success)
            {
                await _toastService.ShowSuccessAsync($"{model.KanalIslemAdi} başarıyla eklendi");
                _navigationManager.NavigateTo("/siramatik/kanal");
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Kanal işlem eklenemedi");
            }
        }

        private async Task UpdateKanalIslem()
        {
            // Form modelini KanalIslemUpdateRequestDto'ya dönüştür
            var updateDto = new KanalIslemUpdateRequestDto
            {
                KanalId = model.KanalId,
                HizmetBinasiId = model.HizmetBinasiId,
                KanalIslemAdi = model.KanalIslemAdi,
                Sira = model.Sira,
                Aktiflik = model.Aktiflik
            };

            var result = await _kanalIslemService.UpdateAsync(KanalIslemId!.Value, updateDto);

            if (result.Success)
            {
                await _toastService.ShowSuccessAsync($"{model.KanalIslemAdi} başarıyla güncellendi");
                _navigationManager.NavigateTo("/siramatik/kanal");
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Kanal işlem güncellenemedi");
            }
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/siramatik/kanal");
        }
    }
}
