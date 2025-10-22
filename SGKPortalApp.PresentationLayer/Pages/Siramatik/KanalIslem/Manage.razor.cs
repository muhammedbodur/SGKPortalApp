using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

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

        // Dropdown Data
        private List<KanalResponseDto> anaKanallar = new();
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();

        // Edit Mode Data
        private DateTime eklenmeTarihi = DateTime.Now;
        private DateTime? duzenlenmeTarihi;

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
                model = new KanalIslemFormModel
                {
                    BaslangicNumara = 0,
                    BitisNumara = 9999,
                    Aktiflik = Aktiflik.Aktif
                };
                isAktif = true;
            }
        }

        private async Task LoadDropdownData()
        {
            try
            {
                // Ana kanalları yükle
                var kanalResult = await _kanalService.GetActiveAsync();
                if (kanalResult.Success && kanalResult.Data != null)
                {
                    anaKanallar = kanalResult.Data;
                }

                // Hizmet binalarını yükle
                var binaResult = await _hizmetBinasiService.GetActiveAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data;
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
                    var kanalIslem = result.Data;

                    model = new KanalIslemFormModel
                    {
                        KanalId = kanalIslem.KanalId,
                        HizmetBinasiId = kanalIslem.HizmetBinasiId,
                        KanalIslemAdi = kanalIslem.KanalIslemAdi,
                        BaslangicNumara = kanalIslem.BaslangicNumara,
                        BitisNumara = kanalIslem.BitisNumara,
                        Aktiflik = kanalIslem.Aktiflik
                    };

                    isAktif = kanalIslem.Aktiflik == Aktiflik.Aktif;
                    eklenmeTarihi = kanalIslem.EklenmeTarihi;
                    duzenlenmeTarihi = kanalIslem.DuzenlenmeTarihi;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Kanal işlem bulunamadı");
                    _navigationManager.NavigateTo("/siramatik/kanalislem");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal işlem yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Kanal işlem yüklenirken bir hata oluştu");
                _navigationManager.NavigateTo("/siramatik/kanalislem");
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
            var createDto = new KanalIslemCreateRequestDto
            {
                KanalId = model.KanalId,
                HizmetBinasiId = model.HizmetBinasiId,
                BaslangicNumara = model.BaslangicNumara,
                BitisNumara = model.BitisNumara,
                Aktiflik = model.Aktiflik
            };

            var result = await _kanalIslemService.CreateAsync(createDto);

            if (result.Success)
            {
                await _toastService.ShowSuccessAsync($"{model.KanalIslemAdi} başarıyla eklendi");
                _navigationManager.NavigateTo("/siramatik/kanalislem");
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Kanal işlem eklenemedi");
            }
        }

        private async Task UpdateKanalIslem()
        {
            var updateDto = new KanalIslemUpdateRequestDto
            {
                KanalId = model.KanalId,
                HizmetBinasiId = model.HizmetBinasiId,
                BaslangicNumara = model.BaslangicNumara,
                BitisNumara = model.BitisNumara,
                Aktiflik = model.Aktiflik
            };

            var result = await _kanalIslemService.UpdateAsync(KanalIslemId!.Value, updateDto);

            if (result.Success)
            {
                await _toastService.ShowSuccessAsync($"{model.KanalIslemAdi} başarıyla güncellendi");
                _navigationManager.NavigateTo("/siramatik/kanalislem");
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Kanal işlem güncellenemedi");
            }
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/siramatik/kanalislem");
        }
    }
}