using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.Enums.Common;
using SGKPortalApp.BusinessObjectLayer.Enums.SiramatikIslemleri;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Common;
using SGKPortalApp.PresentationLayer.Services.ApiServices.Interfaces.Siramatik;
using SGKPortalApp.PresentationLayer.Services.UIServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGKPortalApp.PresentationLayer.Pages.Siramatik.Tv
{
    public partial class Index
    {
        [Inject]
        private ITvApiService _tvService { get; set; } = default!;

        [Inject]
        private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;

        [Inject]
        private IToastService _toastService { get; set; } = default!;

        [Inject]
        private NavigationManager _navigationManager { get; set; } = default!;

        private List<TvResponseDto> allTvler = new();
        private List<TvResponseDto> filteredTvler = new();
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();

        private int? selectedHizmetBinasiId = null;
        private KatTipi? selectedKatTipi = null;
        private Aktiflik? selectedAktiflik = null;
        private bool? selectedConnection = null;

        private bool isLoading = true;
        private bool showDeleteModal = false;
        private bool isDeleting = false;
        private TvResponseDto? selectedTv = null;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            isLoading = true;

            try
            {
                // Hizmet binalarını yükle
                var binaResult = await _hizmetBinasiService.GetAllAsync();
                if (binaResult.Success && binaResult.Data != null)
                {
                    hizmetBinalari = binaResult.Data.OrderBy(b => b.HizmetBinasiAdi).ToList();
                }

                // TV'leri yükle
                var tvResult = await _tvService.GetAllAsync();
                if (tvResult.Success && tvResult.Data != null)
                {
                    allTvler = tvResult.Data.OrderBy(t => t.HizmetBinasiAdi).ThenBy(t => t.TvAdi).ToList();
                    ApplyFilters();
                }
                else
                {
                    await _toastService.ShowErrorAsync(tvResult.Message ?? "TV'ler yüklenemedi");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Veri yüklenirken hata oluştu: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ApplyFilters()
        {
            filteredTvler = allTvler.ToList();

            if (selectedHizmetBinasiId.HasValue)
            {
                filteredTvler = filteredTvler.Where(t => t.HizmetBinasiId == selectedHizmetBinasiId.Value).ToList();
            }

            if (selectedKatTipi.HasValue)
            {
                filteredTvler = filteredTvler.Where(t => t.KatTipi == selectedKatTipi.Value).ToList();
            }

            if (selectedAktiflik.HasValue)
            {
                filteredTvler = filteredTvler.Where(t => t.Aktiflik == selectedAktiflik.Value).ToList();
            }

            if (selectedConnection.HasValue)
            {
                filteredTvler = filteredTvler.Where(t => t.IsConnected == selectedConnection.Value).ToList();
            }

            filteredTvler = filteredTvler.ToList();
        }

        private void OnHizmetBinasiChanged(ChangeEventArgs e)
        {
            selectedHizmetBinasiId = string.IsNullOrEmpty(e.Value?.ToString()) ? null : int.Parse(e.Value.ToString()!);
            ApplyFilters();
        }

        private void OnKatTipiChanged(ChangeEventArgs e)
        {
            selectedKatTipi = string.IsNullOrEmpty(e.Value?.ToString()) ? null : (KatTipi)int.Parse(e.Value.ToString()!);
            ApplyFilters();
        }

        private void OnAktiflikChanged(ChangeEventArgs e)
        {
            selectedAktiflik = string.IsNullOrEmpty(e.Value?.ToString()) ? null : (Aktiflik)int.Parse(e.Value.ToString()!);
            ApplyFilters();
        }

        private void OnConnectionChanged(ChangeEventArgs e)
        {
            selectedConnection = string.IsNullOrEmpty(e.Value?.ToString()) ? null : bool.Parse(e.Value.ToString()!);
            ApplyFilters();
        }

        private async Task RefreshAsync()
        {
            await LoadDataAsync();
            await _toastService.ShowSuccessAsync("TV listesi yenilendi");
        }

        private async Task ToggleAktiflik(TvResponseDto tv)
        {
            try
            {
                var updateDto = new TvUpdateRequestDto
                {
                    TvId = tv.TvId,
                    TvAdi = tv.TvAdi,
                    HizmetBinasiId = tv.HizmetBinasiId,
                    KatTipi = tv.KatTipi,
                    TvAciklama = tv.TvAciklama,
                    Aktiflik = tv.Aktiflik == Aktiflik.Aktif ? Aktiflik.Pasif : Aktiflik.Aktif
                };

                var result = await _tvService.UpdateAsync(updateDto);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync($"TV {updateDto.Aktiflik} yapıldı");
                    await LoadDataAsync();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "İşlem başarısız");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
        }

        private void ShowDeleteConfirm(TvResponseDto tv)
        {
            selectedTv = tv;
            showDeleteModal = true;
        }

        private void HideDeleteConfirm()
        {
            showDeleteModal = false;
            selectedTv = null;
        }

        private async Task DeleteTv()
        {
            if (selectedTv == null) return;

            isDeleting = true;

            try
            {
                var result = await _tvService.DeleteAsync(selectedTv.TvId);
                if (result.Success)
                {
                    await _toastService.ShowSuccessAsync("TV başarıyla silindi");
                    HideDeleteConfirm();
                    await LoadDataAsync();
                }
                else
                {
                    await _toastService.ShowErrorAsync(result.Message ?? "Silme işlemi başarısız");
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                isDeleting = false;
            }
        }
    }
}
