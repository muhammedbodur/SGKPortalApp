using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.KioskMenuAtama
{
    public partial class Index
    {
        [Inject] private IKioskMenuAtamaApiService _kioskMenuAtamaService { get; set; } = default!;
        [Inject] private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;
        [Inject] private IKioskApiService _kioskService { get; set; } = default!;
        [Inject] private IToastService _toastService { get; set; } = default!;
        [Inject] private ILogger<Index> _logger { get; set; } = default!;

        private bool isLoading;
        private bool isDeleting;
        private bool showDeleteModal;

        private List<HizmetBinasiResponseDto> hizmetBinalari = new();
        private List<KioskResponseDto> kiosklar = new();
        private List<KioskMenuAtamaResponseDto> allAtamalar = new();
        private List<KioskMenuAtamaResponseDto> filteredAtamalar = new();

        private int selectedHizmetBinasiId;
        private int selectedKioskId;
        private Aktiflik? selectedAktiflik;
        private KioskMenuAtamaResponseDto? selectedAtama;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                isLoading = true;

                // Tüm aktif hizmet binalarını al ve alfabetik sırala
                var binaResult = await _hizmetBinasiService.GetAllAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data
                        .Where(x => x.Aktiflik == Aktiflik.Aktif)
                        .OrderBy(x => x.HizmetBinasiAdi)
                        .ToList();

                    // İlk hizmet binasını otomatik seç (alfabetik sırayla)
                    if (hizmetBinalari.Any())
                    {
                        selectedHizmetBinasiId = hizmetBinalari.First().HizmetBinasiId;
                        await OnHizmetBinasiChangedAsync();
                        // OnHizmetBinasiChangedAsync içinde LoadAtamalarAsync çağrılıyor
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Veriler yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Veriler yüklenirken bir hata oluştu");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task LoadAtamalarAsync()
        {
            try
            {
                // Sadece seçili kiosk'a ait atamaları getir
                if (selectedKioskId > 0)
                {
                    var result = await _kioskMenuAtamaService.GetByKioskAsync(selectedKioskId);

                    if (result.Success && result.Data != null)
                    {
                        allAtamalar = result.Data;
                        ApplyFilters();
                    }
                    else
                    {
                        allAtamalar = new();
                        filteredAtamalar = new();
                    }
                }
                else
                {
                    allAtamalar = new();
                    filteredAtamalar = new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atamalar yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Atamalar yüklenirken bir hata oluştu");
            }
        }

        private async Task OnHizmetBinasiChangedAsync()
        {
            try
            {
                selectedKioskId = 0;
                kiosklar = new();

                if (selectedHizmetBinasiId > 0)
                {
                    // Seçili binaya ait kiosları yükle ve alfabetik sırala
                    var result = await _kioskService.GetByHizmetBinasiAsync(selectedHizmetBinasiId);
                    if (result.Success && result.Data != null)
                    {
                        kiosklar = result.Data
                            .Where(x => x.Aktiflik == Aktiflik.Aktif)
                            .OrderBy(x => x.KioskAdi)
                            .ToList();
                        
                        // İlk kiosku otomatik seç (alfabetik sırayla)
                        if (kiosklar.Any())
                        {
                            selectedKioskId = kiosklar.First().KioskId;
                            await LoadAtamalarAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kiosk listesi yüklenirken hata oluştu");
                await _toastService.ShowErrorAsync("Kiosk listesi yüklenemedi");
            }
        }

        private async Task OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int binaId))
            {
                selectedHizmetBinasiId = binaId;
                await OnHizmetBinasiChangedAsync();
            }
        }

        private async Task OnKioskChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int kioskId))
            {
                selectedKioskId = kioskId;
                await LoadAtamalarAsync();
            }
        }

        private void OnAktiflikChanged(ChangeEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value?.ToString()))
            {
                selectedAktiflik = null;
            }
            else if (int.TryParse(e.Value?.ToString(), out int aktiflikValue))
            {
                selectedAktiflik = (Aktiflik)aktiflikValue;
            }
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            filteredAtamalar = allAtamalar;

            if (selectedKioskId > 0)
            {
                filteredAtamalar = filteredAtamalar.Where(a => a.KioskId == selectedKioskId).ToList();
            }

            if (selectedAktiflik.HasValue)
            {
                filteredAtamalar = filteredAtamalar.Where(a => a.Aktiflik == selectedAktiflik.Value).ToList();
            }
        }

        private async Task ToggleAktiflik(KioskMenuAtamaResponseDto atama)
        {
            try
            {
                var updateDto = new KioskMenuAtamaUpdateRequestDto
                {
                    KioskMenuAtamaId = atama.KioskMenuAtamaId,
                    KioskId = atama.KioskId,
                    KioskMenuId = atama.KioskMenuId,
                    Aktiflik = atama.Aktiflik == Aktiflik.Aktif ? Aktiflik.Pasif : Aktiflik.Aktif
                };

                var result = await _kioskMenuAtamaService.UpdateAsync(updateDto);
                if (result.Success)
                {
                    atama.Aktiflik = updateDto.Aktiflik;
                    await _toastService.ShowSuccessAsync($"Atama {(atama.Aktiflik == Aktiflik.Aktif ? "aktif" : "pasif")} yapıldı");
                    ApplyFilters();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Durum değiştirilemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atama aktiflik değiştirilemedi");
                await _toastService.ShowErrorAsync("Durum değiştirilirken hata oluştu");
            }
        }

        private void ShowDeleteConfirm(KioskMenuAtamaResponseDto atama)
        {
            selectedAtama = atama;
            showDeleteModal = true;
        }

        private void HideDeleteConfirm()
        {
            showDeleteModal = false;
            selectedAtama = null;
        }

        private async Task DeleteAtama()
        {
            if (selectedAtama == null) return;

            try
            {
                isDeleting = true;
                var result = await _kioskMenuAtamaService.DeleteAsync(selectedAtama.KioskMenuAtamaId);

                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("Atama başarıyla silindi");
                    await LoadAtamalarAsync();
                    HideDeleteConfirm();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Atama silinemedi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Atama silinirken hata oluştu. Id: {Id}", selectedAtama.KioskMenuAtamaId);
                await _toastService.ShowErrorAsync("Atama silinirken bir hata oluştu");
            }
            finally
            {
                isDeleting = false;
            }
        }

        private async Task RefreshAsync()
        {
            await LoadDataAsync();
            await _toastService.ShowSuccessAsync("Veriler yenilendi");
        }
    }
}
