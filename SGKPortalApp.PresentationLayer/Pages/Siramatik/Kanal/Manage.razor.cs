using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.Kanal
{
    public partial class Manage
    {

        [Inject] private IKanalApiService _kanalService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;

        [Parameter] public int? KanalId { get; set; }

        // State
        private bool isLoading = false;
        private bool isSaving = false;
        private bool IsEditMode => KanalId.HasValue && KanalId.Value > 0;

        // Form Model
        private KanalFormModel model = new();
        private bool isAktif = true;

        // Edit Mode Data
        private DateTime eklenmeTarihi = DateTime.Now;
        private DateTime? duzenlenmeTarihi;

        protected override async Task OnInitializedAsync()
        {
            if (IsEditMode)
            {
                await LoadKanal();
            }
            else
            {
                // Yeni kayıt için varsayılan değerler
                model = new KanalFormModel
                {
                    Aktiflik = Aktiflik.Aktif
                };
                isAktif = true;
            }
        }

        private async Task LoadKanal()
        {
            try
            {
                isLoading = true;
                var result = await _kanalService.GetByIdAsync(KanalId!.Value);

                if (result.Success && result.Data != null)
                {
                    var kanal = result.Data;
                    
                    model = new KanalFormModel
                    {
                        KanalAdi = kanal.KanalAdi,
                        Aktiflik = kanal.Aktiflik
                    };

                    isAktif = kanal.Aktiflik == Aktiflik.Aktif;
                    eklenmeTarihi = kanal.EklenmeTarihi;
                    duzenlenmeTarihi = kanal.DuzenlenmeTarihi;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Kanal bulunamadı");
                    _navigationManager.NavigateTo("/siramatik/kanal");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kanal yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Kanal yüklenirken bir hata oluştu");
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
                    await UpdateKanal();
                }
                else
                {
                    await CreateKanal();
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

        private async Task CreateKanal()
        {
            // Form modelini KanalCreateRequestDto'ya dönüştür
            var createDto = new KanalCreateRequestDto
            {
                KanalAdi = model.KanalAdi,
                Aktiflik = model.Aktiflik
            };

            var result = await _kanalService.CreateAsync(createDto);

            if (result.Success)
            {
                await _toastService.ShowSuccessAsync($"{model.KanalAdi} başarıyla eklendi");
                _navigationManager.NavigateTo("/siramatik/kanal");
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Kanal eklenemedi");
            }
        }

        private async Task UpdateKanal()
        {
            // Form modelini KanalUpdateRequestDto'ya dönüştür
            var updateDto = new KanalUpdateRequestDto
            {
                KanalAdi = model.KanalAdi,
                Aktiflik = model.Aktiflik
            };

            var result = await _kanalService.UpdateAsync(KanalId!.Value, updateDto);

            if (result.Success)
            {
                await _toastService.ShowSuccessAsync($"{model.KanalAdi} başarıyla güncellendi");
                _navigationManager.NavigateTo("/siramatik/kanal");
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Kanal güncellenemedi");
            }
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/siramatik/kanal");
        }
    }
}
