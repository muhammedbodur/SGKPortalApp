using Microsoft.AspNetCore.Components;
using SGKPortalApp.BusinessObjectLayer.DTOs.Request.SiramatikIslemleri;
using SGKPortalApp.BusinessObjectLayer.DTOs.Response.Common;
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
    public partial class Manage
    {
        [Parameter]
        public int TvId { get; set; }

        [Inject]
        private ITvApiService _tvService { get; set; } = default!;

        [Inject]
        private IHizmetBinasiApiService _hizmetBinasiService { get; set; } = default!;

        [Inject]
        private IToastService _toastService { get; set; } = default!;

        [Inject]
        private NavigationManager _navigationManager { get; set; } = default!;

        private TvFormModel model = new();
        private List<HizmetBinasiResponseDto> hizmetBinalari = new();

        private bool isLoading = true;
        private bool isSaving = false;
        private bool isAktif = true;

        private DateTime eklenmeTarihi = DateTime.Now;
        private DateTime? duzenlenmeTarihi = null;

        private bool IsEditMode => TvId > 0;

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
                    hizmetBinalari = binaResult.Data
                        .Where(b => b.HizmetBinasiAktiflik == Aktiflik.Aktif)
                        .OrderBy(b => b.HizmetBinasiAdi)
                        .ToList();
                }

                // Edit mode ise TV'yi yükle
                if (IsEditMode)
                {
                    var tvResult = await _tvService.GetByIdAsync(TvId);
                    if (tvResult.Success && tvResult.Data != null)
                    {
                        var tv = tvResult.Data;
                        model = new TvFormModel
                        {
                            TvAdi = tv.TvAdi,
                            HizmetBinasiId = tv.HizmetBinasiId,
                            KatTipi = tv.KatTipi,
                            TvAciklama = tv.TvAciklama
                        };
                        isAktif = tv.TvAktiflik == Aktiflik.Aktif;
                        eklenmeTarihi = tv.EklenmeTarihi;
                        duzenlenmeTarihi = tv.DuzenlenmeTarihi;
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync("TV bulunamadı");
                        NavigateBack();
                    }
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

        private async Task HandleSubmit()
        {
            if (model.HizmetBinasiId == 0)
            {
                await _toastService.ShowWarningAsync("Lütfen hizmet binası seçiniz");
                return;
            }

            isSaving = true;

            try
            {
                if (IsEditMode)
                {
                    var updateDto = new TvUpdateRequestDto
                    {
                        TvId = TvId,
                        TvAdi = model.TvAdi,
                        HizmetBinasiId = model.HizmetBinasiId,
                        KatTipi = model.KatTipi,
                        TvAciklama = model.TvAciklama,
                        TvAktiflik = isAktif ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _tvService.UpdateAsync(updateDto);
                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("TV başarıyla güncellendi");
                        NavigateBack();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Güncelleme başarısız");
                    }
                }
                else
                {
                    var createDto = new TvCreateRequestDto
                    {
                        TvAdi = model.TvAdi,
                        HizmetBinasiId = model.HizmetBinasiId,
                        KatTipi = model.KatTipi,
                        TvAciklama = model.TvAciklama,
                        TvAktiflik = isAktif ? Aktiflik.Aktif : Aktiflik.Pasif
                    };

                    var result = await _tvService.CreateAsync(createDto);
                    if (result.Success)
                    {
                        await _toastService.ShowSuccessAsync("TV başarıyla oluşturuldu");
                        NavigateBack();
                    }
                    else
                    {
                        await _toastService.ShowErrorAsync(result.Message ?? "Oluşturma başarısız");
                    }
                }
            }
            catch (Exception ex)
            {
                await _toastService.ShowErrorAsync($"Hata: {ex.Message}");
            }
            finally
            {
                isSaving = false;
            }
        }

        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/siramatik/tv");
        }

        // Form Model
        private class TvFormModel
        {
            public string TvAdi { get; set; } = string.Empty;
            public int HizmetBinasiId { get; set; }
            public KatTipi KatTipi { get; set; } = KatTipi.zemin;
            public string? TvAciklama { get; set; }
        }
    }
}
