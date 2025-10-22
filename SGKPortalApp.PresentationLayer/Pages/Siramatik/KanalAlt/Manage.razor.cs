using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Models.FormModels.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KanalAlt
{
    public partial class Manage
    {
        [Inject] private IKanalAltApiService _kanalAltService { get; set; } = default!;
        [Inject] private IKanalApiService _kanalService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;
        [Inject] private ILogger<Manage> _logger { get; set; } = default!;

        [Parameter] public int? KanalAltId { get; set; }

        // State
        private bool isLoading = false;
        private bool isSaving = false;
        private bool IsEditMode => KanalAltId.HasValue && KanalAltId.Value > 0;

        // Form Model
        private KanalAltFormModel model = new();
        private bool isAktif = true;

        // Dropdown Data
        private List<KanalResponseDto> anaKanallar = new();

        // Edit Mode Data
        private DateTime eklenmeTarihi = DateTime.Now;
        private DateTime? duzenlenmeTarihi;

        protected override async Task OnInitializedAsync()
        {
            await LoadAnaKanallar();

            if (IsEditMode)
            {
                await LoadKanalAlt();
            }
            else
            {
                // Yeni kayıt için varsayılan değerler
                model = new KanalAltFormModel
                {
                    Aktiflik = Aktiflik.Aktif
                };
                isAktif = true;
            }
        }

        private async Task LoadAnaKanallar()
        {
            try
            {
                var result = await _kanalService.GetAllAsync();
                if (result.Success && result.Data != null)
                {
                    anaKanallar = result.Data.Where(k => k.Aktiflik == Aktiflik.Aktif).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ana kanallar yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Ana kanallar yüklenemedi");
            }
        }

        private async Task LoadKanalAlt()
        {
            try
            {
                isLoading = true;
                var result = await _kanalAltService.GetByIdAsync(KanalAltId!.Value);

                if (result.Success && result.Data != null)
                {
                    var kanalAlt = result.Data;
                    
                    model = new KanalAltFormModel
                    {
                        KanalId = kanalAlt.KanalId,
                        KanalAltAdi = kanalAlt.KanalAltAdi,
                        Aktiflik = kanalAlt.Aktiflik
                    };

                    isAktif = kanalAlt.Aktiflik == Aktiflik.Aktif;
                    eklenmeTarihi = kanalAlt.EklenmeTarihi;
                    duzenlenmeTarihi = kanalAlt.DuzenlenmeTarihi;
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Alt kanal bulunamadı");
                    _navigationManager.NavigateTo("/siramatik/alt-kanal");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Alt kanal yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Alt kanal yüklenirken bir hata oluştu");
                _navigationManager.NavigateTo("/siramatik/alt-kanal");
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
                    await UpdateKanalAlt();
                }
                else
                {
                    await CreateKanalAlt();
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

        private async Task CreateKanalAlt()
        {
            var createDto = new KanalAltKanalCreateRequestDto
            {
                KanalId = model.KanalId,
                KanalAltAdi = model.KanalAltAdi,
                Aktiflik = model.Aktiflik
            };

            var result = await _kanalAltService.CreateAsync(createDto);

            if (result.Success)
            {
                await _toastService.ShowSuccessAsync($"{model.KanalAltAdi} başarıyla eklendi");
                _navigationManager.NavigateTo("/siramatik/alt-kanal");
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Alt kanal eklenemedi");
            }
        }

        private async Task UpdateKanalAlt()
        {
            var updateDto = new KanalAltKanalUpdateRequestDto
            {
                KanalId = model.KanalId,
                KanalAltAdi = model.KanalAltAdi,
                Aktiflik = model.Aktiflik
            };

            var result = await _kanalAltService.UpdateAsync(KanalAltId!.Value, updateDto);

            if (result.Success)
            {
                await _toastService.ShowSuccessAsync($"{model.KanalAltAdi} başarıyla güncellendi");
                _navigationManager.NavigateTo("/siramatik/alt-kanal");
            }
            else
            {
                await _toastService.ShowErrorAsync(result.Message ?? "Alt kanal güncellenemedi");
            }
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/siramatik/alt-kanal");
        }
    }
}
